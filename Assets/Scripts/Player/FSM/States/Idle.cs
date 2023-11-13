using System;
using Camera;
using Cinemachine;
using UnityEngine;

namespace Player.FSM.States
{
    public class Idle : FsmState
    {
        private float _mouseX;
        private float _mouseY;
        private float _xRotation;
        private Vector3 _targetRotation;
        private bool isJumping;
        private bool isSliding;
        private bool isGrounded;
        private bool isMoving;
        private Vector2 mouseInput;
        private Vector2 movementInput;
        private Vector3 playerVelocity;
        private Vector3 verticalVelocity;
        private CinemachineFreeLook thirdPersonCam;
        private CinemachineVirtualCamera firstPersonCam;
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

            if (JumpAction.triggered)
                isJumping = true;
            if (SlideAction.triggered)
                isSliding = true;
            if (movementInput is not {x: 0, y: 0})
                isMoving = true;
            movementInput = MoveAction.ReadValue<Vector2>();
            playerVelocity = (PlayerTransform.right * movementInput.x +
                               PlayerTransform.forward * movementInput.y) * PlayerSpeed;
            

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (isJumping)
                StateMachine.ChangeState(Character.jumpingState);
            if (isMoving)
                StateMachine.ChangeState(Character.walkingState);

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
