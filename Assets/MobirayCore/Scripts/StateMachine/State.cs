using Mobiray.Common;
using UnityEngine;

namespace Mobiray.StateMachine
{
    public abstract class State<T> : ScriptableObject
    {

        protected T character;
        protected StateMachine<T> stateMachine;

        protected MobirayLogger logger;

        public virtual void Initialize(T character, StateMachine<T> stateMachine)
        {
            this.character = character;
            this.stateMachine = stateMachine;
            
            logger = new MobirayLogger(GetType().Name);
        }

        public virtual void Enter() { }

        public virtual void HandleInput() { }

        public virtual void LogicUpdate() { }

        public virtual void PhysicsUpdate() { }

        public virtual void DrawGizmosSelected() { }

        // public virtual void HandleCollisionEnter(Collision other) { }

        public virtual void Exit() { }
    }
}