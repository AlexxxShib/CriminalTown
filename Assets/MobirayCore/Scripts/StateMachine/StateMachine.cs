using UnityEngine;

namespace Mobiray.StateMachine
{
    public class StateMachine<T>
    {
        public State<T> CurrentState { get; private set; }

        public void Initialize(State<T> startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }

        public void ChangeState(State<T> newState)
        {
            Debug.Log($"Change state from {CurrentState.GetType().Name} to {newState.GetType().Name}");
            
            CurrentState.Exit();

            CurrentState = newState;
            newState.Enter();
        }

    }
}