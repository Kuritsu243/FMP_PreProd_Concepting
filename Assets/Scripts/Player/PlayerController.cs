using System;
using Camera;
using input;
using Player.FSM.States;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public static class TransformExtensions
    {
        public static GameObject FindGameObjectInChildWithTag(this Transform parent, string tag)
        {
            GameObject foundChild = null;
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.CompareTag(tag))
                    foundChild = child.gameObject;
            }

            if (foundChild != null)
            {
                return foundChild;
            }
            else
            {
                throw new Exception("No child object with tag!");
            }

        }
    }
    

    
    public class PlayerController : MonoBehaviour
    {
        public GameObject eventSystem;
        public inputSystem inputSystem;
        public PlayerInput playerInput;
        public MainCamera mainCamera;
        public CharacterController characterController;
        public GameObject playerMesh;
        
        public PlayerStateMachine playerStateMachine;
        public Idle IdleState;
        public Walking walkingState;
        public Sprinting sprintingState;
        public Jumping jumpingState;
        public Airborne airborneState;
        public WallJumping wallJumpingState;
        public WallRunning wallRunState;
        
        
        [Header("Player Movement")] 
        [SerializeField] private float playerSpeed;
        [SerializeField] private float sprintingSpeed;
        [SerializeField] private Quaternion maxWallRotation;

        [Header("Player Look")] 
        [SerializeField] private Vector2 mouseSensitivity;
        [SerializeField] private float xClamp;
        [SerializeField] private float rotationSpeed;
        
        
        
        
        
        [Header("Player Jump")] 
        [SerializeField] private float playerJumpHeight;
        [SerializeField] private float playerGravity;
        [SerializeField] private float playerJumpCooldown;

        [Header("Layer Mask Settings")] 
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private LayerMask whatIsWall;

        [Header("Wall Run Settings")] 
        [SerializeField] private float wallRunSpeed;
        [SerializeField] private float wallRunForce;
        [SerializeField] private float wallRunMaxDuration;
        [SerializeField] private float wallRunExitTime;

        [Header("Wall Run Detection Settings")] 
        [SerializeField] private float maxWallDistance;

        [Header("Wall Jump Settings")] 
        [SerializeField] private float wallJumpUpForce;
        [SerializeField] private float wallJumpSideForce;
        [SerializeField] private float wallMemoryTime;

        [Header("Sliding Settings")] 
        [SerializeField] private float maxSlideTime;
        [SerializeField] private float slideForce;
        [SerializeField] private float slideYScale;

        
        public Transform PlayerTransform => characterController.transform;
        public float JumpHeight => playerJumpHeight;
        public float PlayerSpeed => playerSpeed;
        public float SprintingSpeed => sprintingSpeed;

        public float XClamp => xClamp;

        public Vector2 MouseSensitivity => mouseSensitivity;

        public float RotationSpeed => rotationSpeed;

        public float PlayerGravity => playerGravity;

        public float WallRunForce => wallRunForce;

        public float WallRunSpeed => wallRunSpeed;

        public LayerMask WhatIsWall => whatIsWall;
        
        public bool isGrounded;

        public RaycastHit leftWallHit;
        public RaycastHit rightWallHit;
        public bool leftWall;
        public bool rightWall;
        public float MaxWallDistance => maxWallDistance;
        
        public void Awake()
        {
            characterController = GetComponentInChildren<CharacterController>();
            eventSystem = GameObject.FindGameObjectWithTag("EventSystem");
            mainCamera = eventSystem.GetComponent<MainCamera>();
            inputSystem = eventSystem.GetComponent<inputSystem>();
            playerMesh = transform.FindGameObjectInChildWithTag("PlayerMesh");
            playerInput = GetComponent<PlayerInput>();
            playerStateMachine = new PlayerStateMachine();
            
            IdleState = new Idle("Idle", this, playerStateMachine);
            walkingState = new Walking("Walking", this, playerStateMachine);
            jumpingState = new Jumping("Jumping", this, playerStateMachine);
            wallRunState = new WallRunning("WallRunning", this, playerStateMachine);
            
            playerStateMachine.Initialize(IdleState);
            Cursor.lockState = CursorLockMode.Locked;

        }

        private void Update()
        {
            playerStateMachine.CurrentState.HandleInput();
            playerStateMachine.CurrentState.LogicUpdate();
        }
        
        public void Test()
        {
        
        }

        private void FixedUpdate()
        {
            isGrounded = characterController.isGrounded;
            Debug.LogWarning($"Is Player Grounded? {isGrounded}");
            playerStateMachine.CurrentState.PhysicsUpdate();
        }

        private void OnGUI()
        {
            string content = playerStateMachine.CurrentState != null
                ? playerStateMachine.CurrentState.ToString()
                : "No state";
            GUILayout.Label($"<color='black'><size='40'>{content}</size></color>");
        }
    }
}
