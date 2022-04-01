using System;
using System.Collections.Generic;
using CriminalTown.Data;
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
        public Vector2Int policeIsland;

        [Space]
        public List<IslandBranchConfig> branches;

        public bool IsPoliceOpened(List<List<EntityIsland>> islands)
        {
            if (policeIsland.x < islands.Count)
            {
                var branch = islands[policeIsland.x];

                if (policeIsland.y < branch.Count)
                {
                    return branch[policeIsland.y].data.state == IslandState.OPENED;
                }
            }
            
            return false;
        }
    }
    
}