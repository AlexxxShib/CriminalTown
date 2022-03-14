using CriminalTown.Controllers;
using CriminalTown.Data;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.States
{
    
    [CreateAssetMenu(fileName = "StateGameOver", menuName = "GameState/StateGameOver")]
    public class StateGameOver : BaseGameState
    {
        public override void Enter()
        {
            base.Enter();
            
            _host.screenGameOver.SetActive(true);
            
            Analytics.SendLevelFinish(ToolBox.Get<SessionData>(), false);
        }
    }
}