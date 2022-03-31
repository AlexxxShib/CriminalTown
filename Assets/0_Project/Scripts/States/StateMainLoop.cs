using CriminalTown.Configs;
using CriminalTown.Controllers;
using CriminalTown.Data;
using CriminalTown.Entities;
using Mobiray.Common;
using Mobiray.StateMachine;
using UnityEngine;

namespace CriminalTown.States
{
    
    [CreateAssetMenu(fileName = "StateMainLoop", menuName = "GameState/StateMainLoop")]
    public class StateMainLoop : BaseGameState, IReceive<SignalIslandPurchased>, IReceive<SignalPlayerCaught>
    {

        public int lowCameraPriorityDefault = 30;
        public int highCameraPriorityDefault = 20;

        private ConfigBalance _balance;
        private EntityPlayer _player;

        public override void Initialize(GameController character, StateMachine<GameController> stateMachine)
        {
            base.Initialize(character, stateMachine);

            _balance = ToolBox.Get<ConfigBalance>();
        }

        public override void Enter()
        {
            base.Enter();
            
            _host.screenMain.SetActive(true);
            
            _player = ToolBox.Get<EntityPlayer>();
        }

        public override void Exit()
        {
            base.Exit();
            
            _host.screenMain.SetActive(false);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            UpdateCameraPos();
        }

        private void UpdateCameraPos()
        {
            var cameraPos = _host.cameraLow.transform.position;
            var playerDir = _player.transform.position - cameraPos;

            var cameraRay = new Ray(cameraPos, playerDir);

            var lowCameraPriority = lowCameraPriorityDefault;
            var highCameraPriority = highCameraPriorityDefault;

            if (Physics.Raycast(cameraRay, out var hit))
            {
                if (hit.transform.gameObject.layer != _player.gameObject.layer)
                {
                    (lowCameraPriority, highCameraPriority) = (highCameraPriority, lowCameraPriority);
                }
            }

            _host.cameraLow.Priority = lowCameraPriority;
            _host.cameraHigh.Priority = highCameraPriority;
        }

        public void HandleSignal(SignalIslandPurchased signal)
        {
            _host.islandSurface.BuildNavMesh();

            for (var i = 0; i < _gameState.branches.Count; i++)
            {
                for (var j = 0; j < _gameState.branches[i].islands.Count; j++)
                {
                    if (_gameState.branches[i].islands[j].state == IslandState.CLOSED && 
                        (j == 0 || _gameState.branches[i].islands[j - 1].state == IslandState.OPENED))
                    {
                        _host.islands[i][j].SetAvailable();
                    }
                }
            }
            
            _host.SaveGameState();
        }

        public void HandleSignal(SignalPlayerCaught signal)
        {
            _gameState.AddMoney(-_balance.policeFine);
            
            _stateMachine.ChangeState(_host.stateGameOver);
        }
    }
}