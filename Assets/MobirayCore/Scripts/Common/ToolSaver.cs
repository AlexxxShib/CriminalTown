using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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

        public void SaveToJson<T>(string path, T data)
        {
            Delete<T>(path);
            
            using (var stream = File.Open(JsonPathFor(path, typeof(T)), FileMode.OpenOrCreate))
            {
                var stringData = JsonUtility.ToJson(data);
                byte[] bytes = Encoding.UTF8.GetBytes(stringData);
                stream.Write(bytes, 0, bytes.Length);
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
                data = default;
            }

            return data;
        }

        public T LoadJson<T>(string path) where T : new()
        {
            T data;
            if (IsUseCash && cash.ContainsKey(typeof(T)))
            {
                data = (T) cash[typeof(T)];
                return data;
            }

            try
            {
                using (var stream = File.OpenRead(JsonPathFor(path, typeof(T))))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var stringData = reader.ReadToEnd();
                        data = JsonUtility.FromJson<T>(stringData);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                data = default;
            }

            return data;
        }
        
        public void Delete<T>(string path)
        {
            File.Delete(PathFor(path, typeof(T)));
            File.Delete(JsonPathFor(path, typeof(T)));
        }

        public static string PathFor(string path, Type t)
        {
            return Application.persistentDataPath + path + "_" + t.Name;
        }
        
        public static string JsonPathFor(string path, Type t)
        {
            return Application.persistentDataPath + path + "_" + t.Name + ".json";
        }
    }
}