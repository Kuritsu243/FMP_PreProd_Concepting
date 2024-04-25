using Unity.Cinemachine;

namespace Cameras.FSM
{
    public abstract class CameraState
    {
        protected CameraController CameraController;
        protected CameraStateMachine StateMachine;

        public bool IsChangingPerspective { get; set; }

        protected CinemachineCamera StateCamera { get; set; }


        protected CameraState(CameraStateMachine stateMachine, CameraController cameraController,
            CinemachineCamera stateCamera)
        {
            StateMachine = stateMachine;
            CameraController = cameraController;
            StateCamera = stateCamera;
            CameraController.playerInput.actions["Perspective"].performed += _ => PerspectiveChange();
        }

        public void Enter()
        {
            MainCamera.SetActiveCamera(StateCamera);
        }

        public void HandleInput()
        {
        }

        public virtual void LogicUpdate()
        {
        }

        public void PhysicsUpdate()
        {
        }

        private void PerspectiveChange()
        {
            StateMachine.CurrentState.IsChangingPerspective = true;
        }

        public void Exit()
        {
            IsChangingPerspective = false;
        }
    }
}