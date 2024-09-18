﻿using System;
using UnityEngine;

namespace Pancake.Localization
{
    [CreateAssetMenu(menuName = "Pancake/Localization/GameObject", fileName = "gameobject_localizevalue", order = 2)]
    [EditorIcon("so_yellow_gameobject")]
    public class LocalePrefab : LocaleVariable<GameObject>
    {
        [Serializable]
        private class PrefabLocaleItem : LocaleItem<GameObject>
        {
        };

        [SerializeField] private PrefabLocaleItem[] items = new PrefabLocaleItem[1];

        // ReSharper disable once CoVariantArrayConversion
        public override LocaleItemBase[] LocaleItems => items;
    }
}