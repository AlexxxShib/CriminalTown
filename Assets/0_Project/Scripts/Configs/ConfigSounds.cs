using UnityEngine;

namespace Template.Configs
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Configs/Sounds")]
    public class ConfigSounds : ScriptableObject
    {
        public AudioClip Buy;
        public AudioClip StartTrain;
        public AudioClip AppearTrain;
        public AudioClip TapUI;
        public AudioClip Music;
    }
}