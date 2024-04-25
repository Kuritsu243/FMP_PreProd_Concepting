using Unity.Cinemachine;
using UnityEngine;

namespace Player.FSM.States
{
    public class WallRunning : FsmState
    {
        private bool _isGrounded;
        private bool _isJumping;
        private Vector2 _mouseInput;
        private Vector2 _movementInput;
        private Vector3 _verticalVelocity;
        private float _mouseX;
        private float _mouseY;
        private float _xRotation;
        private Vector3 _targetRotation;
        private CinemachineCamera _thirdPersonCam;
        private CinemachineCamera _firstPersonCam;
        private LayerMask _whatIsWall;
        private float _wallRunForce;
        private float _wallRunMaxDuration;
        private float _wallRunExitTime;
        private float _wallRunSpeed;
        private RaycastHit _leftWallHit;
        private RaycastHit _rightWallHit;
        private bool _leftWall;
        private bool _rightWall;
        private bool _isExitingWall;
        private bool _exitWallTimerExceeded;
        private bool _exitWallTimerActive;
        private float _maxWallDistance;

        private Transform PlayerTransform => Character.PlayerTransform;


        public WallRunning(PlayerController playerController, FiniteStateMachine stateMachine) : base(stateMachine,
            playerController)
        {
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            _isGrounded = false;
            _wallRunForce = Character.WallRunForce;
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

            _isJumping = JumpAction.IsPressed();
            _movementInput = MoveAction.ReadValue<Vector2>();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!_leftWall && !_rightWall && !_isGrounded)
                StateMachine.ChangeState(Character.AirborneState);
            if (_isJumping && Character.canWallJump)
                StateMachine.ChangeState(Character.WallJumpingState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _isGrounded = Character.isGrounded;
            var right = PlayerTransform.right;
            var position = PlayerTransform.position;
            _rightWall = Physics.Raycast(position, right, out _rightWallHit, _maxWallDistance, _whatIsWall);
            _leftWall = Physics.Raycast(position, -right, out _leftWallHit, _maxWallDistance, _whatIsWall);

            if (_rightWall)
                _leftWall = false;
            else if (_leftWall)
                _rightWall = false;


            if (_rightWall && _leftWall)
                Debug.LogError("Both walls have been detected. This is logically not meant to happen.");

            if (!_leftWall && !_rightWall) return;

            var wallNormal = _rightWall ? _rightWallHit.normal : _leftWallHit.normal;
            var wallForward = Vector3.Cross(wallNormal, PlayerTransform.up);
            if ((PlayerTransform.forward - wallForward).magnitude > (PlayerTransform.forward - -wallForward).magnitude)
                wallForward = -wallForward;
            Character.characterController.Move(wallForward * (_wallRunForce * Time.deltaTime));

            switch (_leftWall)
            {
                case true or true when _movementInput is not { x: 0, y: 0 } && !_isGrounded:
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
                    
                    break;
                }
                case false when _rightWall && _movementInput is not { x: 0, y: 0 }:
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
            if (!_isJumping) return;
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