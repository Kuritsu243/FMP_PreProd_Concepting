using UnityEngine;

namespace Player.FSM.States
{
    public class Walking : FsmState
    {
        private float _gravityValue;
        private float _playerSpeed;
        private bool _isJumping;
        private bool _isSliding;
        private bool _isGrounded;
        private bool _isMoving;
        private Vector2 _movementInput;
        private Vector3 _playerVelocity;
        private Vector3 _verticalVelocity;
        private Transform PlayerTransform => Character.PlayerTransform;


        public Walking(string stateName, PlayerController playerController, FiniteStateMachine stateMachine) : base(stateMachine, playerController)
        {
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();


            _isMoving = true;
            _isJumping = false;
            _isSliding = false;
            _isGrounded = true;
            _playerSpeed = Character.PlayerSpeed;
            _gravityValue = Character.PlayerGravity;
        }

        public override void HandleInput()
        {
            base.HandleInput();

            _isJumping = JumpAction.IsPressed();
            _isSliding = SlideAction.IsPressed();

            if (_movementInput is { x: 0, y: 0 })
                _isMoving = false;
            _movementInput = MoveAction.ReadValue<Vector2>();
            _playerVelocity = (PlayerTransform.right * _movementInput.x +
                               PlayerTransform.forward * _movementInput.y) * _playerSpeed;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();


            if (_isJumping && Character.canJump)
                StateMachine.ChangeState(Character.JumpingState);
            if (!_isMoving)
                StateMachine.ChangeState(Character.IdleState);
            if (_isSliding && Character.canSlide)
                StateMachine.ChangeState(Character.SlidingState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _verticalVelocity.y += _gravityValue * Time.deltaTime;
            _isGrounded = Character.isGrounded;

            if (_isGrounded && _verticalVelocity.y < 0)
                _verticalVelocity.y = 0f;

            Character.characterController.Move(_playerVelocity * Time.deltaTime + _verticalVelocity * Time.deltaTime);
        }
    }
}