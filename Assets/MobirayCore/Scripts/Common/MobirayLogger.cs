using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Mobiray.Common
{
    public class MobirayLogger
    {
        
        public bool Enabled = true;
        
        private string mainTag;
        
        private bool showCurrentScene;
        private string currentSceneName;

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
            if (!Enabled) return;

            Debug.Log($"{GetSceneName()}[{mainTag}] : {message}");
        }

        public void LogDebug(object message, Object context)
        {
            if (!Enabled) return;

            Debug.Log($"{GetSceneName()}[{mainTag}] : {message}", context);
        }

        public void LogError(object message)
        {
            if (!Enabled) return;

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