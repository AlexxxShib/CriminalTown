using System;
using UnityEngine;

namespace CriminalTown.Configs
{
    [CreateAssetMenu(fileName = "TapticConfig", menuName = "Configs/Taptic")]
    public class ConfigTaptic : ScriptableObject
    {
        // public TapticTypes Collision;
        // public TapticTypes CollectMoney;
        // public TapticTypes Ui;

        public HapticConfig collision;
        public HapticConfig explosion;
    }
    
    [Serializable]
    public class HapticConfig
    {
        public long[] androidVibration = {0, 100};
        public TapticTypes iOSVibration;
    }

    public enum TapticTypes
    {
        Light,
        Medium,
        Hard,
        Success,
        Warning,
        Error
    }
}