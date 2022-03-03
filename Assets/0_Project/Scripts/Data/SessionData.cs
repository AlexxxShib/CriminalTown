using System;
using System.Collections;
using UnityEngine;

namespace Template.Data
{

    [Serializable]
    public class SessionData
    {

        public int currentLevel;
        public int levelLoop;
        public int levelCount;

        public bool levelComplete;
        public DateTime timeLevelStart;
        
        public int LevelTime
        {
            get
            {
                var time = (int) (DateTime.Now - timeLevelStart).TotalSeconds;
                
                Debug.Log($"SessionData LevelTime : {time}");

                return time;
            }
        }

        public SessionData() { }

        public SessionData(SessionData sessionData)
        {
            currentLevel = sessionData.currentLevel;
            levelLoop = sessionData.levelLoop;
            levelCount = sessionData.levelCount;

            levelComplete = sessionData.levelComplete;
            timeLevelStart = sessionData.timeLevelStart;
        }
    }

}