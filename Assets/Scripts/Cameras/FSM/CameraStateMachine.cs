using Cameras.FSM.States;
using UnityEngine;

namespace Cameras.FSM
{
    public abstract class CameraStateMachine
    {
        private CameraState _initialState;
        
        public CameraState CurrentState { get; private set; }
        
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
