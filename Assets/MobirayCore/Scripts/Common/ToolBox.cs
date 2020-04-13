using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mobiray.Common
{
    public class ToolBox : MonoSingleton<ToolBox>
    {
        private static Dictionary<int, object> box = new Dictionary<int, object>();

        private static ToolSignals signals = new ToolSignals();
        public static ToolSignals Signals => signals;

        public static void Add<T>(T instance)
        {
            try
            {
                box.Add(instance.GetType().GetHashCode(), instance);
            }
            catch (ArgumentException exception)
            {
                Debug.LogWarning(exception);
            }
        }

        public static bool Remove<T>()
        {
            try
            {
                return box.Remove(typeof(T).GetHashCode());
            }
            catch (Exception exception)
            {
                Debug.LogWarning(exception);
            }

            return false;
        }

        public static T Get<T>()
        {
            int key = typeof(T).GetHashCode();

            object instance;
            if (!box.TryGetValue(key, out instance))
            {
                Debug.LogError("ToolBox : not found " + typeof(T));
            }

            return (T) instance;
        }

        public static void Clear()
        {
            box.Clear();
        }
    }
}