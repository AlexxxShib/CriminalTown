using Mobiray.Common;
using UnityEngine;

namespace Mobiray.StateMachine
{
    public abstract class State<T> : ScriptableObject
    {

        protected T host;
        protected StateMachine<T> stateMachine;

        protected MobirayLogger logger;

        public virtual void Initialize(T host, StateMachine<T> stateMachine)
        {
            this.host = host;
            this.stateMachine = stateMachine;
            
            logger = new MobirayLogger(GetType().Name);
        }

        public virtual void Enter() { }

        // public virtual void HandleInput() { }

        public virtual void LogicUpdate() { }

        public virtual void PhysicsUpdate() { }

        public virtual void DrawGizmosSelected() { }

        // public virtual void HandleCollisionEnter(Collision other) { }

        public virtual void Exit() { }
    }
}