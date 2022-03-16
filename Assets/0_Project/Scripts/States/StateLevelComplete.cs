using CriminalTown.Controllers;
using CriminalTown.Data;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.States
{
    
    [CreateAssetMenu(fileName = "StateLevelComplete", menuName = "GameState/StateLevelComplete")]
    public class StateLevelComplete : BaseGameState
    {
        public override void Enter()
        {
            base.Enter();

            // _gameState.currentLevel++;
            _host.SaveGameState();
            
            _host.screenLevelComplete.SetActive(true);
            
            Analytics.SendLevelFinish(ToolBox.Get<SessionData>(), true);
        }
    }
}