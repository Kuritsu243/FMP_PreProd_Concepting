using UnityEngine;

namespace Player.FSM.States
{
    public class WallJumping : FsmState
    {
        private float _gravityValue;
        private float _playerSpeed;
        private float _playerJumpHeight;
        private float _wallJumpSideForce;
        private float _wallJumpUpForce;
        private bool _isJumping;
        private bool _isGrounded;
        private bool _isWallRunning;
        private Vector2 _movementInput;
        private Vector3 _playerVelocity;
        private Vector3 _verticalVelocity;
        private RaycastHit _leftWallHit;
        private RaycastHit _rightWallHit;
#pragma warning disable CS0414 // Field is assigned but its value is never used
        private bool _leftWall;
#pragma warning restore CS0414 // Field is assigned but its value is never used
        private bool _rightWall;
        private Transform PlayerTransform => Character.PlayerTransform;

        public WallJumping(PlayerController playerController, FiniteStateMachine stateMachine) : base(
            stateMachine, playerController)
        {
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            
            _isGrounded = false;
            _playerSpeed = Character.PlayerSpeed;
            _gravityValue = Character.PlayerGravity;
            _wallJumpUpForce = Character.WallJumpUpForce;
            _wallJumpSideForce = Character.WallJumpSideForce;
            
            
            if (Character.jumpingFromRightWall && !Character.jumpingFromLeftWall)
            {
                _rightWall = true;
                _leftWall = false;
                _rightWallHit = Character.JumpingRightWallHit;
            }
            else if (Character.jumpingFromLeftWall && !Character.jumpingFromRightWall)
            {
                _leftWall = true;
                _rightWall = false;
                _leftWallHit = Character.JumpingLeftWallHit;
            }
            WallJump();
        }

        public override void HandleInput()
        {
            base.HandleInput();

            _movementInput = MoveAction.ReadValue<Vector2>();
            _playerVelocity = (PlayerTransform.right * _movementInput.x +
                              PlayerTransform.forward * _movementInput.y) * _playerSpeed;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _isGrounded = Character.isGrounded;
            Character.characterController.Move(_playerVelocity * Time.deltaTime + _verticalVelocity * Time.deltaTime);
            if (!_isGrounded) _verticalVelocity.y += _gravityValue * Time.deltaTime;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (_verticalVelocity.y <= 0)
                StateMachine.ChangeState(Character.AirborneState);
        }

        private void WallJump()
        {
            var wallNormal = _rightWall ? _rightWallHit.normal : _leftWallHit.normal;
            var playerForceToApply = PlayerTransform.up * _wallJumpUpForce + wallNormal * _wallJumpSideForce;
            _verticalVelocity = playerForceToApply;
            
        }

        public override void Exit()
        {
            base.Exit();
            Character.canWallJump = false;
            Character.StartCoroutine(PlayerController.ActionCooldown(() => Character.canWallJump = true,
                Character.WallJumpCooldown));
            Character.canJump = false;
            Character.StartCoroutine(PlayerController.ActionCooldown(() => Character.canJump = true,
                Character.JumpCooldown));
            Character.checkForWallsWhenAirborne = true;
            _leftWall = false;
            _rightWall = false;
        }
    }
}
