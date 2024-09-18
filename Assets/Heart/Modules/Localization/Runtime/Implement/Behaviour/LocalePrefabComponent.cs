﻿using UnityEngine;

namespace Pancake.Localization
{
    [EditorIcon("icon_default")]
    public class LocalePrefabComponent : LocaleComponent
    {
        public LocalePrefab prefab;
        private GameObject _instance;

        protected override bool TryUpdateComponentLocalization(bool isOnValidate)
        {
#if UNITY_EDITOR
            if (Application.isPlaying && !isOnValidate)
            {
#endif
                if (prefab)
                {
                    if (_instance) Destroy(_instance);
                    _instance = Instantiate(prefab.Value, transform);
                    return true;
                }

#if UNITY_EDITOR
            }
#endif

            return false;
        }
    }
}