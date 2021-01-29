using System.Collections;
using UnityEngine;

namespace Template.Data
{

    public class SessionData : MonoBehaviour
    {

        public int TotalObjects;
        public int CollectedObjects;
        public long Money;
        
        public float NormalizedTimer { get; private set; }

        public bool LevelCompleted => CollectedObjects >= TotalObjects;

        public void CheckLevelComplete()
        {
            if (CollectedObjects >= TotalObjects)
            {
                NormalizedTimer = 1;
            }
        }

        public void ResetState()
        {
            TotalObjects = 0;
            CollectedObjects = 0;
            NormalizedTimer = 0;
            Money = 0;
        }

        public IEnumerator StartLevelTimer(float duration)
        {
            NormalizedTimer = 0;
            while (NormalizedTimer <= 1)
            {
                NormalizedTimer += Time.deltaTime / duration;
                yield return null;
            }
            
            // ToolBox.Signals.Send<SignalTimerEnd>();
        }
    }

}