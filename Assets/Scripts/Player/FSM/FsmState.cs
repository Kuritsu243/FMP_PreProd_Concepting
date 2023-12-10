using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Camera;
using Unity.Cinemachine;

namespace Player.FSM
{
    public abstract class FsmState
    {
        protected PlayerController Character;
        protected FiniteStateMachine StateMachine;
        protected string StateName;

        protected readonly InputAction MoveAction;
        protected readonly InputAction LookAction;
        public InputAction PerspectiveAction;
        protected readonly InputAction JumpAction;
        protected readonly InputAction SlideAction;
        public InputAction SprintAction;
        protected readonly float PlayerSpeed;
        protected readonly float GravityValue;
        private Vector2 _mouseInput;
        private float _mouseX;
        private float _mouseY;
        private CinemachineCamera _thirdPersonCam;
        private CinemachineCamera _firstPersonCam;
        private float _xRotation;
        private Vector3 _targetRotation;
        protected float XClamp;
        protected Vector2 md;
        protected Vector2 mouseLook;

        protected FsmState(string stateName, FiniteStateMachine stateMachine, PlayerController playerController)
        {
            this.StateName = stateName;
            this.StateMachine = stateMachine;
            this.Character = playerController;


            MoveAction = playerController.playerInput.actions["Movement"];
            LookAction = playerController.playerInput.actions["Look"];
            PerspectiveAction = playerController.playerInput.actions["Perspective"];
            JumpAction = playerController.playerInput.actions["Jump"];
            SlideAction = playerController.playerInput.actions["Slide"];
            SprintAction = playerController.playerInput.actions["Sprint"];
            PlayerSpeed = Character.PlayerSpeed;
            GravityValue = Character.PlayerGravity;
        }
        
        
        // mechanics
        public virtual void Enter()
        {
        }

        public virtual void HandleInput()
        {

            // _mouseInput = LookAction.ReadValue<Vector2>();
            // _mouseX = _mouseInput.x * Character.MouseSensitivity.x;
            // _mouseY = _mouseInput.y * Character.MouseSensitivity.y;
            
            // Debug.Log($"Mouse Input: {MouseInput}");
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
            // CameraChanger.GetActiveCams(out _thirdPersonCam, out _firstPersonCam);
            // switch (MainCamera.ActiveCameraMode)
            // {
            //     case CameraChanger.CameraModes.FirstPerson:
            //         md.x = _mouseX;
            //         md.y = _mouseY;
            //         Vector2 smoothV = Vector2.zero;
            //         // the interpolated float result between the two float values
            //         smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f);
            //         smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f);
            //         // incrementally add to the camera look
            //         mouseLook += smoothV;
            //
            //         // vector3.right means the x-axis
            //         Character.mainCamera.transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
            //         Character.PlayerTransform.localRotation = Quaternion.AngleAxis(mouseLook.x, Character.PlayerTransform.up);
            //         
            //         // if (_mouseInput is {x: 0, y: 0}) return;
            //         // Character.playerMesh.transform.Rotate(Vector3.up, _mouseX * Time.deltaTime);
            //         // // _xRotation -= _mouseY;
            //         // // _xRotation = Mathf.Clamp(_xRotation, -Character.XClamp, Character.XClamp);
            //         // // Debug.LogWarning($"Target Rotation: {_targetRotation}");
            //         // // var prevRotation = _targetRotation;
            //         // // _targetRotation = Character.playerMesh.transform.eulerAngles;
            //         // // if (_targetRotation.x != 0f && Math.Abs(prevRotation.x - -_targetRotation.x) < Mathf.Epsilon) return;
            //         // // _targetRotation.x = _xRotation;
            //         // // _firstPersonCam.transform.eulerAngles = _targetRotation;
            //         // _firstPersonCam.transform.localRotation = Quaternion.AngleAxis(-_mouseY, _firstPersonCam.transform.right);
            //         break;
            //     case CameraChanger.CameraModes.ThirdPerson:
            //         var cameraPos = _thirdPersonCam.transform.position;
            //         var playerPos = Character.PlayerTransform.position;
            //         var viewDir = playerPos - new Vector3(cameraPos.x, playerPos.y, cameraPos.z);
            //         Character.PlayerTransform.forward = viewDir.normalized;
            //         var inputDir = 
            //             Character.PlayerTransform.forward * _mouseInput.x + 
            //             Character.PlayerTransform.right * _mouseInput.y;
            //         if (inputDir != Vector3.zero)
            //             Character.playerMesh.transform.forward = Vector3.Slerp(Character.playerMesh.transform.forward,
            //                 inputDir.normalized, Time.deltaTime * Character.RotationSpeed);
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
        }


        public virtual void Tick(float deltaTime)
        {
            
        }

        public virtual void Exit()
        {
        }
        
    }
}