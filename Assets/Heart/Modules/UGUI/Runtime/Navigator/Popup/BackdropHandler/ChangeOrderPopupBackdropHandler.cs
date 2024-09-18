﻿using Pancake.Common;
using UnityEngine;

namespace Pancake.UI
{
    /// <summary>
    ///     Implementation of <see cref="IPopupBackdropHandler" /> that changes the drawing order of a single backdrop and
    ///     reuses it
    /// </summary>
    internal sealed class ChangeOrderPopupBackdropHandler : IPopupBackdropHandler
    {
        public enum ChangeTiming
        {
            BeforeAnimation,
            AfterAnimation
        }

        private readonly ChangeTiming _changeTiming;
        private readonly PopupBackdrop _prefab;
        private PopupBackdrop _instance;

        public ChangeOrderPopupBackdropHandler(PopupBackdrop prefab, ChangeTiming changeTiming)
        {
            _prefab = prefab;
            _changeTiming = changeTiming;
        }

        public AsyncProcessHandle BeforePopupEnter(Popup popup, int popupIndex, bool playAnimation)
        {
            var parent = (RectTransform) popup.transform.parent;

            // If it is the first popup, generate a new backdrop
            if (popupIndex == 0)
            {
                var backdrop = Object.Instantiate(_prefab);
                backdrop.Setup(parent, popupIndex);
                backdrop.transform.SetSiblingIndex(0);
                _instance = backdrop;
                return backdrop.Enter(playAnimation);
            }

            // For the second and subsequent popups, change the drawing order of the backdrop
            if (_changeTiming == ChangeTiming.BeforeAnimation) _instance.transform.SetSiblingIndex(popupIndex);

            return AsyncProcessHandle.Completed();
        }

        public void AfterPopupEnter(Popup popup, int popupIndex, bool playAnimation)
        {
            // For the second and subsequent popups, change the drawing order of the backdrop
            if (_changeTiming == ChangeTiming.AfterAnimation) _instance.transform.SetSiblingIndex(popupIndex);
        }

        public AsyncProcessHandle BeforePopupExit(Popup popup, int popupIndex, bool playAnimation)
        {
            // If it is the first popup, play the backdrop animation
            if (popupIndex == 0) return _instance.Exit(playAnimation);

            // For the second and subsequent popups, change the drawing order of the backdrop
            if (_changeTiming == ChangeTiming.BeforeAnimation) _instance.transform.SetSiblingIndex(popupIndex - 1);

            return AsyncProcessHandle.Completed();
        }

        public void AfterPopupExit(Popup popup, int popupIndex, bool playAnimation)
        {
            // If it is the first popup, remove the backdrop
            if (popupIndex == 0)
            {
                Object.Destroy(_instance.gameObject);
                return;
            }

            // For the second and subsequent popups, change the drawing order of the backdrop
            if (_changeTiming == ChangeTiming.AfterAnimation) _instance.transform.SetSiblingIndex(popupIndex - 1);
        }
    }
}