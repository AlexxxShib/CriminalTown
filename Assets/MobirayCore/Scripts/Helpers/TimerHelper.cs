using System;
using System.Collections.Generic;
using Mobiray.Common;
using UnityEngine;

namespace Mobiray.Helpers
{

    public class TimerState
    {
        public float Time;
        public Action OnTimerEnd;
    }
    
    public class TimerHelper : MonoBehaviour
    {

        private List<TimerState> timerStates = new List<TimerState>();
        private List<TimerState> completeTimers = new List<TimerState>();
        
        private MobirayLogger logger = new MobirayLogger("TimerHelper");

        public TimerState StartTimer(float time, Action endAction)
        {
            var timer = new TimerState
            {
                Time = time, OnTimerEnd = endAction
            };
            
            timerStates.Add(timer);
            return timer;
        }

        private void OnDisable()
        {
            timerStates.Clear();
            completeTimers.Clear();
        }

        private void Update()
        {
            foreach (var timerState in timerStates)
            {
                timerState.Time -= Time.deltaTime;
                
                if (timerState.Time <= 0)
                {
                    completeTimers.Add(timerState);
                }
            }

            foreach (var completeTimer in completeTimers)
            {
                try
                {
                    completeTimer.OnTimerEnd?.Invoke();
                    
                } catch (Exception e)
                {
                    logger.LogException(e);
                    
                } finally
                {
                    timerStates.Remove(completeTimer);
                }
            }
            
            completeTimers.Clear();
        }
    }
}