using Mobiray.Controllers;

namespace Template.Controllers
{
    public class UIController : SignalReceiver
        // IReceive<SignalLoading>,
        // IReceive<SignalPreStart>,
        // IReceive<SignalShowLevelUpDialog>,
        // IReceive<SignalShowDialogUpgrades>,
        // IReceive<SignalHideDialogUpgrades>
    {
        
        /*public GameObject GameSceneUI;

        public GameObject LoadingScreen;
        public GameObject TapToStartScreen;

        [Space]
        public GameObject DialogFinishLevel;
        public GameObject DialogLevelUp;
        public GameObject DialogUpgrades;*/

        /*public void HandleSignal(SignalLoading signal)
        {
            LoadingScreen.SetActive(true);
        }

        public void HandleSignal(SignalPreStart signal)
        {
            LoadingScreen.SetActive(false);
            TapToStartScreen.SetActive(true);
        }

        public void HandleSignal(SignalStartGame signal)
        {
            GameSceneUI.SetActive(true);
            TapToStartScreen.SetActive(false);
            
//            this.StartTimer(3, () => TapToStartScreen.SetActive(true));
        }*/

    }
}