using System;

namespace CriminalTown.Data
{

    public enum IslandState
    {
        OPENED, AVAILABLE, CLOSED
    }
    
    [Serializable]
    public class DataIsland
    {
        public int index;
        public IslandState state;
        public int currentPrice;
    }
}