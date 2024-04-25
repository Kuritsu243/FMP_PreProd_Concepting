using Unity.Cinemachine;
using UnityEngine;

namespace Player.FSM.States
{
    public class Jumping : FsmState
    {
        private float _gravityValue;
        private float _playerSpeed;
        private float _playerJumpHeight;
        private bool _isGrounded;
        private Vector2 _mouseInput;
        private Vector2 _movementInput;
        private Vector3 _playerVelocity;
        private Vector3 _verticalVelocity;
        private float _mouseX;
        private float _mouseY;
        private float _xRotation;
        private float _maxWallDistance;
        private LayerMask _whatIsWall;
        private Vector3 _targetRotation;
        private CinemachineCamera _thirdPersonCam;
        private CinemachineCamera _firstPersonCam;
        private RaycastHit _leftWallHit;
        private RaycastHit _rightWallHit;
        private bool _leftWall;
        private bool _rightWall;
        private bool _canWallRun;
        private Transform PlayerTransform => Character.PlayerTransform;


        public Jumping(string stateName, PlayerController playerController, FiniteStateMachine stateMachine) : base(stateMachine, playerController)
        {
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            _canWallRun = false;
            _isGrounded = false;
            _playerSpeed = Character.PlayerSpeed;
            _gravityValue = Character.PlayerGravity;
            _playerJumpHeight = Character.JumpHeight;
            _verticalVelocity = Vector3.zero;
            _maxWallDistance = Character.MaxWallDistance;
            _whatIsWall = Character.WhatIsWall;
            Jump();
        }

        private void Jump()
        {
            _verticalVelocity.y = Mathf.Sqrt(-2f * _playerJumpHeight * _gravityValue);
        }

        public override void HandleInput()
        {
            base.HandleInput();


            SlideAction.IsPressed();
            _movementInput = MoveAction.ReadValue<Vector2>();
            _playerVelocity = (PlayerTransform.right * _movementInput.x +
                               PlayerTransform.forward * _movementInput.y) * _playerSpeed;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            switch (_isGrounded)
            {
                case false when (_leftWall || _rightWall) && _canWallRun:
                    StateMachine.ChangeState(Character.WallRunState);
                    break;
                case true when _movementInput is not { x: 0, y: 0 } && _verticalVelocity.y < 0:
                    StateMachine.ChangeState(Character.IdleState);
                    break;
                case true when _movementInput is { x: 0, y: 0 } && _verticalVelocity.y < 0:
                    StateMachine.ChangeState(Character.WalkingState);
                    break;
                case false when _verticalVelocity.y <= 0:
                    StateMachine.ChangeState(Character.AirborneState);
                    break;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _isGrounded = Character.isGrounded;
            Character.characterController.Move(_playerVelocity * Time.deltaTime + _verticalVelocity * Time.deltaTime);
            if (!_isGrounded) _verticalVelocity.y += _gravityValue * Time.deltaTime;


            var right = PlayerTransform.right;
            var position = PlayerTransform.position;
            _rightWall = Physics.Raycast(position, right, out _rightWallHit, _maxWallDistance, _whatIsWall);
            _leftWall = Physics.Raycast(position, -right, out _leftWallHit, _maxWallDistance, _whatIsWall);

            if ((!_leftWall && !_rightWall) || _movementInput is { x: 0, y: 0 } || _isGrounded) return;
            if (_leftWall && !_rightWall)
            {
                Character.leftWall = true;
                Character.rightWall = false;
                Character.LeftWallHit = _leftWallHit;
            }
            else if (_rightWall && !_leftWall)
            {
                Character.rightWall = true;
                Character.leftWall = false;
                Character.RightWallHit = _rightWallHit;
            }

            _canWallRun = true;
        }

        public override void Exit()
        {
            base.Exit();
            _isGrounded = true;

            if (_leftWall && !_rightWall)
            {
                Character.leftWall = true;
                Character.rightWall = false;
                Character.LeftWallHit = _leftWallHit;
            }
            else if (_rightWall && !_leftWall)
            {
                Character.rightWall = true;
                Character.leftWall = false;
                Character.RightWallHit = _rightWallHit;
            }
        }
    }
}