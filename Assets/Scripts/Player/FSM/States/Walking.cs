using System;
using Camera;
using Cinemachine;
using UnityEngine;

namespace Player.FSM.States
{
    public class Walking : FsmState
    {
        private float gravityValue;
        private float playerSpeed;
        private bool isJumping;
        private bool isSliding;
        private bool isGrounded;
        private bool isMoving;
        private Vector2 movementInput;
        private Vector3 playerVelocity;
        private Vector3 verticalVelocity;
        private Transform PlayerTransform => Character.PlayerTransform;


        
        
        public Walking(string stateName, PlayerController playerController, FiniteStateMachine stateMachine) : base(stateName, stateMachine, playerController)
        {
            StateName = stateName;
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();


            isMoving = true;
            isJumping = false;
            isSliding = false;
            isGrounded = true;
            playerSpeed = Character.PlayerSpeed;
            gravityValue = Character.PlayerGravity;
        }

        public override void HandleInput()
        {
            base.HandleInput();

            isJumping = JumpAction.IsPressed();
            isSliding = SlideAction.IsPressed();
            
            if (movementInput is {x: 0, y: 0})
                isMoving = false;
            movementInput = MoveAction.ReadValue<Vector2>();
            playerVelocity = (PlayerTransform.right * movementInput.x +
                              PlayerTransform.forward * movementInput.y) * playerSpeed;
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            
            if (isJumping && Character.canJump)
                StateMachine.ChangeState(Character.JumpingState);
            if (!isMoving)
                StateMachine.ChangeState(Character.IdleState);
            if (isSliding && Character.canSlide)
                StateMachine.ChangeState(Character.SlidingState);

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            
            verticalVelocity.y += gravityValue * Time.deltaTime;
            isGrounded = Character.isGrounded;

            if (isGrounded && verticalVelocity.y < 0)
                verticalVelocity.y = 0f;

            Character.characterController.Move(playerVelocity * Time.deltaTime + verticalVelocity * Time.deltaTime);
        }

        public override void Exit()
        {
            base.Exit();

            
        }
    }
}
