using System;
using System.Collections.Generic;
using CriminalTown.Entities;
using UnityEngine;

namespace CriminalTown.Configs
{
    
    [Serializable]
    public class IslandConfig
    {
        public int price;
        public int citizensCount;
        public int policeCount;

        // [Space]
        // public List<EntityCitizen> citizenPrefabs;
    }

    [Serializable]
    public class IslandBranchConfig
    {
        public List<IslandConfig> islands;
    }
    
    [CreateAssetMenu(fileName = "BalanceConfig", menuName = "Configs/Balance", order = 0)]
    public class ConfigBalance : ScriptableObject
    {
        public int policeFine = 200;
        public int policeReward = 300;
        
        public List<IslandBranchConfig> branches;
    }
    
}