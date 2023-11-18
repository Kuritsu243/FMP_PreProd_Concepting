using UnityEngine;

namespace Player.FSM.States
{
    public class WallJumping : FsmState
    {
        private float gravityValue;
        private float playerSpeed;
        private float playerJumpHeight;
        private float wallJumpSideForce;
        private float wallJumpUpForce;
        private bool isJumping;
        private bool isGrounded;
        private bool isMoving;
        private bool isWallRunning;
        private bool isExitingWall;
        private Vector2 movementInput;
        private Vector3 playerVelocity;
        private Vector3 verticalVelocity;
        private RaycastHit _leftWallHit;
        private RaycastHit _rightWallHit;
        private bool _leftWall;
        private bool _rightWall;
        private Transform PlayerTransform => Character.PlayerTransform;

        public WallJumping(string stateName, PlayerController playerController, FiniteStateMachine stateMachine) : base(
            stateName, stateMachine, playerController)
        {
            StateName = stateName;
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            isMoving = true;
            isGrounded = false;
            playerSpeed = Character.PlayerSpeed;
            gravityValue = Character.PlayerGravity;
            wallJumpUpForce = Character.WallJumpUpForce;
            wallJumpSideForce = Character.WallJumpSideForce;
            
            
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
            Debug.Log($"Left wall? {_leftWall}\n" +
                      $"Right Wall? {_rightWall}");
            WallJump();
        }

        public override void HandleInput()
        {
            base.HandleInput();

            movementInput = MoveAction.ReadValue<Vector2>();
            playerVelocity = (PlayerTransform.right * movementInput.x +
                              PlayerTransform.forward * movementInput.y) * playerSpeed;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            isGrounded = Character.isGrounded;
            Character.characterController.Move(playerVelocity * Time.deltaTime + verticalVelocity * Time.deltaTime);
            if (!isGrounded) verticalVelocity.y += gravityValue * Time.deltaTime;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (verticalVelocity.y <= 0)
                StateMachine.ChangeState(Character.AirborneState);
        }

        private void WallJump()
        {
            isExitingWall = true;
            var wallNormal = _rightWall ? _rightWallHit.normal : _leftWallHit.normal;
            var playerForceToApply = PlayerTransform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;
            verticalVelocity = playerForceToApply;
            Debug.LogWarning($"Force to apply: {playerForceToApply}");
            
        }

        public override void Exit()
        {
            base.Exit();
            Character.canWallJump = false;
            Character.StartCoroutine(Character.ActionCooldown(() => Character.canWallJump = true,
                Character.WallJumpCooldown));
            Character.canJump = false;
            Character.StartCoroutine(Character.ActionCooldown(() => Character.canJump = true,
                Character.JumpCooldown));
            // if (Character.jumpingFromLeftWall)
            //     Character.jumpingFromLeftWall = false;
            // else if (Character.jumpingFromRightWall)
            //     Character.jumpingFromRightWall = false;
            Character.checkForWallsWhenAirborne = true;
            _leftWall = false;
            _rightWall = false;
        }
    }
}
