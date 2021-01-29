using Mobiray.Common;
using Mobiray.Controllers;
using Template.Configs;
using UnityEngine;

namespace Template.Controllers
{
    public class SoundController : SignalReceiver
    {
        public ConfigTaptic ConfigTaptic;

        public bool Vibro;

        private static string vibroPref = "isVibroOn";

        public void SetVibro(bool vibro)
        {
            PlayerPrefs.SetInt(vibroPref, vibro ? 1 : 0);

            Vibro = vibro;
        }

        private void Awake()
        {
            Vibro = PlayerPrefs.GetInt(vibroPref, 1) == 1;
            
            ToolBox.Add(this);
        }

        private void OnDestroy()
        {
            ToolBox.Remove<SoundController>();
        }
    
        public void PlayCollision()
        {
            PlayTaptic(ConfigTaptic.Collision);
        }

        public void PlayAddMoney()
        {
            PlayTaptic(ConfigTaptic.CollectMoney);
        }
        
        public void PlayUi()
        {
            PlayTaptic(ConfigTaptic.Ui);
        }


        private void PlayTaptic(TapticTypes type)
        {
            if (!Vibro) return;
            
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