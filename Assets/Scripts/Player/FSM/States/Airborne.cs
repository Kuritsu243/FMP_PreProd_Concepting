using UnityEngine;

namespace Player.FSM.States
{
    public class Airborne : FsmState
    {
        private float _gravityValue;
        private float _playerSpeed;
        private bool _isGrounded;
        private bool _checkForWalls;
        private bool _leftWall;
        private bool _rightWall;
        private bool _canWallRun;
        private RaycastHit _leftWallHit;
        private RaycastHit _rightWallHit;
        private Vector2 _movementInput;
        private Vector3 _playerVelocity;
        private Vector3 _verticalVelocity;
        private LayerMask _whatIsWall;
        private float _maxWallDistance;
        private Transform PlayerTransform => Character.PlayerTransform;

        public Airborne(string stateName, PlayerController playerController, FiniteStateMachine stateMachine) : base(stateMachine, playerController)
        {
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            _isGrounded = false;
            _leftWall = false;
            _rightWall = false;
            _playerSpeed = Character.PlayerSpeed;
            _gravityValue = Character.PlayerGravity;
            _verticalVelocity = Vector3.zero;
            _checkForWalls = Character.checkForWallsWhenAirborne;
            _whatIsWall = Character.WhatIsWall;
            _maxWallDistance = Character.MaxWallDistance;

        }

        public override void HandleInput()
        {
            base.HandleInput();
            _movementInput = MoveAction.ReadValue<Vector2>();
            _playerVelocity = (PlayerTransform.right * _movementInput.x +
                              PlayerTransform.forward * _movementInput.y) * _playerSpeed;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            switch (_isGrounded)
            {
                case true when _movementInput is not {x: 0, y: 0}:
                    StateMachine.ChangeState(Character.WalkingState);
                    break;
                case true when _movementInput is {x: 0, y:0}:
                    StateMachine.ChangeState(Character.IdleState);
                    break;
                case false when (_leftWall || _rightWall) && _canWallRun:
                    StateMachine.ChangeState(Character.WallRunState);
                    break;
            }
            
            
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            _isGrounded = Character.isGrounded;
            Character.characterController.Move(_playerVelocity * Time.deltaTime + _verticalVelocity * Time.deltaTime);
            if (!_isGrounded) _verticalVelocity.y += _gravityValue * Time.deltaTime;
            if (!_checkForWalls) return;
            var right = PlayerTransform.right;
            var position = PlayerTransform.position;
            _rightWall = Physics.Raycast(position, right, out _rightWallHit, _maxWallDistance, _whatIsWall);
            _leftWall = Physics.Raycast(position, -right, out _leftWallHit, _maxWallDistance, _whatIsWall);

            if ((!_leftWall && !_rightWall) || _movementInput is {x: 0, y: 0} || _isGrounded) return;
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
            Character.canJump = false;
            Character.StartCoroutine(PlayerController.ActionCooldown(() => Character.canJump = true, Character.JumpCooldown));
            Character.StartCoroutine(PlayerController.ActionCooldown(() => { },
                Character.WallRunCooldown));
            Character.jumpingFromLeftWall = false;
            Character.jumpingFromRightWall = false;
            Character.checkForWallsWhenAirborne = false;
        }
    }
}
