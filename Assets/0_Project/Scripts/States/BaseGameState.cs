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
        protected GameSettings _settings;
        protected ConfigMain _configs;
        protected DataGameState _gameState;
        protected TimerHelper _timerHelper;

        public override void Initialize(GameController character, StateMachine<GameController> stateMachine)
        {
            base.Initialize(character, stateMachine);
            
            _configs = ToolBox.Get<ConfigMain>();
            _settings = ToolBox.Get<GameSettings>();
            _gameState = ToolBox.Get<DataGameState>();
            _timerHelper = ToolBox.Get<TimerHelper>();
        }

        public override void Enter()
        {
            base.Enter();
            
            ToolBox.Signals.Add(this);
        }

        public override void Exit()
        {
            base.Exit();
            
            ToolBox.Signals.Remove(this);
        }
    }
}