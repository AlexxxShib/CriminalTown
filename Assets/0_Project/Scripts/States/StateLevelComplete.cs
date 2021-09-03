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

            gameState.CurrentLevel++;
            host.SaveGameState();
            
            host.ScreenLevelComplete.SetActive(true);
            
            Analytics.SendLevelFinish(ToolBox.Get<SessionData>(), true);
        }
    }
}