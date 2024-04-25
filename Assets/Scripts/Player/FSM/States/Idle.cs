using Tutorial;
using UnityEngine;

namespace Player.FSM.States
{
    public class Idle : FsmState
    {
        private bool _isJumping;
        private bool _isSliding;
        private bool _isGrounded;
        private bool _isMoving;
        private Vector2 _movementInput;
        private Vector3 _playerVelocity;
        private Vector3 _verticalVelocity;
        private Transform PlayerTransform => Character.PlayerTransform;


        public Idle(string name, PlayerController playerController, FiniteStateMachine stateMachine) : base(stateMachine, playerController)
        {
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            _isMoving = false;
            _isJumping = false;
            _isSliding = false;
            _isGrounded = true;
            _playerVelocity = Vector3.zero;
            _verticalVelocity = Vector3.zero;
        }

        public override void Tick(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleInput()
        {
            base.HandleInput();
            _isJumping = JumpAction.IsPressed();
            if (_isJumping && Character.IsTutorial && TutorialController.IntroComplete() &&
                TutorialController.nextKeyToPress == TutorialController.NextKeyPress.Jump)
                TutorialController.TutorialChecks["Jump"] = true;
            if (_movementInput is not { x: 0, y: 0 })
            {
                _isMoving = true;
                if (!Character.IsTutorial) return;
                switch (_movementInput)
                {
                    case { x: 0, y: > 0 }:
                        if (!TutorialController.IntroComplete() && TutorialController.nextKeyToPress ==
                            TutorialController.NextKeyPress.Forward) break;
                        TutorialController.TutorialChecks["Forward"] = true;
                        break;
                    case { x: 0, y: < 0 }:
                        if (!TutorialController.IntroComplete() && TutorialController.nextKeyToPress !=
                            TutorialController.NextKeyPress.Backwards) break;
                        TutorialController.TutorialChecks["Backwards"] = true;
                        break;
                    case { x: > 0, y: 0 }:
                        if (!TutorialController.IntroComplete() && TutorialController.nextKeyToPress !=
                            TutorialController.NextKeyPress.Right) break;
                        TutorialController.TutorialChecks["Right"] = true;
                        break;
                    case { x: < 0, y: 0 }:
                        if (!TutorialController.IntroComplete() &&
                            TutorialController.nextKeyToPress != TutorialController.NextKeyPress.Left) break;
                        TutorialController.TutorialChecks["Left"] = true;
                        break;
                }
            }

            _movementInput = MoveAction.ReadValue<Vector2>();
            _playerVelocity = (PlayerTransform.right * _movementInput.x +
                               PlayerTransform.forward * _movementInput.y) * PlayerSpeed;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (_isJumping && Character.canJump)
                StateMachine.ChangeState(Character.JumpingState);
            if (_isMoving)
                StateMachine.ChangeState(Character.WalkingState);
            if (_isSliding && Character.canSlide)
                StateMachine.ChangeState(Character.SlidingState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            _verticalVelocity.y += GravityValue * Time.deltaTime;
            _isGrounded = Character.isGrounded;
            if (_isGrounded && _verticalVelocity.y < 0)
                _verticalVelocity.y = 0f;

            Character.characterController.Move(_playerVelocity * Time.deltaTime + _verticalVelocity * Time.deltaTime);
        }
    }
}