using System.Collections.Generic;
using Assets.SimpleLocalization;
using UnityEngine;

namespace Mobiray.Common
{
    public class SceneStarter : MonoBehaviour
    {
        public List<ScriptableObject> Tools;

        public bool IsForceEnglish;

        private void Awake()
        {
            Application.targetFrameRate = 60;

            foreach (var tool in Tools)
            {
                if (tool is INeedInitialization)
                {
                    ((INeedInitialization) tool).Initialize();
                }

                ToolBox.Add(tool);
            }

            LocalizationManager.Read();

            switch (Application.systemLanguage)
            {
                case SystemLanguage.Russian:
                    LocalizationManager.Language = "Russian";
                    break;
                default:
                    LocalizationManager.Language = "English";
                    break;
            }

            if (IsForceEnglish)
            {
                LocalizationManager.Language = "English";
            }
        }
    }
}