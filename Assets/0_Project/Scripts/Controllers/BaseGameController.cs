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
        protected DataGameState GameState;

        public SessionData SessionData;
        
        protected GameSettings Settings;
        protected ConfigMain Configs;

        protected virtual void Awake()
        {
            Time.timeScale = 1;
            
            Configs = ToolBox.Get<ConfigMain>();
            Settings = ToolBox.Get<GameSettings>();

            Application.targetFrameRate = Settings.TargetFrameRate;

            ToolBox.Add(this);
            ToolBox.Add(new NumericalFormatter());
            ToolBox.Add(SessionData = GetComponent<SessionData>());
            
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
            var dif = (DateTime.Now - GameState.AppClosedDateTime).TotalSeconds;

            if (dif < 60) dif = 0;
            return (int) dif;
        }

        #region LOADING AND SAVING

        private void InitGameState()
        {
            GameState = ToolSaver.Instance.Load<DataGameState>(Settings.PathSaves);

            if (GameState == null)
            {
                GameState = Settings.IsDebugGameState ? 
                    Configs.DebugGameState : Configs.InitGameState;
            }

            ToolBox.Add(GameState);
        }

        private void SaveGameState()
        {
            GameState.AppClosedDateTime = DateTime.Now;
//            Debug.Log("quit time " + GameState.AppClosedDateTime);

            if (Settings.IsSaveGame)
            {
                ToolSaver.Instance.Save(Settings.PathSaves, GameState);
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