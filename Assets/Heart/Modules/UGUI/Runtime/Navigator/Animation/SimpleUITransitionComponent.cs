﻿using Alchemy.Inspector;
using LitMotion;
using Pancake.Common;
using UnityEngine;

namespace Pancake.UI
{
    [EditorIcon("icon_setting")]
    public class SimpleUITransitionComponent : UITransitionComponent
    {
        [SerializeField] private float delay;
        [SerializeField] private float duration;
        [HorizontalLine] [SerializeField] private Ease ease = Ease.Linear;
        [SerializeField] private EAlignment beforeAligment = EAlignment.Center;
        [SerializeField] private Vector3 beforeScale = Vector3.one;
        [HorizontalLine] [SerializeField] private float beforeAlpha = 1f;
        [SerializeField] private EAlignment afterAligment = EAlignment.Center;
        [SerializeField] private Vector3 afterScale = Vector3.one;
        [SerializeField] private float afterAlpha = 1f;

        private Vector3 _afterPosition;
        private Vector3 _beforePosition;
        private CanvasGroup _canvasGroup;

        public override float Duration => delay + duration;

        public override void Setup()
        {
            _beforePosition = beforeAligment.ToPosition(RectTransform);
            _afterPosition = afterAligment.ToPosition(RectTransform);
            if (!gameObject.TryGetComponent(out _canvasGroup)) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public override void SetTime(float time)
        {
            time = 0f.Max(time - delay);
            float progress = duration <= 0f ? 1f : (time / duration).Clamp01();
            var e = ease;
            if (ease == Ease.CustomAnimationCurve) e = Ease.OutQuad;
            progress = EaseUtility.Evaluate(progress, e);
            var position = Vector3.Lerp(_beforePosition, _afterPosition, progress);
            var scale = Vector3.Lerp(beforeScale, afterScale, progress);
            float alpha = Math.Lerp(beforeAlpha, afterAlpha, progress);

            RectTransform.anchoredPosition = position;
            RectTransform.localScale = scale;
            _canvasGroup.alpha = alpha;
        }
    }
}