using Unity.Cinemachine;

namespace Cameras.FSM.States
{
    public class FirstPersonState : CameraState
    {

        private bool isChangingPerspective;
        public FirstPersonState(string stateName, CameraStateMachine stateMachine, CameraController cameraController, CinemachineCamera stateCamera) : base(stateName, stateMachine, cameraController, stateCamera)
        {
            StateName = stateName;
            StateMachine = stateMachine;
            CameraController = cameraController;
            StateCamera = stateCamera;
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
                StateMachine.ChangeState(CameraController.ThirdPersonState);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
