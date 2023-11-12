using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Camera;
using Cinemachine;

namespace Player.FSM
{
    public abstract class FsmState
    {
        protected PlayerController Character;
        protected FiniteStateMachine StateMachine;
        protected string StateName;

        protected readonly InputAction MoveAction;
        protected readonly InputAction LookAction;
        public InputAction PerspectiveAction;
        protected readonly InputAction JumpAction;
        protected readonly InputAction SlideAction;
        public InputAction SprintAction;
        protected bool IsGrounded;
        protected bool IsJumping;
        protected bool IsSliding;
        protected bool IsMoving;
        protected Vector2 MovementInput;
        // protected Vector3 PlayerVelocity;
        protected Vector2 MouseInput;
        protected float MouseX;
        protected float MouseY;
        protected Vector3 VerticalVelocity;
        protected Transform PlayerTransform;
        protected CinemachineFreeLook ThirdPersonCam;
        protected CinemachineVirtualCamera FirstPersonCam;
        protected float XRotation;
        protected Vector3 TargetRotation;
        protected float GravityValue;
        protected float PlayerSpeed;


        protected FsmState(string stateName, FiniteStateMachine stateMachine, PlayerController playerController)
        {
            this.StateName = stateName;
            this.StateMachine = stateMachine;
            this.Character = playerController;


            MoveAction = playerController.playerInput.actions["Movement"];
            LookAction = playerController.playerInput.actions["Look"];
            PerspectiveAction = playerController.playerInput.actions["Perspective"];
            JumpAction = playerController.playerInput.actions["Jump"];
            SlideAction = playerController.playerInput.actions["Slide"];
            SprintAction = playerController.playerInput.actions["Sprint"];
            PlayerTransform = Character.PlayerTransform;
            GravityValue = Character.PlayerGravity;
            PlayerSpeed = Character.PlayerSpeed;

        }
        
        
        // mechanics
        public virtual void Enter()
        {
            //
            // XRotation = 0;
            // TargetRotation = Vector3.zero;
            // PlayerVelocity = Vector3.zero;
        }

        public virtual void HandleInput()
        {
            if (JumpAction.triggered)
            {
                VerticalVelocity = Vector3.zero;
                IsJumping = true;
            }

            if (SlideAction.triggered)
            {
                IsSliding = true;
            }

            IsMoving = MovementInput is not {x: 0, y: 0};

            MovementInput = MoveAction.ReadValue<Vector2>();


  


        }

        public virtual void LogicUpdate()
        {
            switch (IsMoving)
            {
                case true:
                    StateMachine.ChangeState(Character.walkingState);
                    break;
                case false:
                    StateMachine.ChangeState(Character.IdleState);
                    break;
            }
        }

        public virtual void PhysicsUpdate()
        {
            IsGrounded = Character.isGrounded;
            VerticalVelocity.y += GravityValue * Time.deltaTime;
            
        }



        public virtual void Tick(float deltaTime)
        {
            
        }

        public virtual void Exit()
        {

            // PlayerVelocity = Vector3.zero;
        }
        
    }
}
