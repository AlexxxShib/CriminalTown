using UnityEngine;

namespace Template.States
{
    
    [CreateAssetMenu(fileName = "StateMainLoop", menuName = "GameState/StateMainLoop")]
    public class StateMainLoop : BaseGameState
    {
        
        public override void Enter()
        {
            base.Enter();
            
            host.ScreenMain.SetActive(true);
        }

        public override void Exit()
        {
            base.Exit();
            
            host.ScreenMain.SetActive(false);
        }
        
    }
}