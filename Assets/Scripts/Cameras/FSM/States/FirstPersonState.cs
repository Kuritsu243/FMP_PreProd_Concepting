using Unity.Cinemachine;

namespace Cameras.FSM.States
{
    public class FirstPersonState : CameraState
    {
        private bool _isChangingPerspective;
        public FirstPersonState(string stateName, CameraStateMachine stateMachine, CameraController cameraController, CinemachineCamera stateCamera) : base(stateMachine, cameraController, stateCamera)
        {
            StateMachine = stateMachine;
            CameraController = cameraController;
            StateCamera = stateCamera;
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (StateMachine.CurrentState.IsChangingPerspective)
                StateMachine.ChangeState(CameraController.ThirdPersonState);
        }
    }
}
