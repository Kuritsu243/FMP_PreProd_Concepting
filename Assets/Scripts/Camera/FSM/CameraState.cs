using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Camera;

namespace Camera.FSM
{
    public abstract class CameraState
    {
        protected CameraController CameraController;
        protected CameraStateMachine StateMachine;
        protected string StateName;

        protected readonly InputAction PerspectiveAction;
        public bool IsChangingPerspective { get; set; }

        protected CinemachineCamera StateCamera { get; set; }


        protected CameraState(string stateName, CameraStateMachine stateMachine, CameraController cameraController,
            CinemachineCamera stateCamera)
        {
            this.StateName = stateName;
            this.StateMachine = stateMachine;
            this.CameraController = cameraController;
            this.StateCamera = stateCamera;

            // PerspectiveAction = cameraController.playerInput.actions["Perspective"];
            CameraController.playerInput.actions["Perspective"].performed += _ => PerspectiveChange();
        }

        public virtual void Enter()
        {
            // foreach (var cinemachineCamera in CameraController.cinemachineCameras)
            // {
            //     if (cinemachineCamera == ActiveCamera) return;
            //     cinemachineCamera.Priority.Value = 0;
            // }
            // MainCamera.ChangeCamera(ActiveCamera);

            MainCamera.SetActiveCamera(StateCamera);
        }

        public virtual void HandleInput()
        {
        }

        public virtual void LogicUpdate()
        {
            
        }

        public virtual void PhysicsUpdate()
        {
            
        }

        public virtual void Tick(float deltaTime)
        {
            
        }

        private void PerspectiveChange()
        {
            Debug.LogWarning("changing perspective");
            StateMachine.CurrentState.IsChangingPerspective = true;
        }

        public virtual void Exit()
        {
            IsChangingPerspective = false;
        }
        
        
        
        
        
    }
}
