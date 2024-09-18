﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pancake.Common;
using Pancake.Linq;
using UnityEngine;

namespace Pancake.UI
{
    [DisallowMultipleComponent]
    public class Sheet : MonoBehaviour, ISheetLifecycleEvent
    {
        [field: SerializeField] private string Id { get; set; }

        [SerializeField] private int order;

        [SerializeField] protected List<TransitionAnimation> enterAnimations = new();
        [SerializeField] protected List<TransitionAnimation> exitAnimations = new();

        private CanvasGroup _canvasGroup;
        private RectTransform _parentTransform;
        private RectTransform _rectTransform;
        private Progress<float> _transitionProgressReporter;

        private Progress<float> TransitionProgressReporter
        {
            get
            {
                if (_transitionProgressReporter == null) _transitionProgressReporter = new Progress<float>(SetTransitionProgress);
                return _transitionProgressReporter;
            }
        }

        private readonly CompositeLifecycleEvent<ISheetLifecycleEvent> _lifecycleEvents = new();

        public bool IsTransitioning { get; private set; }

        /// <summary>
        ///     Return the transition animation type currently playing.
        ///     If not in transition, return null.
        /// </summary>
        public SheetTransitionType? TransitionAnimationType { get; private set; }

        /// <summary>
        ///     Progress of the transition animation.
        /// </summary>
        public float TransitionAnimationProgress { get; private set; }

        /// <summary>
        ///     Event when the transition animation progress changes.
        /// </summary>
        public event Action<float> TransitionAnimationProgressChanged;

        public ITransitionAnimation GetTransitionAnimation(bool enter, string partnerTransitionIdentifier)
        {
            var anims = enter ? enterAnimations : exitAnimations;
            var anim = anims.FirstOrDefault(x => x.IsValid(partnerTransitionIdentifier));
            var result = anim?.GetAnimation();
            return result;
        }

        public virtual Task Initialize() { return Task.CompletedTask; }


        public virtual Task WillEnter() { return Task.CompletedTask; }


        public virtual void DidEnter() { }

        public virtual Task WillExit() { return Task.CompletedTask; }


        public virtual void DidExit() { }

        public virtual Task Cleanup() { return Task.CompletedTask; }


        public void AddLifecycleEvent(ISheetLifecycleEvent lifecycleEvent, int priority = 0) { _lifecycleEvents.AddItem(lifecycleEvent, priority); }

        public void RemoveLifecycleEvent(ISheetLifecycleEvent lifecycleEvent) { _lifecycleEvents.RemoveItem(lifecycleEvent); }

        internal AsyncProcessHandle AfterLoad(RectTransform parentTransform)
        {
            _rectTransform = (RectTransform) transform;
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            _lifecycleEvents.AddItem(this, 0);
            _parentTransform = parentTransform;
            _rectTransform.FillWithParent(_parentTransform);

            // Set order of rendering.
            var siblingIndex = 0;
            for (var i = 0; i < _parentTransform.childCount; i++)
            {
                var child = _parentTransform.GetChild(i);
                var childPage = child.GetComponent<Sheet>();
                siblingIndex = i;
                if (order >= childPage.order) continue;

                break;
            }

            _rectTransform.SetSiblingIndex(siblingIndex);

            gameObject.SetActive(false);

            return App.StartCoroutine(CreateCoroutine(_lifecycleEvents.ExecuteLifecycleEventsSequentially(x => x.Initialize())));
        }

        internal AsyncProcessHandle BeforeEnter(Sheet partnerSheet) { return App.StartCoroutine(BeforeEnterRoutine(partnerSheet)); }

        private IEnumerator BeforeEnterRoutine(Sheet partnerSheet)
        {
            IsTransitioning = true;
            TransitionAnimationType = SheetTransitionType.Enter;
            gameObject.SetActive(true);
            _rectTransform.FillWithParent(_parentTransform);
            SetTransitionProgress(0.0f);

            _canvasGroup.alpha = 0.0f;

            var handle = App.StartCoroutine(CreateCoroutine(_lifecycleEvents.ExecuteLifecycleEventsSequentially(x => x.WillEnter())));
            while (!handle.IsTerminated)
                yield return null;
        }

