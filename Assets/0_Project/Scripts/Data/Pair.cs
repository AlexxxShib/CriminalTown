using System;

namespace CriminalTown.Data
{
    [Serializable]
    public struct Pair<T, TD>
    {
        public T key;
        public TD value;
    }
}