using Camera;
using UnityEngine;

namespace Player.FSM.States
{
    public class Airborne : FsmState
    {
        private float gravityValue;
        private float playerSpeed;
        private bool isMoving;
        private bool isGrounded;
        private bool checkForWalls;
        private bool _leftWall;
        private bool _rightWall;
        private bool canWallRun;
        private RaycastHit _leftWallHit;
        private RaycastHit _rightWallHit;
        private Vector2 movementInput;
        private Vector3 playerVelocity;
        private Vector3 verticalVelocity;
        private LayerMask _whatIsWall;
        private float _maxWallDistance;
        private Transform PlayerTransform => Character.PlayerTransform;

        public Airborne(string stateName, PlayerController playerController, FiniteStateMachine stateMachine) : base(stateName, stateMachine, playerController)
        {
            StateName = stateName;
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            isGrounded = false;
            isMoving = true;
            _leftWall = false;
            _rightWall = false;
            playerSpeed = Character.PlayerSpeed;
            gravityValue = Character.PlayerGravity;
            verticalVelocity = Vector3.zero;
            checkForWalls = Character.checkForWallsWhenAirborne;
            _whatIsWall = Character.WhatIsWall;
            _maxWallDistance = Character.MaxWallDistance;
            MainCamera.DoFov(90f);
            MainCamera.DoTilt(0f);
        }

        public override void HandleInput()
        {
            base.HandleInput();
            if (movementInput is {x: 0, y: 0})
                isMoving = false;
            movementInput = MoveAction.ReadValue<Vector2>();
            playerVelocity = (PlayerTransform.right * movementInput.x +
                              PlayerTransform.forward * movementInput.y) * playerSpeed;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            switch (isGrounded)
            {
                case true when movementInput is not {x: 0, y: 0}:
                    StateMachine.ChangeState(Character.WalkingState);
                    break;
                case true when movementInput is {x: 0, y:0}:
                    StateMachine.ChangeState(Character.IdleState);
                    break;
                case false when (_leftWall || _rightWall) && canWallRun:
                    StateMachine.ChangeState(Character.WallRunState);
                    break;
            }
            
            
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            isGrounded = Character.isGrounded;
            Character.characterController.Move(playerVelocity * Time.deltaTime + verticalVelocity * Time.deltaTime);
            if (!isGrounded) verticalVelocity.y += gravityValue * Time.deltaTime;

            if (!checkForWalls) return;
            
            var right = PlayerTransform.right;
            var position = PlayerTransform.position;
            _rightWall = Physics.Raycast(position, right, out _rightWallHit, _maxWallDistance, _whatIsWall);
            _leftWall = Physics.Raycast(position, -right, out _leftWallHit, _maxWallDistance, _whatIsWall);

            if ((!_leftWall && !_rightWall) || movementInput is {x: 0, y: 0} || isGrounded) return;
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
            canWallRun = true;
        }

        public override void Exit()
        {
            base.Exit();
            isGrounded = true;
            Character.canJump = false;
            Character.StartCoroutine(Character.ActionCooldown(() => Character.canJump = true, Character.JumpCooldown));
            Character.StartCoroutine(Character.ActionCooldown(() => Character.canWallRun = true,
                Character.WallRunCooldown));
            Character.jumpingFromLeftWall = false;
            Character.jumpingFromRightWall = false;
            Character.checkForWallsWhenAirborne = false;
        }
    }
}
