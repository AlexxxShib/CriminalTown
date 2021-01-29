using UnityEngine;

namespace Template.Configs
{
    [CreateAssetMenu(fileName = "TapticConfig", menuName = "Configs/Taptic")]
    public class ConfigTaptic : ScriptableObject
    {
        public TapticTypes Collision;
        public TapticTypes CollectMoney;
        public TapticTypes Ui;
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