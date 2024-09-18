﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using Pancake.AssetLoader;
using Pancake.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Pancake.UI
{
    [RequireComponent(typeof(RectMask2D))]
    [EditorIcon("icon_popup")]
    public sealed class SheetContainer : MonoBehaviour, IUIContainer
    {
        private static readonly Dictionary<int, SheetContainer> InstanceCacheByTransform = new();
        private static readonly Dictionary<string, SheetContainer> InstanceCacheByName = new();

        [SerializeField, LabelText("Name")] private string displayName;

        private readonly Dictionary<string, AssetLoadHandle<GameObject>> _assetLoadHandles = new();
        private readonly List<ISheetContainerCallbackReceiver> _callbackReceivers = new();
        private readonly Dictionary<string, string> _sheetNameToId = new();
        private readonly Dictionary<string, Sheet> _sheets = new();
        private IAssetLoader _assetLoader;
        private CanvasGroup _canvasGroup;

        public static List<SheetContainer> Instances { get; } = new();

        /// <summary>
        ///     By default, <see cref="IAssetLoader" /> in <see cref="DefaultNavigatorSetting" /> is used.
        ///     If this property is set, it is used instead.
        /// </summary>
        public IAssetLoader AssetLoader { get => _assetLoader ?? DefaultNavigatorSetting.AssetLoader; set => _assetLoader = value; }

        public string ActiveSheetId { get; private set; }

        public Sheet ActiveSheet
        {
            get
            {
                if (ActiveSheetId == null) return null;

                return _sheets[ActiveSheetId];
            }
        }

        /// <summary>
        ///     True if in transition.
        /// </summary>
        public bool IsInTransition { get; private set; }

        /// <summary>
        ///     Registered sheets.
        /// </summary>
        public IReadOnlyDictionary<string, Sheet> Sheets => _sheets;

        public bool Interactable { get => _canvasGroup.interactable; set => _canvasGroup.interactable = value; }

        private void Awake()
        {
            Instances.Add(this);

            _callbackReceivers.AddRange(GetComponents<ISheetContainerCallbackReceiver>());

            if (!string.IsNullOrWhiteSpace(displayName)) InstanceCacheByName.Add(displayName, this);
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            UnregisterAll();

            InstanceCacheByName.Remove(displayName);
            var keysToRemove = new List<int>();
            foreach (var cache in InstanceCacheByTransform)
            {
                if (Equals(cache.Value)) keysToRemove.Add(cache.Key);
            }

            foreach (var keyToRemove in keysToRemove) InstanceCacheByTransform.Remove(keyToRemove);

            Instances.Remove(this);
        }

        /// <summary>
        ///     Get the <see cref="SheetContainer" /> that manages the sheet to which <see cref="transform" /> belongs.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="transform" />.</param>
        /// <returns></returns>
        public static SheetContainer Of(Transform transform, bool useCache = true) { return Of((RectTransform) transform, useCache); }

        /// <summary>
        ///     Get the <see cref="SheetContainer" /> that manages the sheet to which <see cref="rectTransform" /> belongs.
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="rectTransform" />.</param>
        /// <returns></returns>
        public static SheetContainer Of(RectTransform rectTransform, bool useCache = true)
        {
            var hashCode = rectTransform.GetInstanceID();

            if (useCache && InstanceCacheByTransform.TryGetValue(hashCode, out var container)) return container;

            container = rectTransform.GetComponentInParent<SheetContainer>();
            if (container != null)
            {
                InstanceCacheByTransform.Add(hashCode, container);
                return container;
            }

            return null;
        }

        /// <summary>
        ///     Find the <see cref="SheetContainer" /> of <see cref="containerName" />.
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static SheetContainer Find(string containerName)
        {
            if (InstanceCacheByName.TryGetValue(containerName, out var instance)) return instance;

            return null;
        }

        /// <summary>
        ///     Add a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void AddCallbackReceiver(ISheetContainerCallbackReceiver callbackReceiver) { _callbackReceivers.Add(callbackReceiver); }

        /// <summary>
        ///     Remove a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void RemoveCallbackReceiver(ISheetContainerCallbackReceiver callbackReceiver) { _callbackReceivers.Remove(callbackReceiver); }

        /// <summary>
        ///     Show a sheet.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="playAnimation"></param>
        /// <returns></returns>
        public AsyncProcessHandle ShowByResourceKey(string resourceKey, bool playAnimation)
        {
            return App.StartCoroutine(ShowByResourceKeyRoutine(resourceKey, playAnimation));
        }

        /// <summary>
        ///     Show a sheet.
        /// </summary>
        /// <param name="sheetId"></param>
        /// <param name="playAnimation"></param>
        /// <returns></returns>
        public AsyncProcessHandle Show(string sheetId, bool playAnimation) { return App.StartCoroutine(ShowRoutine(sheetId, playAnimation)); }

        /// <summary>
        ///     Hide a sheet.
        /// </summary>
        /// <param name="playAnimation"></param>
        public AsyncProcessHandle Hide(bool playAnimation) { return App.StartCoroutine(HideRoutine(playAnimation)); }

        /// <summary>
        ///     Register a sheet.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="onLoad"></param>
        /// <param name="loadAsync"></param>
        /// <param name="sheetId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AsyncProcessHandle Register(string resourceKey, Action<(string sheetId, Sheet sheet)> onLoad = null, bool loadAsync = true, string sheetId = null)
        {
            return App.StartCoroutine(RegisterRoutine(typeof(Sheet),
                resourceKey,
                onLoad,
                loadAsync,
                sheetId));
        }

        /// <summary>
        ///     Register a sheet.
        /// </summary>
        /// <param name="sheetType"></param>
        /// <param name="resourceKey"></param>
        /// <param name="onLoad"></param>
        /// <param name="loadAsync"></param>
        /// <param name="sheetId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AsyncProcessHandle Register(
            Type sheetType,
            string resourceKey,
            Action<(string sheetId, Sheet sheet)> onLoad = null,
            bool loadAsync = true,
            string sheetId = null)
        {
            return App.StartCoroutine(RegisterRoutine(sheetType,
                resourceKey,
                onLoad,
                loadAsync,
                sheetId));
        }

        /// <summary>
        ///     Register a sheet.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="onLoad"></param>
        /// <param name="loadAsync"></param>
        /// <param name="sheetId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AsyncProcessHandle Register<TSheet>(string resourceKey, Action<(string sheetId, TSheet sheet)> onLoad = null, bool loadAsync = true, string sheetId = null)
            where TSheet : Sheet
        {
            return App.StartCoroutine(RegisterRoutine(typeof(TSheet),
                resourceKey,
                x => onLoad?.Invoke((x.sheetId, (TSheet) x.sheet)),
                loadAsync,
                sheetId));
        }

        private IEnumerator RegisterRoutine(
            Type sheetType,
            string resourceKey,
            Action<(string sheetId, Sheet sheet)> onLoad = null,
            bool loadAsync = true,
            string sheetId = null)
        {
            if (resourceKey == null) throw new ArgumentNullException(nameof(resourceKey));

            var assetLoadHandle = loadAsync ? AssetLoader.LoadAsync<GameObject>(resourceKey) : AssetLoader.Load<GameObject>(resourceKey);
            while (!assetLoadHandle.IsDone) yield return null;

            if (assetLoadHandle.Status == AssetLoadStatus.Failed) throw assetLoadHandle.OperationException;

            var instance = Instantiate(assetLoadHandle.Result);
            if (!instance.TryGetComponent(sheetType, out var c))
                c = instance.AddComponent(sheetType);
            var sheet = (Sheet) c;

            if (sheetId == null) sheetId = Guid.NewGuid().ToString();
            _sheets.Add(sheetId, sheet);
            _sheetNameToId[resourceKey] = sheetId;
            _assetLoadHandles.Add(sheetId, assetLoadHandle);
            onLoad?.Invoke((sheetId, sheet));
            var afterLoadHandle = sheet.AfterLoad((RectTransform) transform);
            while (!afterLoadHandle.IsTerminated) yield return null;

            yield return sheetId;
        }

        private IEnumerator ShowByResourceKeyRoutine(string resourceKey, bool playAnimation)
        {
            var sheetId = _sheetNameToId[resourceKey];
            yield return ShowRoutine(sheetId, playAnimation);
        }

        private IEnumerator ShowRoutine(string sheetId, bool playAnimation)
        {
            if (IsInTransition) throw new InvalidOperationException("Cannot transition because the screen is already in transition.");

            if (ActiveSheetId != null && ActiveSheetId == sheetId) throw new InvalidOperationException("Cannot transition because the sheet is already active.");

            IsInTransition = true;

            if (!DefaultNavigatorSetting.EnableInteractionInTransition)
            {
                if (DefaultNavigatorSetting.ControlInteractionAllContainer)
                {
                    foreach (var pageContainer in PageContainer.Instances) pageContainer.Interactable = false;
                    foreach (var popupContainer in PopupContainer.Instances) popupContainer.Interactable = false;
                    foreach (var sheetContainer in Instances) sheetContainer.Interactable = false;
                }
                else
                {
                    Interactable = false;
                }
            }

            var enterSheet = _sheets[sheetId];
            var exitSheet = ActiveSheetId != null ? _sheets[ActiveSheetId] : null;

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.BeforeShow(enterSheet, exitSheet);

            var preprocessHandles = new List<AsyncProcessHandle>();
            if (exitSheet != null) preprocessHandles.Add(exitSheet.BeforeExit(enterSheet));

            preprocessHandles.Add(enterSheet.BeforeEnter(exitSheet));
            foreach (var coroutineHandle in preprocessHandles)
            {
                while (!coroutineHandle.IsTerminated)
                    yield return null;
            }

            // Play Animation
            var animationHandles = new List<AsyncProcessHandle>();
            if (exitSheet != null) animationHandles.Add(exitSheet.Exit(playAnimation, enterSheet));

            animationHandles.Add(enterSheet.Enter(playAnimation, exitSheet));

            foreach (var handle in animationHandles)
            {
                while (!handle.IsTerminated)
                    yield return null;
            }

            // End Transition
            ActiveSheetId = sheetId;
            IsInTransition = false;

            // Postprocess
            if (exitSheet != null) exitSheet.AfterExit(enterSheet);

            enterSheet.AfterEnter(exitSheet);

            foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.AfterShow(enterSheet, exitSheet);

            if (!DefaultNavigatorSetting.EnableInteractionInTransition)
            {
                if (DefaultNavigatorSetting.ControlInteractionAllContainer)
                {
                    // If there's a container in transition, it should restore Interactive to true when the transition is finished.
                    // So, do nothing here if there's a transitioning container.
                    if (PageContainer.Instances.All(x => !x.IsInTransition) && PopupContainer.Instances.All(x => !x.IsInTransition) &&
                        Instances.All(x => !x.IsInTransition))
                    {
                        foreach (var pageContainer in PageContainer.Instances) pageContainer.Interactable = true;
                        foreach (var popupContainer in PopupContainer.Instances) popupContainer.Interactable = true;
                        foreach (var sheetContainer in Instances) sheetContainer.Interactable = true;
                    }
                }
                else
                {
                    Interactable = true;
                }
            }
        }

        private IEnumerator HideRoutine(bool playAnimation)
        {
            if (IsInTransition)
                throw new InvalidOperationException("Cannot transition because the screen is already in transition.");

            if (ActiveSheetId == null)
                throw new InvalidOperationException("Cannot transition because there is no active sheets.");

            IsInTransition = true;

            if (!DefaultNavigatorSetting.EnableInteractionInTransition)
            {
                if (DefaultNavigatorSetting.ControlInteractionAllContainer)
                {
                    foreach (var pageContainer in PageContainer.Instances) pageContainer.Interactable = false;
                    foreach (var popupContainer in PopupContainer.Instances) popupContainer.Interactable = false;
                    foreach (var sheetContainer in Instances) sheetContainer.Interactable = false;
                }
                else
                {
                    Interactable = false;
                }
            }

            var exitSheet = _sheets[ActiveSheetId];

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.BeforeHide(exitSheet);

            var preprocessHandle = exitSheet.BeforeExit(null);
            while (!preprocessHandle.IsTerminated) yield return preprocessHandle;

            // Play Animation
            var animationHandle = exitSheet.Exit(playAnimation, null);
            while (!animationHandle.IsTerminated) yield return null;

            // End Transition
            ActiveSheetId = null;
            IsInTransition = false;

            // Postprocess
            exitSheet.AfterExit(null);
            foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.AfterHide(exitSheet);

            if (!DefaultNavigatorSetting.EnableInteractionInTransition)
            {
                if (DefaultNavigatorSetting.ControlInteractionAllContainer)
                {
                    // If there's a container in transition, it should restore Interactive to true when the transition is finished.
                    // So, do nothing here if there's a transitioning container.
                    if (PageContainer.Instances.All(x => !x.IsInTransition) && PopupContainer.Instances.All(x => !x.IsInTransition) &&
                        Instances.All(x => !x.IsInTransition))
                    {
                        foreach (var pageContainer in PageContainer.Instances) pageContainer.Interactable = true;
                        foreach (var popupContainer in PopupContainer.Instances) popupContainer.Interactable = true;
                        foreach (var sheetContainer in Instances) sheetContainer.Interactable = true;
                    }
                }
                else
                {
                    Interactable = true;
                }
            }
        }

        /// <summary>
        ///     Destroy and release all sheets.
        /// </summary>
        public void UnregisterAll()
        {
            foreach (var sheet in _sheets.Values)
            {
                if (DefaultNavigatorSetting.CallCleanupWhenDestroy) sheet.BeforeReleaseAndForget();
                Destroy(sheet.gameObject);
            }

            foreach (var assetLoadHandle in _assetLoadHandles.Values) AssetLoader.Release(assetLoadHandle);

            _assetLoadHandles.Clear();
        }
    }
}