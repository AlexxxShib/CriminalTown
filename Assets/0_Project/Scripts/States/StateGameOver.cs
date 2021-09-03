using Mobiray.Common;
using Template.Controllers;
using Template.Data;
using UnityEngine;

namespace Template.States
{
    
    [CreateAssetMenu(fileName = "StateGameOver", menuName = "GameState/StateGameOver")]
    public class StateGameOver : BaseGameState
    {
        public override void Enter()
        {
            base.Enter();
            
            host.ScreenGameOver.SetActive(true);
            
            Analytics.SendLevelFinish(ToolBox.Get<SessionData>(), false);
        }
    }
}