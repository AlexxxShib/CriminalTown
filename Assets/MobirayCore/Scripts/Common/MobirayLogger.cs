using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Mobiray.Common
{
    
    [Serializable]
    public class MobirayLogger
    {
        public bool showLog = true;
        public string mainTag;
     
        [Space]
        public bool showCurrentScene;
        public string currentSceneName;

        public MobirayLogger(string tag, bool showCurrentScene = true)
        {
            mainTag = tag;

            this.showCurrentScene = showCurrentScene;
            currentSceneName = "unknown";
            
            try
            {
                currentSceneName = SceneManager.GetActiveScene().name;

            } catch (Exception _) { }
        }

        public void SetTag(GameObject gameObject) { mainTag = gameObject.name; }

        public void SetTag(string tag) { mainTag = tag; }

        private string GetSceneName()
        {
            if (!showCurrentScene) return string.Empty;
            
            try
            {
                currentSceneName = SceneManager.GetActiveScene().name;

            } catch (Exception _) { }

            return $"[{currentSceneName}]-";
        }

        public void LogDebug(object message)
        {
            if (!showLog) return;

            Debug.Log($"{GetSceneName()}[{mainTag}] : {message}");
        }

        public void LogDebug(object message, Object context)
        {
            if (!showLog) return;

            Debug.Log($"{GetSceneName()}[{mainTag}] : {message}", context);
        }

        public void LogError(object message)
        {
            if (!showLog) return;

            Debug.LogError($"{GetSceneName()}[{mainTag}] : {message}");
        }

        public void LogException(Exception exception)
        {
            // if (!Enabled) return;

            Debug.LogError($"{GetSceneName()}[{mainTag}] : {exception.Message}");
            Debug.LogException(exception);
        }
    }
}