using System;
using Camera;
using Cinemachine;
using UnityEngine;

namespace Player.FSM.States
{
    public class Jumping : FsmState
    {
        private float gravityValue;
        private float playerSpeed;
        private float playerJumpHeight;
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
        private float _maxWallDistance;
        private LayerMask _whatIsWall;
        private Vector3 _targetRotation;
        private CinemachineFreeLook thirdPersonCam;
        private CinemachineVirtualCamera firstPersonCam;
        private RaycastHit _leftWallHit;
        private RaycastHit _rightWallHit;
        private bool _leftWall;
        private bool _rightWall;
        private bool _canWallRun;
        private Transform PlayerTransform => Character.PlayerTransform;


        
        
        public Jumping(string stateName, PlayerController playerController, FiniteStateMachine stateMachine) : base(stateName, stateMachine, playerController)
        {
            StateName = stateName;
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();


            _canWallRun = false;
            isMoving = true;
            isJumping = true;
            isSliding = false;
            isGrounded = false;
            playerSpeed = Character.PlayerSpeed;
            gravityValue = Character.PlayerGravity;
            playerJumpHeight = Character.JumpHeight;
            verticalVelocity = Vector3.zero;
            _maxWallDistance = Character.MaxWallDistance;
            _whatIsWall = Character.WhatIsWall;
            
            
            Jump();
        }

        private void Jump()
        {
            verticalVelocity.y = Mathf.Sqrt(-2f * playerJumpHeight * gravityValue);
        }

        public override void HandleInput()
        {
            base.HandleInput();
            
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

            switch (isGrounded)
            {
                case false when (_leftWall || _rightWall) && _canWallRun:
                    StateMachine.ChangeState(Character.wallRunState);
                    break;
                case true when movementInput is not {x: 0, y: 0} && verticalVelocity.y < 0:
                    StateMachine.ChangeState(Character.IdleState);
                    break;
                case true when movementInput is {x: 0, y: 0} && verticalVelocity.y < 0:
                    StateMachine.ChangeState(Character.walkingState);
                    break; 
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            isGrounded = Character.isGrounded;
            Character.characterController.Move(playerVelocity * Time.deltaTime + verticalVelocity * Time.deltaTime);
            if (!isGrounded) verticalVelocity.y += gravityValue * Time.deltaTime;


            var right = PlayerTransform.right;
            var position = PlayerTransform.position;
            _rightWall = Physics.Raycast(position, right, out _rightWallHit, _maxWallDistance, _whatIsWall);
            _leftWall = Physics.Raycast(position, -right, out _leftWallHit, _maxWallDistance, _whatIsWall);

            if ((_leftWall || _rightWall) && movementInput is not {x: 0, y: 0} && !isGrounded)
            {
                if (_leftWall)
                {
                    Character.leftWall = true;
                    Character.leftWallHit = _leftWallHit;
                }
                else if (_rightWall)
                {
                    Character.rightWall = true;
                    Character.rightWallHit = _rightWallHit;
                }
                _canWallRun = true;
            }
            
            if (mouseInput is {x: 0, y: 0}) return;
            CameraChanger.GetActiveCams(out thirdPersonCam, out firstPersonCam);
            switch (MainCamera.ActiveCameraMode)
            {
                case CameraChanger.CameraModes.FirstPerson:
                    Character.playerMesh.transform.Rotate(Vector3.up, _mouseX * Time.deltaTime);
                    _xRotation -= _mouseY;
                    _xRotation = Mathf.Clamp(_xRotation, -Character.XClamp, Character.XClamp);
                    _targetRotation = Character.playerMesh.transform.eulerAngles;
                    _targetRotation.x = _xRotation;
                    firstPersonCam.transform.eulerAngles = _targetRotation;
                    break;
                case CameraChanger.CameraModes.ThirdPerson:
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
            isGrounded = true;
            isSliding = false;
            isJumping = false;
            
        }
    }
}
