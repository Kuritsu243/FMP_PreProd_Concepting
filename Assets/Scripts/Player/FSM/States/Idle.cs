using System;
using Cameras;
using Unity.Cinemachine;
using UnityEngine;

namespace Player.FSM.States
{
    public class Idle : FsmState
    {
        private bool isJumping;
        private bool isSliding;
        private bool isGrounded;
        private bool isMoving;
        private Vector2 movementInput;
        private Vector3 playerVelocity;
        private Vector3 verticalVelocity;
        private Transform PlayerTransform => Character.PlayerTransform;
        

        public Idle(string name, PlayerController playerController, FiniteStateMachine stateMachine) : base("Idle", stateMachine, playerController)
        {
            StateName = name;
            Character = playerController;
            StateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            isMoving = false;
            isJumping = false;
            isSliding = false;
            isGrounded = true;
            playerVelocity = Vector3.zero;
            verticalVelocity = Vector3.zero;
            
            // MainCamera.DoFov(90f, 0.25f);
            // MainCamera.DoTilt(0f, 0.25f);
        }

        public void Execute()
        {
            throw new System.NotImplementedException();
        }

        public override void Tick(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void HandleInput()
        {
            base.HandleInput();


            isJumping = JumpAction.IsPressed();
            
            if (Character.IsTutorial && isJumping)
                TutorialController.TutorialChecks["Jump"] = true;

            if (movementInput is not { x: 0, y: 0 })
            {
                isMoving = true;
                if (!Character.IsTutorial) return;
                switch (movementInput)
                {
                    case { x: 0, y: > 0 }:
                        TutorialController.TutorialChecks["Forward"] = true;
                        break;
                    case { x: 0, y: < 0 }:
                        TutorialController.TutorialChecks["Backwards"] = true;
                        break;
                    case { x: > 0, y: 0}:
                        TutorialController.TutorialChecks["Right"] = true;
                        break;
                    case { x: < 0, y: 0}:
                        TutorialController.TutorialChecks["Left"] = true;
                        break;
                }


            }

            movementInput = MoveAction.ReadValue<Vector2>();
            playerVelocity = (PlayerTransform.right * movementInput.x +
                               PlayerTransform.forward * movementInput.y) * PlayerSpeed;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (isJumping && Character.canJump)
                StateMachine.ChangeState(Character.JumpingState);
            if (isMoving)
                StateMachine.ChangeState(Character.WalkingState);
            if (isSliding && Character.canSlide)
                StateMachine.ChangeState(Character.SlidingState);

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            verticalVelocity.y += GravityValue * Time.deltaTime;
            isGrounded = Character.isGrounded;
            if (isGrounded && verticalVelocity.y < 0)
                verticalVelocity.y = 0f;

            Character.characterController.Move(playerVelocity * Time.deltaTime + verticalVelocity * Time.deltaTime);

        }

    }
}
