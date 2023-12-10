using Camera.FSM.States;
using UnityEngine;

namespace Camera.FSM
{
    public abstract class CameraStateMachine
    {
        private CameraState initialState;
        
        public CameraState CurrentState { get; set; }
        
        public CameraState PreviousState { get; set; }

        public void Initialize(CameraState startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }

        public void ChangeState(CameraState newState)
        {
            PreviousState = CurrentState;
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState?.Enter();
        }

    }

    public class CameraMachine : CameraStateMachine
    {
        [HideInInspector] public FirstPersonState FirstPersonState;
        [HideInInspector] public ThirdPersonState ThirdPersonState;
    }
}
