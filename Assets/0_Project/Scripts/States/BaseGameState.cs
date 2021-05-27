using Mobiray.Common;
using Mobiray.Helpers;
using Mobiray.StateMachine;
using Template.Configs;
using Template.Controllers;
using Template.Data;
using UnityEngine;

namespace Template.States
{
    public class BaseGameState : State<GameController>
    {

        // public LevelState LevelState;
        [Space]
        
        protected GameSettings settings;
        protected ConfigMain configs;
        // protected LevelConfig levelConfig;
        protected DataGameState gameState;
        
        protected TimerHelper timerHelper;

        public override void Initialize(GameController character, StateMachine<GameController> stateMachine)
        {
            base.Initialize(character, stateMachine);
            
            configs = ToolBox.Get<ConfigMain>();
            settings = ToolBox.Get<GameSettings>();
            gameState = ToolBox.Get<DataGameState>();
            // levelConfig = ToolBox.Get<LevelConfig>();
            timerHelper = ToolBox.Get<TimerHelper>();
        }

        public override void Enter()
        {
            base.Enter();
            
            ToolBox.Signals.Add(this);
            
            // character.SessionData.ChangeLevelState(LevelState);
        }

        public override void Exit()
        {
            base.Exit();
            
            ToolBox.Signals.Remove(this);
        }
    }
}