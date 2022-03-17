using CriminalTown.Entities;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.States
{
    
    [CreateAssetMenu(fileName = "StateMainLoop", menuName = "GameState/StateMainLoop")]
    public class StateMainLoop : BaseGameState, IReceive<SignalIslandPurchased>
    {
        
        public override void Enter()
        {
            base.Enter();
            
            _host.screenMain.SetActive(true);
        }

        public override void Exit()
        {
            base.Exit();
            
            _host.screenMain.SetActive(false);
        }

        public void HandleSignal(SignalIslandPurchased signal)
        {
            _host.islandSurface.BuildNavMesh();
        }
    }
}