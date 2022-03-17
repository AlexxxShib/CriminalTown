using CriminalTown.Data;
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

            for (var i = 0; i < _gameState.islands.Count; i++)
            {
                if (i > 0 && _gameState.islands[i - 1].state == IslandState.OPENED && 
                    _gameState.islands[i].state == IslandState.CLOSED)
                {
                    _host.islands[i].SetAvailable();
                }
            }
        }
    }
}