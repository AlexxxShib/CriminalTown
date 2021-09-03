using System.Collections.Generic;
using Mobiray.Common;
using Template.Controllers;
using Template.Data;
using UnityEngine;
using UnityEngine.AI;

namespace Template.States
{
    
    [CreateAssetMenu(fileName = "StatePreparing", menuName = "GameState/StatePreparing")]
    public class StatePreparing : BaseGameState
    {

        private Transform currentLevel;
        
        public override void Enter()
        {
            base.Enter();
            
            host.ScreenMain.SetActive(false);
            host.ScreenLoading.SetActive(true);
            
            timerHelper.StartTimer(1, LoadingComplete);
            
            var sessionData = ToolBox.Get<SessionData>();

            sessionData.CurrentLevel = gameState.CurrentLevel;
            
            host.CurrentLevel = currentLevel = GetCurrentLevel(gameState.CurrentLevel, 
                out sessionData.LevelNumber, out sessionData.LevelLoop);
            
            Analytics.SendLevelStart(sessionData);

            host.TextLevel.text = $"LEVEL {gameState.CurrentLevel + 1}";
            
            //INIT LEVEL
        }

        private void LoadingComplete()
        {
            host.ScreenLoading.SetActive(false);
            
            stateMachine.ChangeState(host.StateMainLoop);
        }

        private Transform GetCurrentLevel(int currentLevel, out int levelNum, out int loop)
        {
            if (host.TutorialLevelsParent != null)
            {
                var tutorialLevels = host.TutorialLevelsParent.GetChildren();

                if (currentLevel < tutorialLevels.Count)
                {
                    host.LevelsParent.gameObject.SetActive(false);
                    host.TutorialLevelsParent.gameObject.SetActive(true);
                    
                    for (var i = 0; i < tutorialLevels.Count; i++)
                    {
                        tutorialLevels[i].gameObject.SetActive(i == currentLevel);
                    }

                    levelNum = currentLevel + 1;
                    loop = 1;
                    
                    return tutorialLevels[currentLevel];
                }

                host.TutorialLevelsParent.gameObject.SetActive(false);
                currentLevel -= tutorialLevels.Count;
            }
            
            host.LevelsParent.gameObject.SetActive(true);
            
            var levels = host.LevelsParent.GetChildren();
            var localLevel = currentLevel % levels.Count;
            
            for (var i = 0; i < levels.Count; i++)
            {
                levels[i].gameObject.SetActive(i == localLevel);
            }

            levelNum = currentLevel + localLevel + 1;
            loop = currentLevel / levels.Count + 1;

            return levels[localLevel];
        }
    }
}