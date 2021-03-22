using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using Template.Data;
using Mobiray.Common;

namespace Template.Configs
{
    [CreateAssetMenu(fileName = "ConfigMain", menuName = "Configs/ConfigMain")]
    public class ConfigMain : ScriptableObject, INeedInitialization
    {
        public DataGameState InitGameState;
        public DataGameState DebugGameState;

        [Space] 
        public long BasePriceParameter1;
        public float PriceStepParameter1;
        
        [Space]
        public float BaseParameter1Val;
        public float StepParameter1Val;
        
        public void Initialize()
        {
            //READ CONFIGS
            // ReadConfigMoneyOnLevel();
            // ReadConfigMoneyToLevelUp();
        }
        public int GetParameter1Val(int level)
        {
            //BaseParameter1Val
            //StepParameter1Val
            return level + 1;
        }

        public long GetParameter1Price(int level)
        {
            return (long) (BasePriceParameter1 * (1 + PriceStepParameter1 * level));
        }
    }
}