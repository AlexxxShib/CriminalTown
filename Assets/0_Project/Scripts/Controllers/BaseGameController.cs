using System;
using Mobiray.Common;
using Mobiray.Controllers;
using Mobiray.Helpers;
using Mobiray.Numbers;
using Template.Configs;
using Template.Data;
using UnityEngine;

namespace Template.Controllers
{
    public class BaseGameController : SignalReceiver
    {
        public DataGameState gameState;
        public SessionData sessionData;
        
        protected GameSettings _settings;
        protected ConfigMain _configs;

        protected virtual void Awake()
        {
            Time.timeScale = 1;
            
            _configs = ToolBox.Get<ConfigMain>();
            _settings = ToolBox.Get<GameSettings>();

            Application.targetFrameRate = _settings.targetFrameRate;

            ToolBox.Add(this);
            ToolBox.Add(new NumericalFormatter());
            ToolBox.Add(sessionData = new SessionData());
            
            var timerHelper = new GameObject("TimerHelper");
            timerHelper.transform.parent = transform;
            ToolBox.Add(timerHelper.AddComponent<TimerHelper>());

            InitGameState();

            //DEBUG DEBUG DEBUG
            // GameState.CurrentLevel = 2;

            // LoadLevel();
        }

        private int TimeFromLastSessionInSeconds()
        {
            var dif = (DateTime.Now - gameState.appClosedDateTime).TotalSeconds;

            if (dif < 60) dif = 0;
            return (int) dif;
        }

        #region LOADING AND SAVING

        private void InitGameState()
        {
            gameState = ToolSaver.Instance.Load<DataGameState>(_settings.pathSaves);

            if (gameState == null)
            {
                gameState = _settings.isDebugGameState ? 
                    _configs.debugGameState : _configs.initGameState;
            }

            ToolBox.Add(gameState);
        }

        public void SaveGameState()
        {
            gameState.appClosedDateTime = DateTime.Now;
//            Debug.Log("quit time " + GameState.AppClosedDateTime);

            if (_settings.isSaveGame)
            {
                ToolSaver.Instance.Save(_settings.pathSaves, gameState);
            }
        }

#if UNITY_ANDROID || UNITY_IOS
        private void OnApplicationPause(bool pauseStatus)
        {
            Debug.Log("OnApplicationPause " + pauseStatus);

            if (pauseStatus) SaveGameState();
        }

#endif

#if UNITY_EDITOR
        private void OnApplicationQuit()
        {
            SaveGameState();
        }
#endif

        #endregion
    }
}