using UnityEngine;

namespace CriminalTown.Configs
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Configs/Sounds")]
    public class ConfigSounds : ScriptableObject
    {
        public AudioClip buy;
        public AudioClip tapUI;
        
        [Space]
        public AudioClip music;
    }
}