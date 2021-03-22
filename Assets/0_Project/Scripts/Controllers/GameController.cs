using System;
using Mobiray.Common;
using Mobiray.Controllers;
using Mobiray.Numbers;
using Template.Configs;
using Template.Data;
using UnityEngine;

namespace Template.Controllers
{
    public class GameController : SignalReceiver
    {
        public DataGameState GameState;

        private SessionData sessionData;
        
        private GameSettings settings;
        private ConfigMain configs;
        
        private MobirayLogger logger = new MobirayLogger("GameController");

        private void Awake()
        {
            configs = ToolBox.Get<ConfigMain>();
            settings = ToolBox.Get<GameSettings>();

            Application.targetFrameRate = settings.TargetFrameRate;

            ToolBox.Add(this);
            ToolBox.Add(new NumericalFormatter());
            ToolBox.Add(sessionData = GetComponent<SessionData>());

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
            GameState = ToolSaver.Instance.Load<DataGameState>(settings.PathSaves);

            if (GameState == null)
            {
                GameState = settings.IsDebugGameState ? 
                    configs.DebugGameState : configs.InitGameState;
            }

            ToolBox.Add(GameState);
        }

        private void SaveGameState()
        {
            GameState.AppClosedDateTime = DateTime.Now;
//            Debug.Log("quit time " + GameState.AppClosedDateTime);

            if (settings.IsSaveGame)
            {
                ToolSaver.Instance.Save(settings.PathSaves, GameState);
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