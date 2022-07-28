using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace MobirayCore.Helpers
{
    public interface ILoadable
    {
        public void LoadInUI();
        public List<HashMap<object, object>.MapEntry> GetPairs();
    }
    
    [Serializable]
    public class HashMap<TK, TV> : Dictionary<TK, TV>, ISerializationCallbackReceiver, ILoadable
    {
        [Serializable]
        public struct MapEntry
        {
            public TK key;
            public TV value;

            public MapEntry(TK key, TV value)
            {
                this.key = key;
                this.value = value;
            }

            public override string ToString()
            {
                return $"{key}:{value}";
            } 
        }
        
        public HashMap() : base() {}
        public HashMap(SerializationInfo info, StreamingContext context) : base(info, context) {}

        public List<MapEntry> entries;
    
        public void LoadInUI()
        {
            if (HasSameKey()) return;
            
            if (entries != null)
            {
                entries.Clear();
            }
            else
            {
                entries = new List<MapEntry>();
            }

            foreach (var kvp in this)
            {
                entries.Add(new MapEntry(kvp.Key, kvp.Value));
            }
        }

        public List<HashMap<object, object>.MapEntry> GetPairs()
        {
            return Keys.Select(e => new HashMap<object, object>.MapEntry(e, this[e])).ToList();
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            if (HasSameKey()) return;

            Clear();
            
            foreach (var entry in entries)
            {
                if (entry.key != null)
                {
                    Add(entry.key, entry.value);
                }
            }
        }

        private bool HasSameKey()
        {
            if (entries == null)
            {
                return false;
            }
            
            var counts = new Dictionary<TK, int>();
            
            foreach (var entry in entries)
            {
                if(entry.key == null) continue;
                
                if (!counts.ContainsKey(entry.key))
                {
                    counts.Add(entry.key, 0);   
                }
                counts[entry.key]++;

                if (counts[entry.key] > 1)
                {
                    Debug.LogWarning("HashMap has some entries with same keys, it won't be serialized or deserialized before you remove doubling keys");
                    return true;
                }
            }
            
            return false;
        }

        public override string ToString()
        {
            return Keys.Count.ToString();
        }
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(HashMap<,>), true)]
    public class HashMapDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
            
            var t = property.GetValue<ILoadable>();
            t.LoadInUI();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    }
    
    public static class SerializedPropertyExtensions {
        public static T GetValue<T> (this SerializedProperty property) where T : class {
            object obj = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace (".Array.data", "");
            string[] fieldStructure = path.Split ('.');
            Regex rgx = new Regex (@"\[\d+\]");
            for (int i = 0; i < fieldStructure.Length; i++) {
                if (fieldStructure[i].Contains ("[")) {
                    int index = System.Convert.ToInt32 (new string (fieldStructure[i].Where (c => char.IsDigit (c)).ToArray ()));
                    obj = GetFieldValueWithIndex (rgx.Replace (fieldStructure[i], ""), obj, index);
                } else {
                    obj = GetFieldValue (fieldStructure[i], obj);
                }
            }
            return (T) obj;
        }

        public static bool SetValue<T> (this SerializedProperty property, T value) where T : class {
            object obj = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace (".Array.data", "");
            string[] fieldStructure = path.Split ('.');
            Regex rgx = new Regex (@"\[\d+\]");
            for (int i = 0; i < fieldStructure.Length - 1; i++) {
                if (fieldStructure[i].Contains ("[")) {
                    int index = System.Convert.ToInt32 (new string (fieldStructure[i].Where (c => char.IsDigit (c)).ToArray ()));
                    obj = GetFieldValueWithIndex (rgx.Replace (fieldStructure[i], ""), obj, index);
                } else {
                    obj = GetFieldValue (fieldStructure[i], obj);
                }
            }

            string fieldName = fieldStructure.Last ();
            if (fieldName.Contains ("[")) {
                int index = System.Convert.ToInt32 (new string (fieldName.Where (c => char.IsDigit (c)).ToArray ()));
                return SetFieldValueWithIndex (rgx.Replace (fieldName, ""), obj, index, value);
            } else {
                Debug.Log(value);
                return SetFieldValue (fieldName, obj, value);
            }
        }

        private static object GetFieldValue (string fieldName, object obj, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) {
            FieldInfo field = obj.GetType ().GetField (fieldName, bindings);
            if (field != null) {
                return field.GetValue (obj);
            }
            return default (object);
        }

        private static object GetFieldValueWithIndex (string fieldName, object obj, int index, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) {
            FieldInfo field = obj.GetType ().GetField (fieldName, bindings);
            if (field != null) {
                object list = field.GetValue (obj);
                if (list.GetType ().IsArray) {
                    return ((object[]) list)[index];
                } else if (list is IEnumerable) {
                    return ((IList) list)[index];
                }
            }
            return default (object);
        }

        public static bool SetFieldValue (string fieldName, object obj, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) {
            FieldInfo field = obj.GetType ().GetField (fieldName, bindings);
            if (field != null) {
                field.SetValue (obj, value);
                return true;
            }
            return false;
        }

        public static bool SetFieldValueWithIndex (string fieldName, object obj, int index, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) {
            FieldInfo field = obj.GetType ().GetField (fieldName, bindings);
            if (field != null) {
                object list = field.GetValue (obj);
                if (list.GetType ().IsArray) {
                    ((object[]) list)[index] = value;
                    return true;
                } else if (value is IEnumerable) {
                    ((IList) list)[index] = value;
                    return true;
                }
            }
            return false;
        }
    }
#endif
}
