using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Camera;
using Cinemachine;

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
        protected Vector2 MouseInput;
        protected float MouseX;
        protected float MouseY;
        protected CinemachineFreeLook ThirdPersonCam;
        protected CinemachineVirtualCamera FirstPersonCam;
        protected float XRotation;
        protected Vector3 TargetRotation;
        protected float XClamp;

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
            MouseInput = LookAction.ReadValue<Vector2>();
            MouseX = MouseInput.x * Character.MouseSensitivity.x;
            MouseY = MouseInput.y * Character.MouseSensitivity.y;
        }

        public virtual void LogicUpdate()
        {
            
        }

        public virtual void PhysicsUpdate()
        {
            CameraSwitcher.GetActiveCams(out ThirdPersonCam, out FirstPersonCam);
            switch (MainCamera.ActiveCameraMode)
            {
                case CameraSwitcher.CameraModes.FirstPerson:
                    if (MouseInput is {x: 0, y: 0}) return;
                    if (TargetRotation == Vector3.zero) return;
                    if (XRotation == 0) return;
                    Character.playerMesh.transform.Rotate(Vector3.up, MouseX * Time.deltaTime);
                    XRotation -= MouseY;
                    XRotation = Mathf.Clamp(XRotation, -Character.XClamp, Character.XClamp);
                    TargetRotation = Character.playerMesh.transform.eulerAngles;
                    TargetRotation.x = XRotation;
                    FirstPersonCam.transform.eulerAngles = TargetRotation;
                    break;
                case CameraSwitcher.CameraModes.ThirdPerson:
                    var cameraPos = ThirdPersonCam.transform.position;
                    var playerPos = Character.PlayerTransform.position;
                    var viewDir = playerPos - new Vector3(cameraPos.x, playerPos.y, cameraPos.z);
                    Character.PlayerTransform.forward = viewDir.normalized;
                    var inputDir = 
                        Character.PlayerTransform.forward * MouseInput.x + 
                        Character.PlayerTransform.right * MouseInput.y;
                    if (inputDir != Vector3.zero)
                        Character.playerMesh.transform.forward = Vector3.Slerp(Character.playerMesh.transform.forward,
                            inputDir.normalized, Time.deltaTime * Character.RotationSpeed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public virtual void Tick(float deltaTime)
        {
            
        }

        public virtual void Exit()
        {
            StateMachine.CurrentState.TargetRotation = StateMachine.PreviousState.TargetRotation;
            StateMachine.CurrentState.XRotation = StateMachine.PreviousState.XRotation;
            Debug.Log($"Entered State: {StateMachine.CurrentState}\n" +
                      $"from State: {StateMachine.PreviousState}\n" +
                      $"Previous State Target Rotation: {StateMachine.PreviousState.TargetRotation}\n" +
                      $"New State Target Rotation: {StateMachine.CurrentState.TargetRotation}");
        }
        
    }
}