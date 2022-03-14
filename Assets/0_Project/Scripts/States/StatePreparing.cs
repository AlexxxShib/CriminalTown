using System.Collections.Generic;
using CriminalTown.Controllers;
using CriminalTown.Data;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.AI;

namespace CriminalTown.States
{
    
    [CreateAssetMenu(fileName = "StatePreparing", menuName = "GameState/StatePreparing")]
    public class StatePreparing : BaseGameState
    {

        private Transform _currentLevel;
        
        public override void Enter()
        {
            base.Enter();
            
            _host.screenMain.SetActive(false);
            _host.screenLoading.SetActive(true);
            
            _timerHelper.StartTimer(1, LoadingComplete);
            
            var sessionData = ToolBox.Get<SessionData>();
            sessionData.currentLevel = _gameState.currentLevel;
            
            _host.currentLevel = _currentLevel = GetCurrentLevel(_gameState.currentLevel, out sessionData.levelLoop);
            
            Analytics.SendLevelStart(sessionData);

            _host.textLevel.text = $"LEVEL {_gameState.currentLevel + 1}";
            
            //INIT LEVEL
        }

        private void LoadingComplete()
        {
            _host.screenLoading.SetActive(false);
            
            _stateMachine.ChangeState(_host.stateMainLoop);
        }

        private Transform GetCurrentLevel(int currentLevel, out int loop)
        {
            if (_host.tutorialLevelsParent != null)
            {
                var tutorialLevels = _host.tutorialLevelsParent.GetChildren();

                if (currentLevel < tutorialLevels.Count)
                {
                    _host.levelsParent.gameObject.SetActive(false);
                    _host.tutorialLevelsParent.gameObject.SetActive(true);
                    
                    for (var i = 0; i < tutorialLevels.Count; i++)
                    {
                        tutorialLevels[i].gameObject.SetActive(i == currentLevel);
                    }

                    loop = 1;
                    
                    return tutorialLevels[currentLevel];
                }

                _host.tutorialLevelsParent.gameObject.SetActive(false);
                currentLevel -= tutorialLevels.Count;
            }
            
            _host.levelsParent.gameObject.SetActive(true);
            
            var levels = _host.levelsParent.GetChildren();
            var localLevel = currentLevel % levels.Count;
            
            for (var i = 0; i < levels.Count; i++)
            {
                levels[i].gameObject.SetActive(i == localLevel);
            }

            loop = currentLevel / levels.Count + 1;

            return levels[localLevel];
        }
    }
}