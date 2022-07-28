using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobirayCore.Scripts.Common
{
    public class ToolBoxInjector : MonoBehaviour
    {
        private void Awake()
        {
            Inject(true);
        }

        private void Start()
        {
            Inject(false);
        }

        private static void Inject(bool onAwake)
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();

            void Init(IEnumerable<GameObject> gos)
            {
                foreach (var go in gos)
                {
                    var gos2 = go.transform.GetChildren().Select(t => t.gameObject);

                    Inject(go, true, onAwake);
                    
                    Init(gos2);
                }
            }
            
            Init(roots);
        }

        public static void Inject(GameObject gameObject, bool checkAwake = false, bool onAwake = false)
        {
            var components = gameObject.GetComponents<Component>();
            foreach (var comp in components)
            {
                var fields = comp.GetType().GetFields(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

                foreach (var field in fields)
                {
                    var a = field.GetCustomAttribute<ToolBoxGetAttribute>();
                    if (a != null && (!checkAwake || a.onAwake == onAwake))
                    {
                        field.SetValue(comp, ToolBox.Get(field.FieldType));
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ToolBoxGetAttribute : Attribute
    {
        public bool onAwake;
    
        public ToolBoxGetAttribute(bool onAwake = false)
        {
            this.onAwake = onAwake;
        }
    }
}
