using System.Collections.Generic;
using CriminalTown.Controllers;
using CriminalTown.Data;
using CriminalTown.Entities;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.AI;

namespace CriminalTown.States
{
    
    [CreateAssetMenu(fileName = "StatePreparing", menuName = "GameState/StatePreparing")]
    public class StatePreparing : BaseGameState
    {

        public override void Enter()
        {
            base.Enter();
            
            _host.screenMain.SetActive(false);
            _host.screenLoading.SetActive(true);
            
            var sessionData = ToolBox.Get<SessionData>();
            Analytics.SendLevelStart(sessionData);//TODO

            InitializeIslands();
            
            _timerHelper.StartTimer(0.25f, LoadingComplete);
        }

        private void LoadingComplete()
        {
            _host.screenLoading.SetActive(false);
            
            _stateMachine.ChangeState(_host.stateMainLoop);
        }

        private void InitializeIslands()
        {
            _host.islands.AddRange(_host.parentIslands.GetComponentsInChildren<EntityIsland>(true));
            
            for (var i = 0; i < _host.islands.Count; i++)
            {
                DataIsland dataIsland;

                if (i < _gameState.islands.Count)
                {
                    dataIsland = _gameState.islands[i];
                }
                else
                {
                    dataIsland = new DataIsland
                    {
                        index = i,
                        state = IslandState.CLOSED
                    };
                    
                    _gameState.islands.Add(dataIsland);
                }

                _host.islands[i].Initialize(dataIsland);

                if (dataIsland.state == IslandState.CLOSED &&
                    (i == 0 || _gameState.islands[i - 1].state == IslandState.OPENED))
                {
                    _host.islands[i].SetAvailable();
                }
            }
            
            _host.islandSurface.BuildNavMesh();
        }
    }
}