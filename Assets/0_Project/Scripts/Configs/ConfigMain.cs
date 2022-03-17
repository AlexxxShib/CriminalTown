using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using CriminalTown.Data;
using Mobiray.Common;

namespace CriminalTown.Configs
{
    [CreateAssetMenu(fileName = "ConfigMain", menuName = "Configs/ConfigMain")]
    public class ConfigMain : ScriptableObject, INeedInitialization
    {
        [Header("Game States")]
        public DataGameState initGameState;
        public DataGameState debugGameState;

        [Header("Balance")]
        public int islandBasePrice;
        public float islandPriceFactor;

        [Header("Other")]
        public float connectionTime = 1;
        public float emitResourceTime = 0.8f;
        public int emitMoneyMin = 10;
        public int emitMoneyMax = 800;

        /*[Space] 
        public long BasePriceParameter1;
        public float PriceStepParameter1;
        
        [Space]
        public float BaseParameter1Val;
        public float StepParameter1Val;*/
        
        public void Initialize()
        {
            //READ CONFIGS
            // ReadConfigMoneyOnLevel();
            // ReadConfigMoneyToLevelUp();
        }

        public int GetIslandPrice(int level)
        {
            return (int) (islandBasePrice * (1 + islandPriceFactor * level));
        }
        
        /*public int GetParameter1Val(int level)
        {
            //BaseParameter1Val
            //StepParameter1Val
            return level + 1;
        }
        public long GetParameter1Price(int level)
        {
            return (long) (BasePriceParameter1 * (1 + PriceStepParameter1 * level));
        }*/
    }
}