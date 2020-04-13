using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Mobiray.Common
{
    public class ToolSaver : Singleton<ToolSaver>
    {
        public bool IsUseCash = false;

        private Dictionary<Type, object> cash = new Dictionary<Type, object>();

        public void Save<T>(string path, T data)
        {
            using (var stream = File.Open(PathFor(path, typeof(T)), FileMode.OpenOrCreate))
            {
                new BinaryFormatter().Serialize(stream, data);
            }

            if (IsUseCash)
            {
                cash[typeof(T)] = data;
            }
        }

        public T Load<T>(string path) where T : new()
        {
            T data;
            if (IsUseCash && cash.ContainsKey(typeof(T)))
            {
                data = (T) cash[typeof(T)];
                return data;
            }

            try
            {
                using (var stream = File.OpenRead(PathFor(path, typeof(T))))
                {
                    data = (T) new BinaryFormatter().Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                data = default(T);
            }

            return data;
        }

        public static string PathFor(string path, Type t)
        {
            return Application.persistentDataPath + path + "_" + t.Name;
        }
    }
}