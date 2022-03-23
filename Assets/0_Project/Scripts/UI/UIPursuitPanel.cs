using CriminalTown.Controllers;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.UI;

namespace CriminalTown.UI
{
    public class UIPursuitPanel : SignalReceiver, IReceive<SignalPoliceActivated>, IReceive<SignalPoliceDeactivated>
    {
        public GameObject body;

        [Space]
        public Color colorNormal;
        public Color colorNotVisible;
        public Color colorShelter;

        [Space]
        public Image imageProgress;
        public GameObject shelterStatus;

        private void Awake()
        {
            body.SetActive(false);
            
            ToolBox.Get<CitizenSystem>().OnCatchingProgress += CatchingProgress;
        }

        private void CatchingProgress(float progress, bool isHidden, bool isVisible)
        {
            imageProgress.fillAmount = progress;
            shelterStatus.SetActive(isHidden);

            if (isHidden)
            {
                imageProgress.color = colorShelter;
                return;
            }

            imageProgress.color = isVisible ? colorNormal : colorNotVisible;
        }

        public void HandleSignal(SignalPoliceActivated signal)
        {
            body.SetActive(true);
        }

        public void HandleSignal(SignalPoliceDeactivated signal)
        {
            body.SetActive(false);
        }
    }
}