        internal AsyncProcessHandle Enter(bool playAnimation, Sheet partnerSheet) { return App.StartCoroutine(EnterRoutine(playAnimation, partnerSheet)); }

        private IEnumerator EnterRoutine(bool playAnimation, Sheet partnerSheet)
        {
            _canvasGroup.alpha = 1.0f;

            if (playAnimation)
            {
                var anim = GetTransitionAnimation(true, partnerSheet?.Id);
                if (anim == null) anim = DefaultNavigatorSetting.GetDefaultSheetTransition(true);

                if (anim.Duration > 0.0f)
                {
                    anim.SetPartner(partnerSheet?.transform as RectTransform);
                    anim.Setup(_rectTransform);
                    yield return App.StartCoroutine(anim.CreateRoutine(TransitionProgressReporter));
                }
            }

            _rectTransform.FillWithParent(_parentTransform);
            SetTransitionProgress(1.0f);
        }

        internal void AfterEnter(Sheet partnerSheet)
        {
            _lifecycleEvents.ExecuteLifecycleEventsSequentially(x => x.DidEnter());
            IsTransitioning = false;
            TransitionAnimationType = null;
        }

        internal AsyncProcessHandle BeforeExit(Sheet partnerSheet) { return App.StartCoroutine(BeforeExitRoutine(partnerSheet)); }

        private IEnumerator BeforeExitRoutine(Sheet partnerSheet)
        {
            IsTransitioning = true;
            TransitionAnimationType = SheetTransitionType.Exit;
            gameObject.SetActive(true);
            _rectTransform.FillWithParent(_parentTransform);
            SetTransitionProgress(0.0f);

            _canvasGroup.alpha = 1.0f;

            var handle = App.StartCoroutine(CreateCoroutine(_lifecycleEvents.ExecuteLifecycleEventsSequentially(x => x.WillExit())));
            while (!handle.IsTerminated)
                yield return null;
        }

        internal AsyncProcessHandle Exit(bool playAnimation, Sheet partnerSheet) { return App.StartCoroutine(ExitRoutine(playAnimation, partnerSheet)); }

        private IEnumerator ExitRoutine(bool playAnimation, Sheet partnerSheet)
        {
            if (playAnimation)
            {
                var anim = GetTransitionAnimation(false, partnerSheet?.Id);
                if (anim == null) anim = DefaultNavigatorSetting.GetDefaultSheetTransition(false);

                if (anim.Duration > 0.0f)
                {
                    anim.SetPartner(partnerSheet?.transform as RectTransform);
                    anim.Setup(_rectTransform);
                    yield return App.StartCoroutine(anim.CreateRoutine(TransitionProgressReporter));
                }
            }

            _canvasGroup.alpha = 0.0f;
            SetTransitionProgress(1.0f);
        }

        internal void AfterExit(Sheet partnerSheet)
        {
            _lifecycleEvents.ExecuteLifecycleEventsSequentially(x => x.DidExit());
            gameObject.SetActive(false);
            IsTransitioning = false;
            TransitionAnimationType = null;
        }

        internal void BeforeReleaseAndForget() { _ = _lifecycleEvents.ExecuteLifecycleEventsSequentially(x => x.Cleanup()); }

        private IEnumerator CreateCoroutine(IEnumerable<Task> targets)
        {
            foreach (var target in targets)
            {
                var handle = App.StartCoroutine(CreateCoroutine(target));
                if (!handle.IsTerminated)
                    yield return handle;
            }
        }

        private IEnumerator CreateCoroutine(Task target)

        {
            async void WaitTaskAndCallback(Task task, Action callback)
            {
                await task;
                callback?.Invoke();
            }

            var isCompleted = false;
            WaitTaskAndCallback(target, () => { isCompleted = true; });
            return new WaitUntil(() => isCompleted);
        }

        private void SetTransitionProgress(float progress)
        {
            TransitionAnimationProgress = progress;
            TransitionAnimationProgressChanged?.Invoke(progress);
        }
    }
}