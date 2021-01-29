using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mobiray.Common
{
    public class ToolBox : Singleton<ToolBox>
    {
        public static ToolSignals Signals => Instance.signals;

        private readonly Dictionary<Type, object> box = new Dictionary<Type, object>();
        private readonly ToolSignals signals = new ToolSignals();

        public Dictionary<Type, object> Box => box;

        public static void Add<T>(T instance)
        {
            try
            {
                Instance.box.Add(instance.GetType(), instance);
            }
            catch (ArgumentException exception)
            {
                Debug.Log($"Toolbox ADD PROBLEM {typeof(T)}");
                Debug.LogWarning(exception);
            }
        }

        public static bool Remove<T>()
        {
            try
            {
                return Instance.box.Remove(typeof(T));
            }
            catch (Exception exception)
            {
                Debug.LogWarning(exception);
            }

            return false;
        }

        public static bool Has<T>()
        {
            return Instance.box.ContainsKey(typeof(T));
        }

        public static T Get<T>()
        {

            object instance;
            if (!Instance.box.TryGetValue(typeof(T), out instance))
            {
                Debug.LogError("ToolBox : not found " + typeof(T));
            }

            return (T) instance;
        }

        public static void Clear()
        {
            Instance.box.Clear();
            Debug.Log($"Toolbox Clear");
        }
        
        [ContextMenu("Clear Player Prefs")]
        void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Perform operation");
        }
    }
}