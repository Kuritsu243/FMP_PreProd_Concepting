using System;
using System.Collections;
using Camera;
using Cinemachine;
using input;
using Player.FSM.States;
using UnityEngine;
using UnityEngine.InputSystem;

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
        
#region Required Components
        [HideInInspector] public GameObject eventSystem;
        [HideInInspector] public inputSystem inputSystem;
        [HideInInspector] public PlayerInput playerInput;
        [HideInInspector] public MainCamera mainCamera;
        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public GameObject playerMesh;
#endregion

#region Player States

        private PlayerStateMachine _playerStateMachine;
        [HideInInspector] public Idle IdleState;
        [HideInInspector] public Walking WalkingState;
        [HideInInspector] public Sprinting sprintingState;
        [HideInInspector] public Jumping JumpingState;
        [HideInInspector] public Airborne AirborneState;
        [HideInInspector] public WallJumping WallJumpingState;
        [HideInInspector] public WallRunning WallRunState;
        [HideInInspector] public Sliding SlidingState;
        
#endregion

#region Configurable Settings

        [Header("Player Movement")] 
        [SerializeField] private float playerSpeed;
        [SerializeField] private float sprintingSpeed;
        [SerializeField] private Quaternion maxWallRotation;

        [Header("Player Look")] 
        [Range(0, 200)][SerializeField] private float mouseSensitivity;
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
        [SerializeField] private float wallRunCooldown;
        
        [Header("Wall Run Detection Settings")] 
        [SerializeField] private float maxWallDistance;

        [Header("Wall Jump Settings")] 
        [SerializeField] private float wallJumpUpForce;
        [SerializeField] private float wallJumpSideForce;
        [SerializeField] private float wallMemoryTime;
        [SerializeField] private float wallJumpCooldown;
        

        [Header("Sliding Settings")] 
        [SerializeField] private float maxSlideTime;
        [SerializeField] private float slideForce;
        [SerializeField] private float slideYScale;
        [SerializeField] private float slideCooldown;
        
#endregion

#region Public References to private vars

        public Transform PlayerTransform => characterController.transform;
        public float JumpHeight => playerJumpHeight;
        public float PlayerSpeed => playerSpeed;
        public float SprintingSpeed => sprintingSpeed;
        public float XClamp => xClamp;
        public float RotationSpeed => rotationSpeed;
        public float SlideCooldown => slideCooldown;
        public float PlayerGravity => playerGravity;
        public float WallRunForce => wallRunForce;
        public float WallRunSpeed => wallRunSpeed;
        public LayerMask WhatIsWall => whatIsWall;
        public float MaxSlideTime => maxSlideTime;
        public float SlideForce => slideForce;
        public float SlideYScale => slideYScale;
        public float JumpCooldown => playerJumpCooldown;
        public float WallRunCooldown => wallRunCooldown;
        public float WallJumpCooldown => wallJumpCooldown;
        public float WallJumpUpForce => wallJumpUpForce;
        public float WallJumpSideForce => wallJumpSideForce;
        public float MaxWallDistance => maxWallDistance;
        
#endregion

#region Public Vars

        public bool isGrounded;
        public bool canSlide;
        public bool canJump;
        public bool checkForWallsWhenAirborne;
        public bool canWallRun;
        public bool canWallJump;
        public bool jumpingFromLeftWall;
        public bool jumpingFromRightWall;
        public bool leftWall;
        public bool rightWall;
        
        public RaycastHit JumpingLeftWallHit;
        public RaycastHit JumpingRightWallHit;
        public RaycastHit LeftWallHit;
        public RaycastHit RightWallHit;

        public CinemachineBrain activeCinemachineBrain;

#endregion


        
        public void Awake()
        {
            characterController = GetComponentInChildren<CharacterController>();
            eventSystem = GameObject.FindGameObjectWithTag("EventSystem");
            mainCamera = eventSystem.GetComponent<MainCamera>();
            inputSystem = eventSystem.GetComponent<inputSystem>();
            playerMesh = transform.FindGameObjectInChildWithTag("PlayerMesh");
            playerInput = GetComponent<PlayerInput>();
            _playerStateMachine = new PlayerStateMachine();
            activeCinemachineBrain = GetComponentInChildren<CinemachineBrain>();
            
            IdleState = new Idle("Idle", this, _playerStateMachine);
            WalkingState = new Walking("Walking", this, _playerStateMachine);
            JumpingState = new Jumping("Jumping", this, _playerStateMachine);
            WallRunState = new WallRunning("WallRunning", this, _playerStateMachine);
            AirborneState = new Airborne("Airborne", this, _playerStateMachine);
            SlidingState = new Sliding("Sliding", this, _playerStateMachine);
            WallJumpingState = new WallJumping("WallJumping", this, _playerStateMachine);
            canSlide = true;
            canJump = true;
            canWallJump = true;
            _playerStateMachine.Initialize(IdleState);
            Cursor.lockState = CursorLockMode.Locked;
            SetMouseSensitivity();

        }

        private void Update()
        {
            _playerStateMachine.CurrentState.HandleInput();
            _playerStateMachine.CurrentState.LogicUpdate();
        }
        

        private void SetMouseSensitivity()
        {
            mainCamera.SetSensitivity(mouseSensitivity);
        }

        private void FixedUpdate()
        {
            isGrounded = characterController.isGrounded;
            Debug.LogWarning($"Is Player Grounded? {isGrounded}");
            _playerStateMachine.CurrentState.PhysicsUpdate();
        }

        private void OnGUI()
        {
            string content = _playerStateMachine.CurrentState != null
                ? _playerStateMachine.CurrentState.ToString()
                : "No state";
            GUILayout.Label($"<color='black'><size='40'>{content}</size></color>");
        }

        public IEnumerator ActionCooldown(Action cooldownComplete, float timeToTake)
        {
            yield return new WaitForSeconds(timeToTake);
            cooldownComplete?.Invoke();
        }

 
    }
}
