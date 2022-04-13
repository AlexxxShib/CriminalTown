using CriminalTown.Data;
using UnityEngine;

namespace CriminalTown.Configs
{
    
    [CreateAssetMenu(fileName = "crime_xxx", menuName = "Configs/Crime")]
    public class ConfigCrime : ScriptableObject
    {
        public CrimeType type;
        public float timeLock;
        public int pursuitLevel;
    }
}