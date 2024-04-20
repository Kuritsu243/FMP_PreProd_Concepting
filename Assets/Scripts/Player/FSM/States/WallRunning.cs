using System;
using Cameras;
using Unity.Cinemachine;
using UnityEngine;

namespace Player.FSM.States
{
    public class WallRunning : FsmState
    {
        private bool isGrounded;
        private bool isMoving;
        private bool isWallRunning;
        private bool isJumping;
        private bool isSliding;
        private Vector2 mouseInput;
        private Vector2 movementInput;
        private Vector3 playerVelocity;
        private Vector3 verticalVelocity;
        private float _mouseX;
        private float _mouseY;
        private float _xRotation;
        private float playerSpeed;
        private float gravityValue;
        private Vector3 _targetRotation;
        private CinemachineCamera thirdPersonCam;
        private CinemachineCamera firstPersonCam;
        private LayerMask _whatIsWall;
        private float wallRunForce;
        private float wallRunMaxDuration;
        private float wallRunExitTime;
        private float maxWallDistance;
        private float wallRunSpeed;
        private RaycastHit _leftWallHit;
        private RaycastHit _rightWallHit;
        private bool _leftWall;
        private bool _rightWall;
        private bool _isExitingWall;
        private bool _exitWallTimerExceeded;
        private bool _exitWallTimerActive;
        private bool _canWallRun;
        private float _maxWallDistance;
        
        private Transform PlayerTransform => Character.PlayerTransform;
        


        public WallRunning(string stateName, PlayerController playerController, FiniteStateMachine stateMachine) : base(stateName, stateMachine, playerController)
        {
            StateName = stateName;
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            isMoving = true;
            isWallRunning = true;
            isSliding = false;
            isGrounded = false;
            playerSpeed = Character.PlayerSpeed;
            gravityValue = Character.PlayerGravity;
            wallRunForce = Character.WallRunForce;
            wallRunSpeed = Character.WallRunSpeed;
            _maxWallDistance = Character.MaxWallDistance;
            _whatIsWall = Character.WhatIsWall;

            if (Character.leftWall)
            {
                _leftWallHit = Character.LeftWallHit;
                _leftWall = true;
            }
            else if (Character.rightWall)
            {
                _rightWallHit = Character.RightWallHit;
                _rightWall = true;
            }
        }

        public override void HandleInput()
        {
            base.HandleInput();
            
            if (movementInput is {x: 0, y: 0})
                isMoving = false;

            isJumping = JumpAction.IsPressed();
            movementInput = MoveAction.ReadValue<Vector2>();
            playerVelocity = (PlayerTransform.right * movementInput.x +
                              PlayerTransform.forward * movementInput.y) * playerSpeed;
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!_leftWall && !_rightWall && !isGrounded)
                StateMachine.ChangeState(Character.AirborneState);
            if (isJumping && Character.canWallJump)
                StateMachine.ChangeState(Character.WallJumpingState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            isGrounded = Character.isGrounded;
            // Character.characterController.Move(playerVelocity * Time.deltaTime + verticalVelocity * Time.deltaTime);
            // if (!isGrounded) verticalVelocity.y += gravityValue * Time.deltaTime;

            Debug.Log($"Right Wall detected: {_rightWall}\n" +
                      $"Left Wall Detected: {_leftWall}");
            
            var right = PlayerTransform.right;
            var position = PlayerTransform.position;
            _rightWall = Physics.Raycast(position, right, out _rightWallHit, _maxWallDistance, _whatIsWall);
            _leftWall = Physics.Raycast(position, -right, out _leftWallHit, _maxWallDistance, _whatIsWall);

            if (_rightWall)
                _leftWall = false;
            else if (_leftWall)
                _rightWall = false;
            

            if (_rightWall && _leftWall)
                Debug.LogError("You've fucked it lmao");
            
            if (!_leftWall && !_rightWall) return;
   
            // // todo: add camera effects 
            // MainCamera.DoFov(120f, 0.15f);
            //
            // if (_leftWall)
            //     MainCamera.DoTilt(-5f, 0.15f);
            // else if (_rightWall)
            //     MainCamera.DoTilt(5f, 0.15f);
            
            var wallNormal = _rightWall ? _rightWallHit.normal : _leftWallHit.normal;
            var wallForward = Vector3.Cross(wallNormal, PlayerTransform.up);
            if ((PlayerTransform.forward - wallForward).magnitude > (PlayerTransform.forward - -wallForward).magnitude)
                wallForward = -wallForward;
            Character.characterController.Move(wallForward * (wallRunForce * Time.deltaTime));
            
            switch (_leftWall)
            {
                case true or true when movementInput is not {x: 0, y: 0} && !isGrounded:
                {
                    if (_leftWall)
                    {
                        if (Character.IsTutorial && !TutorialController.WallRunChecks["FirstWall"])
                            TutorialController.WallRunChecks["FirstWall"] = true;
                        Character.leftWall = true;
                        Character.LeftWallHit = _leftWallHit;
                    }
                    else if (_rightWall)
                    {
                        if (Character.IsTutorial && TutorialController.WallRunChecks["FirstWall"] &&
                            !TutorialController.WallRunChecks["SecondWall"])
                            TutorialController.WallRunChecks["SecondWall"] = true;
                        Character.rightWall = true;
                        Character.RightWallHit = _rightWallHit;
                    }
                    _canWallRun = true;
                    break;
                }
                case false when _rightWall && movementInput is not {x: 0, y: 0}:
                    if (Character.IsTutorial && TutorialController.WallRunChecks["FirstWall"] &&
                        !TutorialController.WallRunChecks["SecondWall"])
                        TutorialController.WallRunChecks["SecondWall"] = true;
                    Character.characterController.Move(-wallNormal * (100 * Time.deltaTime));
                    break;
            }




        }

        public override void Exit()
        {
            base.Exit();
            // MainCamera.DoFov(90f);
            // MainCamera.DoTilt(0f);
            isWallRunning = false;
            if (!isJumping) return;
            if (_leftWall && !_rightWall)
            {
                Character.jumpingFromLeftWall = true;
                Character.jumpingFromRightWall = false;
                Character.rightWall = false;
                Character.leftWall = false;
                Character.JumpingLeftWallHit = _leftWallHit;
            }
            else if (_rightWall && !_leftWall)
            {
                Character.jumpingFromRightWall = true;
                Character.jumpingFromLeftWall = false;
                Character.leftWall = false;
                Character.rightWall = false;
                Character.JumpingRightWallHit = _rightWallHit;
            }
            
        }
    }
}
