using Mobiray.Common;
using Template.Controllers;
using Template.Data;
using UnityEngine;

namespace Template.States
{
    
    [CreateAssetMenu(fileName = "StateLevelComplete", menuName = "GameState/StateLevelComplete")]
    public class StateLevelComplete : BaseGameState
    {
        public override void Enter()
        {
            base.Enter();

            _gameState.currentLevel++;
            _host.SaveGameState();
            
            _host.screenLevelComplete.SetActive(true);
            
            Analytics.SendLevelFinish(ToolBox.Get<SessionData>(), true);
        }
    }
}