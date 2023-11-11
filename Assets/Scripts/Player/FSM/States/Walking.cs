using System;
using Camera;
using Cinemachine;
using UnityEngine;

namespace Player.FSM.States
{
    public class Walking : FsmState
    {
        private float gravityValue;
        private float playerSpeed;
        private bool isJumping;
        private bool isSliding;
        private bool isGrounded;
        private bool isMoving;
        private Vector2 mouseInput;
        private Vector2 movementInput;
        private Vector3 playerVelocity;
        private Vector3 verticalVelocity;
        private float _mouseX;
        private float _mouseY;
        private float _xRotation;
        private Vector3 _targetRotation;
        private CinemachineFreeLook thirdPersonCam;
        private CinemachineVirtualCamera firstPersonCam;
        private Transform PlayerTransform => Character.PlayerTransform;


        
        
        public Walking(string stateName, PlayerController playerController, FiniteStateMachine stateMachine) : base(stateName, stateMachine, playerController)
        {
            StateName = stateName;
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();


            isMoving = true;
            isJumping = false;
            isSliding = false;
            isGrounded = true;
            playerSpeed = Character.PlayerSpeed;
            gravityValue = Character.PlayerGravity;
        }

        public override void HandleInput()
        {
            base.HandleInput();
            
            if (JumpAction.triggered)
                isJumping = true;
            if (SlideAction.triggered)
                isSliding = true;
            if (movementInput is {x: 0, y: 0})
                isMoving = false;
            movementInput = MoveAction.ReadValue<Vector2>();
            playerVelocity = (PlayerTransform.right * movementInput.x +
                              PlayerTransform.forward * movementInput.y) * playerSpeed;

            mouseInput = LookAction.ReadValue<Vector2>();
            _mouseX = mouseInput.x * Character.MouseSensitivity.x;
            _mouseY = mouseInput.y * Character.MouseSensitivity.y;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            
            if (isJumping)
                StateMachine.ChangeState(Character.jumpingState);
            if (!isMoving)
                StateMachine.ChangeState(Character.IdleState);

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            
            verticalVelocity.y += gravityValue * Time.deltaTime;
            isGrounded = Character.isGrounded;

            if (isGrounded && verticalVelocity.y < 0)
                verticalVelocity.y = 0f;

            Character.characterController.Move(playerVelocity * Time.deltaTime + verticalVelocity * Time.deltaTime);

            
            if (mouseInput is {x: 0, y: 0}) return;
            CameraSwitcher.GetActiveCams(out thirdPersonCam, out firstPersonCam);
            switch (MainCamera.ActiveCameraMode)
            {
                case CameraSwitcher.CameraModes.FirstPerson:
                    Character.playerMesh.transform.Rotate(Vector3.up, _mouseX * Time.deltaTime);
                    _xRotation -= _mouseY;
                    _xRotation = Mathf.Clamp(_xRotation, -Character.XClamp, Character.XClamp);
                    _targetRotation = Character.playerMesh.transform.eulerAngles;
                    _targetRotation.x = _xRotation;
                    firstPersonCam.transform.eulerAngles = _targetRotation;
                    break;
                case CameraSwitcher.CameraModes.ThirdPerson:
                    var cameraPos = thirdPersonCam.transform.position;
                    var playerPos = PlayerTransform.position;
                    var viewDir = playerPos - new Vector3(cameraPos.x, playerPos.y, cameraPos.z);
                    PlayerTransform.forward = viewDir.normalized;
                    var inputDir = 
                        PlayerTransform.forward * mouseInput.x + 
                        PlayerTransform.right * mouseInput.y;
                    if (inputDir != Vector3.zero)
                        Character.playerMesh.transform.forward = Vector3.Slerp(Character.playerMesh.transform.forward,
                            inputDir.normalized, Time.deltaTime * Character.RotationSpeed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Exit()
        {
            base.Exit();

            _xRotation = 0;
            _targetRotation = Vector3.zero;
            
        }
    }
}
