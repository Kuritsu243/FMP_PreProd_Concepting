using Unity.Cinemachine;

namespace Camera.FSM.States
{
    public class ThirdPersonState : CameraState
    {
        
        public ThirdPersonState(string stateName, CameraStateMachine stateMachine, CameraController cameraController, CinemachineCamera stateCamera) : base(stateName, stateMachine, cameraController, stateCamera)
        {
            StateName = stateName;
            StateMachine = stateMachine;
            CameraController = cameraController;
            StateCamera = stateCamera;
        }
        
        
        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();

        }

   
        
        
        public override void HandleInput()
        {
            base.HandleInput();

            // IsChangingPerspective = PerspectiveAction.IsPressed();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (StateMachine.CurrentState.IsChangingPerspective)
                StateMachine.ChangeState(CameraController.FirstPersonState);
        }
        
    }
}
