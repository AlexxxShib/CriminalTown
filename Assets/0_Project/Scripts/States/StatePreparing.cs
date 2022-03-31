using System.Collections.Generic;
using CriminalTown.Configs;
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
            // Analytics.SendLevelStart(sessionData);//TODO

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
            var balance = ToolBox.Get<ConfigBalance>();
            
            _host.islands = new List<List<EntityIsland>>();

            for (int i = 1; i < _host.parentIslands.childCount; i++)
            {
                var branchTransform = _host.parentIslands.GetChild(i);
                
                var islands = new List<EntityIsland>();
                for (int j = 0; j < branchTransform.childCount; j++)
                {
                    islands.Add(branchTransform.GetChild(j).GetComponent<EntityIsland>());
                }
                
                _host.islands.Add(islands);
            }
            
            for (var i = 0; i < _host.islands.Count; i++) // Island branches
            {
                DataIslandBranch branch;
                
                if (i < _gameState.branches.Count)
                {
                    branch = _gameState.branches[i];
                }
                else
                {
                    branch = new DataIslandBranch
                    {
                        islands = new List<DataIsland>()
                    };
                    
                    _gameState.branches.Add(branch);
                }
                
                for (var j = 0; j < _host.islands[i].Count; j++)
                {
                    var island = _host.islands[i][j];
                    
                    DataIsland dataIsland;

                    if (j < branch.islands.Count)
                    {
                        dataIsland = branch.islands[j];
                    }
                    else
                    {
                        dataIsland = new DataIsland
                        {
                            branch = i,
                            index = j,
                            state = IslandState.CLOSED
                        };
                        
                        branch.islands.Add(dataIsland);
                    }
                    
                    _host.islands[i][j].Initialize(dataIsland, balance.branches[i].islands[j]);
                    
                    if (dataIsland.state == IslandState.CLOSED &&
                        (j == 0 || _gameState.branches[i].islands[j - 1].state == IslandState.OPENED))
                    {
                        _host.islands[i][j].SetAvailable();
                    }//*/
                }//*/
            }
            
            _host.islandSurface.BuildNavMesh();
        }
    }
}