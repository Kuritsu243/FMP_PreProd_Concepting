using UnityEngine;
using UnityEngine.InputSystem;
using Camera;

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
        }
        
        
        // mechanics
        public virtual void Enter()
        {
        }

        public virtual void HandleInput()
        {
            
        }

        public virtual void LogicUpdate()
        {
            
        }

        public virtual void PhysicsUpdate()
        {
            
        }


        public virtual void Tick(float deltaTime)
        {
            
        }

        public virtual void Exit()
        {
            
        }
        
    }
}
