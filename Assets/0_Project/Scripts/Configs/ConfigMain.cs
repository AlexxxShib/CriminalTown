using System.Collections.Generic;
using UnityEngine;
using CriminalTown.Data;
using CriminalTown.Entities;
using Mobiray.Common;

namespace CriminalTown.Configs
{
    [CreateAssetMenu(fileName = "ConfigMain", menuName = "Configs/ConfigMain")]
    public class ConfigMain : ScriptableObject, INeedInitialization
    {
        [Header("Game States")]
        public DataGameState initGameState;
        public DataGameState debugGameState;

        [Header("Other")]
        public float connectionTime = 1;
        public float emitResourceTime = 0.8f;
        public int emitMoneyMin = 10;
        public int emitMoneyMax = 800;

        [Header("Citizens")]
        public int citizenHealth = 3;
        public float citizenSpeedWalk = 2;
        public float citizenSpeedRun = 3;
        
        [Space]
        public List<Material> citizenMaterials;
        public List<EntityCitizen> citizenPrefabs;
        public List<EntityCitizen> policePrefabs;

        [Header("Citizen System")]
        public float updateCitizensTime = 1;
        public float citizenPlayerDistanceMin = 8;
        public float citizenPanicDistance = 4;
        public float citizenPanicAngle = 120;
        public float citizenDeathTime = 6f;

        [Header("Police")]
        public float policeCatchTime = 10;
        public float policePassiveTime = 10;
        public float policeVisibilityDelay = 1f;
        public float hidingTimeBonus = 1.2f;
        public int policeHelperCount = 1;

        [Space]
        public bool activateCitizenPanic = true;
        public bool activatePolicePanic = true;
        public float policePanicDistance = 10;

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