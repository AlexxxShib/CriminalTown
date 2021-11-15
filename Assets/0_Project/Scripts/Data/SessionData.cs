using System;
using System.Collections;
using UnityEngine;

namespace Template.Data
{

    [Serializable]
    public class SessionData
    {

        public int CurrentLevel;
        public int LevelLoop;
        public int LevelCount;

        public bool LevelComplete;
        public DateTime TimeLevelStart;
        public int LevelTime
        {
            get
            {
                var time = (int) (DateTime.Now - TimeLevelStart).TotalSeconds;
                
                Debug.Log($"SessionData LevelTime : {time}");

                return time;
            }
        }

        public SessionData() { }

        public SessionData(SessionData sessionData)
        {
            CurrentLevel = sessionData.CurrentLevel;
            LevelLoop = sessionData.LevelLoop;
            LevelCount = sessionData.LevelCount;

            LevelComplete = sessionData.LevelComplete;
            TimeLevelStart = sessionData.TimeLevelStart;
        }
    }

}