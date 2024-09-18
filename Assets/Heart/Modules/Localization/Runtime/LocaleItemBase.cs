using System;
using UnityEngine;

namespace Pancake.Localization
{
    [Serializable]
    public abstract class LocaleItemBase
    {
        [SerializeField] private Language language = Language.English;

        public Language Language { get => language; set => language = value; }

        public abstract object ObjectValue { get; set; }
    }
}