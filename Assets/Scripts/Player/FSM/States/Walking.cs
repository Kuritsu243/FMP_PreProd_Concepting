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
        private Vector2 mouseInput;
        private Vector2 movementInput;
        private Vector3 playerVelocity;
        private Vector3 verticalVelocity;
        private float _mouseX;
        private float _mouseY;
        private float _xRotation;
        private Vector3 _targetRotation;
        private CinemachineFreeLook thirdPersonCam;
        private CinemachineVirtualCamera firstPersonCam;
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
            
            playerVelocity = (PlayerTransform.right * MovementInput.x + PlayerTransform.forward * MovementInput.y) *
                             PlayerSpeed;

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            
            if (isJumping)
                StateMachine.ChangeState(Character.jumpingState);
            if (!isMoving && !isJumping)
                StateMachine.ChangeState(Character.IdleState);

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            
            verticalVelocity.y += gravityValue * Time.deltaTime;
            isGrounded = Character.isGrounded;
            
            Debug.LogWarning(playerVelocity);
            Character.characterController.Move(playerVelocity * Time.deltaTime + VerticalVelocity * Time.deltaTime);

        }

        public override void Exit()
        {
            base.Exit();
            verticalVelocity = Vector3.zero;
            XRotation = 0;
            TargetRotation = Vector3.zero;
            

        }
    }
}
