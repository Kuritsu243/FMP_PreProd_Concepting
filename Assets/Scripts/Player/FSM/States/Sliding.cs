using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Player.FSM.States
{
    public class Sliding : FsmState
    {

        private float _maxSlideTime;
        private float _slideForce;
        private float _slideYScale;
        private Transform _playerTransform;
        private float _playerSpeed;
        private Vector2 _movementInput;
        private Vector3 _playerVelocity;
        private Vector3 _slideVelocity;
        private bool _isSliding;
        private bool _isMoving;
        private bool _isGrounded;
        private bool _isJumping;
        private Coroutine slideTimer;
        private float _startYScale;
        public Sliding(string stateName, PlayerController playerController, FiniteStateMachine stateMachine) : base(
            stateName, stateMachine, playerController)
        {
            StateName = stateName;
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            _isSliding = true;
            _isMoving = true;
            _isGrounded = true;
            _isJumping = false;
            _maxSlideTime = Character.MaxSlideTime;
            _slideForce = Character.SlideForce;
            _slideYScale = Character.SlideYScale;
            _playerTransform = Character.PlayerTransform;
            _playerSpeed = Character.PlayerSpeed;
            slideTimer = Character.StartCoroutine(SlideTimer());
            _startYScale = _playerTransform.localScale.y;
            
            var localScale = _playerTransform.localScale;
            localScale = new Vector3(localScale.x, _slideYScale, localScale.y);
            _playerTransform.localScale = localScale;
        }

        public override void HandleInput()
        {
            base.HandleInput();

            _movementInput = MoveAction.ReadValue<Vector2>();
            _playerVelocity = (_playerTransform.right * _movementInput.x +
                               _playerTransform.forward * _movementInput.y) * _playerSpeed;


            _isJumping = JumpAction.IsPressed();
            if (_movementInput is {x: 0, y: 0})
                _isMoving = false;

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            
            switch (_isSliding)
            {
                case false when _movementInput is {x: 0, y: 0}:
                    StateMachine.ChangeState(Character.IdleState);
                    break;
                case false when _movementInput is not {x: 0, y: 0}:
                    StateMachine.ChangeState(Character.WalkingState);
                    break;
            }
            if (_isJumping)
                StateMachine.ChangeState(Character.JumpingState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _slideVelocity = _playerVelocity.normalized * _slideForce;

            if (!_isSliding) return;
            Character.characterController.Move(_slideVelocity * Time.deltaTime);
        }

        public override void Exit()
        {
            base.Exit();
            _isSliding = false;
            Character.StopCoroutine(slideTimer);
            Character.canSlide = false;
            Character.StartCoroutine(Character.ActionCooldown(() => Character.canSlide = true, Character.SlideCooldown));
            var localScale = _playerTransform.localScale;
            localScale = new Vector3(localScale.x, _startYScale, localScale.z);
            _playerTransform.localScale = localScale;
        }

        private IEnumerator SlideTimer()
        {
            yield return new WaitForSeconds(_maxSlideTime);
            _isSliding = false;
        }
    }
}
