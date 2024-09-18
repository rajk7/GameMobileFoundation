namespace Pancake
{
    using UnityEngine;
    using System;

    public abstract class ScriptableSettings<T> : ScriptableObject where T : ScriptableObject
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance != null) return instance;

                instance = Resources.Load<T>(typeof(T).Name);
                if (instance == null) throw new Exception($"Scriptable setting for {typeof(T)} must be create before run!. Please find and setup it in wizard!");
                return instance;
            }
        }

        public static bool IsExist() => Resources.Load<T>(typeof(T).Name) != null;
    }
}