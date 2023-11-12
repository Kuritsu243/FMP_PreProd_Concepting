using System;
using Camera;
using Cinemachine;
using UnityEngine;

namespace Player.FSM.States
{
    public class Idle : FsmState
    {
        private float gravityValue;
        private float playerSpeed;
        private float _mouseX;
        private float _mouseY;
        private float _xRotation;
        private Vector3 _targetRotation;
        private bool isJumping;
        private bool isSliding;
        private bool isGrounded;
        private bool isMoving;
        private Vector2 mouseInput;
        private Vector2 movementInput;
        private Vector3 playerVelocity;
        private Vector3 verticalVelocity;
        private CinemachineFreeLook thirdPersonCam;
        private CinemachineVirtualCamera firstPersonCam;
        private Transform PlayerTransform => Character.PlayerTransform;
        

        public Idle(string name, PlayerController playerController, FiniteStateMachine stateMachine) : base("Idle", stateMachine, playerController)
        {
            StateName = name;
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            isMoving = false;
            isJumping = false;
            isSliding = false;
            isGrounded = true;
            verticalVelocity = Vector3.zero;
            playerSpeed = Character.PlayerSpeed;
            gravityValue = Character.PlayerGravity;
        }

        public void Execute()
        {
            throw new System.NotImplementedException();
        }

        public override void Tick(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            base.Exit();
            verticalVelocity = Vector3.zero;
            XRotation = 0;
            TargetRotation = Vector3.zero;
        }

        public override void HandleInput()
        {
            base.HandleInput();
            
            playerVelocity = (PlayerTransform.right * MovementInput.x + PlayerTransform.forward * MovementInput.y) *
                             PlayerSpeed;
            
            
            MouseInput = LookAction.ReadValue<Vector2>();
            MouseX = MouseInput.x * Character.MouseSensitivity.x;
            MouseY = MouseInput.y * Character.MouseSensitivity.y;

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (isJumping)
                StateMachine.ChangeState(Character.jumpingState);
            if (isMoving && !isJumping)
                StateMachine.ChangeState(Character.walkingState);

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            verticalVelocity.y += gravityValue * Time.deltaTime;
            isGrounded = Character.isGrounded;
            Character.characterController.Move(playerVelocity * Time.deltaTime + verticalVelocity * Time.deltaTime);
            
            
            Debug.Log($"XRotation: {XRotation},\n" +
                      $"TargetRotation: {TargetRotation},\n");
            CameraSwitcher.GetActiveCams(out ThirdPersonCam, out FirstPersonCam);
            switch (MainCamera.ActiveCameraMode)
            {
                case CameraSwitcher.CameraModes.FirstPerson:
                    Character.playerMesh.transform.Rotate(Vector3.up, MouseX * Time.deltaTime);
   
                    _xRotation -= MouseY;
                    _xRotation = Mathf.Clamp(_xRotation, -Character.XClamp, Character.XClamp);
                    TargetRotation = Character.playerMesh.transform.eulerAngles;
                    TargetRotation.x = _xRotation8;
                    FirstPersonCam.transform.eulerAngles = TargetRotation;
                    break;
                case CameraSwitcher.CameraModes.ThirdPerson:
                    var cameraPos = ThirdPersonCam.transform.position;
                    var playerPos = PlayerTransform.position;
                    var viewDir = playerPos - new Vector3(cameraPos.x, playerPos.y, cameraPos.z);
                    PlayerTransform.forward = viewDir.normalized;
                    var inputDir = 
                        PlayerTransform.forward * MouseInput.x + 
                        PlayerTransform.right * MouseInput.y;
                    if (inputDir != Vector3.zero)
                        Character.playerMesh.transform.forward = Vector3.Slerp(Character.playerMesh.transform.forward,
                            inputDir.normalized, Time.deltaTime * Character.RotationSpeed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
            CameraControl();
        }
        
        
        public void CameraControl()
        {

        }

    }
}
