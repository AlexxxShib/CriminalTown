using Template.Configs;
using Mobiray.Common;
using UnityEngine;

namespace Template.Controllers
{
    public class SoundController : SignalReceiver
    {
        private ConfigTaptic _configTaptic;
        private ConfigSounds _configSounds;

        public bool vibro;
        public bool localDisabledVibro;

        private const string PREF_VIBRO = "pref_vibro";

        public void SetVibro(bool vibro)
        {
            PlayerPrefs.SetInt(PREF_VIBRO, vibro ? 1 : 0);

            this.vibro = vibro;
        }

        private void Awake()
        {
            vibro = PlayerPrefs.GetInt(PREF_VIBRO, 1) == 1;
            
            ToolBox.Add(this);

            _configTaptic = ToolBox.Get<ConfigTaptic>();
            _configSounds = ToolBox.Get<ConfigSounds>();
        }

        public void SetEnabled(bool enabled)
        {
            localDisabledVibro = !enabled;
        }
    
        public void PlayCollision()
        {
            PlayHaptic(_configTaptic.collision);
        }

        public void PlayExplosion()
        {
            PlayHaptic(_configTaptic.explosion);
        }

        public void PlayAddMoney()
        {
            // PlayTaptic(ConfigTaptic.CollectMoney);
        }
        
        public void PlayUi()
        {
            // PlayTaptic(ConfigTaptic.Ui);
        }

        private void PlayHaptic(HapticConfig haptic)
        {
            if (!vibro || localDisabledVibro) return;

#if UNITY_ANDROID
            Vibration.Init();
            Vibration.Vibrate(haptic.androidVibration, -1);
#elif UNITY_IOS
            PlayIOSTaptic(haptic.iOSVibration);
#endif
        }


        private void PlayIOSTaptic(TapticTypes type)
        {
            switch (type)
            {
                case TapticTypes.Light:
                    TapticEngine.TriggerLight();
                    break;
                case TapticTypes.Medium:
                    TapticEngine.TriggerMedium();
                    break;
                case TapticTypes.Hard:
                    TapticEngine.TriggerHeavy();
                    break;
                case TapticTypes.Success:
                    TapticEngine.TriggerSuccess();
                    break;
                case TapticTypes.Warning:
                    TapticEngine.TriggerWarning();
                    break;
                case TapticTypes.Error:
                    TapticEngine.TriggerError();
                    break;
            }
        }
    }
}