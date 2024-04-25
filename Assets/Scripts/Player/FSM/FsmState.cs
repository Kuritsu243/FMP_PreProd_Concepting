using UnityEngine;
using UnityEngine.InputSystem;
using Cameras;
using Tutorial;
using Unity.Cinemachine;

namespace Player.FSM
{
    public abstract class FsmState
    {
        protected PlayerController Character;
        protected readonly TutorialController TutorialController;
        protected FiniteStateMachine StateMachine;

        protected readonly InputAction MoveAction;
        protected readonly InputAction JumpAction;
        protected readonly InputAction SlideAction;
        protected readonly float PlayerSpeed;
        protected readonly float GravityValue;
        private Vector2 _mouseInput;
        private float _mouseX;
        private float _mouseY;
        // ReSharper disable once NotAccessedField.Local
        private CinemachineCamera _thirdPersonCam;
        // ReSharper disable once NotAccessedField.Local
        private CinemachineCamera _firstPersonCam;
        private float _xRotation;
        private Vector3 _targetRotation;

        protected FsmState(FiniteStateMachine stateMachine, PlayerController playerController)
        {
            StateMachine = stateMachine;
            Character = playerController;


            MoveAction = playerController.playerInput.actions["Movement"];
            JumpAction = playerController.playerInput.actions["Jump"];
            SlideAction = playerController.playerInput.actions["Slide"];
            PlayerSpeed = Character.PlayerSpeed;
            GravityValue = Character.PlayerGravity;

            if (Character.IsTutorial) TutorialController = Character.tutorialController;
        }
        
        
        public virtual void Enter()
        {
        }

        public virtual void HandleInput()
        {
        }

        public virtual void LogicUpdate()
        {
            
        }

        public virtual void PhysicsUpdate()
        {
            CameraChanger.GetActiveCams(out _thirdPersonCam, out _firstPersonCam);
            switch (MainCamera.ActiveCameraMode)
            {
                case CameraChanger.CameraModes.FirstPerson:
                    var playerLocalRotation = Character.PlayerTransform.localRotation;
                    playerLocalRotation.y = Character.activeCinemachineBrain.transform.rotation.y;
                    Character.PlayerTransform.localRotation = playerLocalRotation;
                    break;
            }
        }


        public virtual void Tick(float deltaTime)
        {
            
        }

        public virtual void Exit()
        {
        }
        
    }
}