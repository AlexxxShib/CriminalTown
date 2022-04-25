using System;
using GameAnalyticsSDK;
using UnityEngine;

namespace CriminalTown.Controllers
{
    public class GAStart : MonoBehaviour
    {
        public static bool Initialized { get; private set; }
        
        private void Awake()
        {
            if (!Initialized)
            {
                GameAnalytics.Initialize();

                Initialized = true;
            }
        }
    }
}