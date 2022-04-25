using CriminalTown.Components.Connectors;
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
    public class StateMainLoop : BaseGameState, IReceive<SignalIslandPurchased>, IReceive<SignalPlayerCaught>, IReceive<SignalUICaughtContinue>
    {

        public int lowCameraPriorityDefault = 30;
        public int highCameraPriorityDefault = 20;

        private ConfigBalance _balance;
        private EntityPlayer _player;
        private IslandConnector _islandConnector;

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
            _islandConnector = _player.GetComponent<IslandConnector>();
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
            if (_player.isHidden || _player.isCaught)
            {
                _host.cameraLow.Priority = lowCameraPriorityDefault;
                _host.cameraHigh.Priority = highCameraPriorityDefault;
                
                return;
            }

            if (_player.isPursuit || _islandConnector.IsConnected)
            {
                _host.cameraLow.Priority = highCameraPriorityDefault;
                _host.cameraHigh.Priority = lowCameraPriorityDefault;
                
                return;
            }
            
            var cameraPos = _host.cameraLow.transform.position;
            var playerDir = _player.transform.position + new Vector3(0, 0.5f, 0) - cameraPos;

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
                    var curState = _gameState.branches[i].islands[j].state;
                    
                    if (curState == IslandState.LOCK || curState == IslandState.CLOSED && 
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
            // _gameState.AddMoney(-_balance.policeFine);
            
            _host.screenMain.SetActive(false);
            _host.screenGameOver.SetActive(true);
        }

        public void HandleSignal(SignalUICaughtContinue signal)
        {
            _host.screenMain.SetActive(true);
            _host.screenGameOver.SetActive(false);
        }
    }
}