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
        // IReceive<SignalStartGame>,
        // IReceive<SignalTimerEnd>,
    {
        public DataGameState GameState;
        // public Joystick Joystick;

        private SessionData sessionData;
        
        private GameSettings gameSettings;
        private ConfigMain configMain;
        // private LevelConfig levelConfig;
        
        // private SceneLoader sceneLoader = new SceneLoader();

        private void Awake()
        {
            configMain = ToolBox.Get<ConfigMain>();
            gameSettings = ToolBox.Get<GameSettings>();

            Application.targetFrameRate = gameSettings.TargetFrameRate;

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
            GameState = ToolSaver.Instance.Load<DataGameState>(gameSettings.PathSaves);

            if (GameState == null)
            {
                GameState = configMain.InitState();

                // if (gameSettings.IsCustomSave && gameSettings.CustomSave != null)
                // {
                    // gameSettings.CustomSave.InitCustomSave(GameState);
                // }
            }

            ToolBox.Add(GameState);
        }

        private void SaveGameState()
        {
            GameState.AppClosedDateTime = DateTime.Now;
//            Debug.Log("quit time " + GameState.AppClosedDateTime);

            if (gameSettings.IsSaveGame)
            {
                ToolSaver.Instance.Save(gameSettings.PathSaves, GameState);
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