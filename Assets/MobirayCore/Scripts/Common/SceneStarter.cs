using System;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using UnityEngine;

namespace Mobiray.Common
{
    public class SceneStarter : MonoBehaviour
    {

        public ConfigBundle CommonTools;
        
        public List<ScriptableObject> Tools;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            
            var tools = new List<ScriptableObject>();

            if (CommonTools != null)
            {
                tools.AddRange(CommonTools.Tools);
            }
            
            tools.AddRange(Tools);

            foreach (var tool in tools)
            {
                try
                {
                    var instance = Instantiate(tool);
                    if (instance is INeedInitialization initialization)
                    {
                        initialization.Initialize();
                    }

                    ToolBox.Add(instance);
                    
                    // Debug.Log($"ToolBox Added {instance.name}");
                    
                } catch (Exception e)
                {
                    Debug.LogException(e);
                }
                
            }
        }

        private void OnDestroy()
        {
            foreach (var pair in ToolBox.Instance.Box)
            {
                if (pair.Value is INeedDestroy instance)
                {
                    instance.OnDestroy();
                }
            }

            ToolBox.Clear();
        }
    }
}