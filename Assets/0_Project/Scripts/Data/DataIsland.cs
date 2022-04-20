using System;
using System.Collections.Generic;

namespace CriminalTown.Data
{

    public enum IslandState
    {
        OPENED, AVAILABLE, CLOSED, LOCK
    }

    [Serializable]
    public class DataIslandBranch
    {
        public List<DataIsland> islands;
    }
    
    [Serializable]
    public class DataIsland
    {
        public int branch;
        public int index;
        public IslandState state;
        public int currentPrice;
    }
}