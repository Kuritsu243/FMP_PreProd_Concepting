using System;
using UnityEngine;

namespace Player.FSM
{
    public abstract class FiniteStateMachine
    {
        private FsmState initialState;
        
        public FsmState CurrentState { get; set; }
        public FsmState PreviousState { get; set; }
        
        

        public void Initialize(FsmState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
            
        }
        

        public void ChangeState(FsmState newState)
        {
            PreviousState = CurrentState;
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState?.Enter();
        }
        

    }
}
