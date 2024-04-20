using System;
using System.Collections;
using Cameras;
using input;
using Player.FSM;
using Player.FSM.States;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    
    
    #region stateMachine

    public class PlayerStateMachine : FiniteStateMachine
    {
        [HideInInspector] public Idle IdleState;
        [HideInInspector] public Walking walkingState;
        [HideInInspector] public Sprinting sprintingState;
        [HideInInspector] public Jumping jumpingState;
        [HideInInspector] public Airborne airborneState;
        [HideInInspector] public WallJumping wallJumpingState;
        [HideInInspector] public WallRunning wallRunState;


        // private void Awake()
        // {
        //     IdleState = new Idle(this);
        //     walkingState = new Walking(this);
        //     sprintingState = new Sprinting(this);
        //     jumpingState = new Jumping(this);
        //     airborneState = new Airborne(this);
        //     wallJumpingState = new WallJumping(this);
        //     wallRunState = new WallRunning(this);
        // }
        
    }
    

    #endregion
    
    
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Player Movement")] [SerializeField]
        private float playerSpeed;

        [SerializeField] private float sprintingSpeed;
        [SerializeField] private Quaternion maxWallRotation;

        [Header("Player Jump")] [SerializeField]
        private float playerJumpHeight;

        [SerializeField] private float playerGravity;
        [SerializeField] private float playerJumpCooldown;

        [Header("Layer Mask Settings")] [SerializeField]
        private LayerMask groundMask;

        [SerializeField] private LayerMask whatIsWall;

        [Header("Wall Run Settings")] [SerializeField]
        private float wallRunSpeed;

        [SerializeField] private float wallRunForce;
        [SerializeField] private float wallRunMaxDuration;
        [SerializeField] private float wallRunExitTime;

        [Header("Wall Run Detection Settings")] [SerializeField]
        private float maxWallDistance;

        [Header("Wall Jump Settings")] [SerializeField]
        private float wallJumpUpForce;

        [SerializeField] private float wallJumpSideForce;
        [SerializeField] private float wallMemoryTime;

        [Header("Sliding Settings")] [SerializeField]
        private float maxSlideTime;

        [SerializeField] private float slideForce;
        [SerializeField] private float slideYScale;



        public enum MoveStates
        {
            Sprinting,
            Walking,
            Wallrunning,
            Walljumping,
            Sliding,
            Jumping,
            Idle,
            Airborne
            
        }

        
        private inputSystem _inputSystem;
        private GameObject _eventSystem;
        private GameObject _mostRecentWall;
        private bool _isJumping;
        private Vector3 _playerVelocity;
        private Vector3 _verticalVelocity = Vector3.zero;
        private CharacterController _characterController;
        private PlayerStamina _playerStamina;
        private PlayerController _playerController;
        private RaycastHit _leftWallHit;
        private RaycastHit _rightWallHit;
        private bool _leftWall;
        private bool _rightWall;
        private bool _isWallRunning;
        private bool _useGravity = true;
        private bool _canJump = true;
        private bool _isExitingWall;
        private bool _exitWallTimerActive;
        private bool _hasExceededWallRunTime;
        private Coroutine activeWallRunTimer;
        private bool _isSliding;
        private bool _isSprinting;
        private float _startYScale;
        private MoveStates _playerMoveState;



        private bool _isGrounded;
        private Transform PlayerTransform => _characterController.transform;


        public float JumpHeight => playerJumpHeight;
        public float PlayerSpeed => playerSpeed;
        public float SprintingSpeed => sprintingSpeed;

        public MoveStates PlayerMoveState => _playerMoveState;

        private void Start()
        {
            _playerController = GetComponent<PlayerController>(); // get player controller script
            _inputSystem = _playerController.inputSystem; // reference input system script / component
            _characterController = _playerController.characterController; // get character controller component
            // _playerStamina = _playerController.playerStamina;
            _startYScale = PlayerTransform.localScale.y;

        }

        private void FixedUpdate()
        {
            _isGrounded = _characterController.isGrounded; // constantly updating if player is grounded
            CheckWalls();
            HandleMovement();
            if (_isSliding)
                SlidingMovement();

        }

        private void HandleMovement()
        {
            // if grounded zero vertical velocity
            if (_isGrounded) ZeroVerticalVelocity();
            // player velocity calculations
            CalculatePlayerVelocity();
            // if is wall running then wall run movement if not then normal movement
            if (_isWallRunning)
                WallRunMovement();
            else
                NormalPlayerMovement();
            // if the player has the ability to wall run then start wall run
            if (IfCanWallRun())
                StartWallRun();
            // is wall running, no walls and is touching ground
            if (IfCanNoLongerWallRun())
                StopWallRun();
            // check if player is currently exiting a wall
            CheckExitWallState();
            // control jumping:as
            ControlJumpingState();
            // control gravity 
            ControlGravityState();
        }

        public void Jump()
        {
            if (!_canJump) return;
            _useGravity = true;
            // if airborne and not wallrunning OR if player is already jumping then exit function
            // if (!_isGrounded && !_isWallRunning) return;
            var moveState = GetPlayerMovementState();
            if (moveState is MoveStates.Airborne) return;
            _isJumping = true;
            StartCoroutine(JumpCooldown());
        }

        private void CalculateVerticalVelocity()
        {
            _verticalVelocity.y = Mathf.Sqrt(-2.21251f * JumpHeight * playerGravity);
        }

        private void ControlGravityState()
        {
            if (!_useGravity && !_isWallRunning) return;
            ImitateGravity();
            PlayerVerticalMovement();
        }

        private void ControlJumpingState()
        {
            switch (_isJumping)
            {
                case true when !_isGrounded && !_isWallRunning:
                    _isJumping = false;
                    break;
                case true when _isWallRunning:
                    WallJump();
                    break;
                case true when _isGrounded && _useGravity:
                    CalculateVerticalVelocity();
                    break;
            }

        }

        private void ImitateGravity()
        {
            _verticalVelocity.y += playerGravity * Time.deltaTime;
        }

        private void CheckExitWallState()
        {
            // Exiting Wall State
            if (_isExitingWall)
            {
                if (_isWallRunning) StopWallRun();
                if (!_exitWallTimerActive) StartCoroutine(ExitingWallTimer());
            }
            else
                switch (_hasExceededWallRunTime)
                {
                    case true when _isWallRunning:
                        StopWallRun();
                        _hasExceededWallRunTime = false;
                        break;
                    case true when _isGrounded:
                        _hasExceededWallRunTime = false;
                        break;
                }
        }

        private void ZeroVerticalVelocity()
        {
            _verticalVelocity = Vector3.zero;
        }

        private void NormalPlayerMovement()
        {
            _characterController.Move(_playerVelocity * Time.deltaTime);
        }

        private void PlayerVerticalMovement()
        {
            _characterController.Move(_verticalVelocity * Time.deltaTime); // move player vertically
        }

        private void CalculatePlayerVelocity()
        {
            _playerVelocity = (PlayerTransform.right * _inputSystem.HorizontalInput +
                               PlayerTransform.forward * _inputSystem.VerticalInput) * playerSpeed;
        }

        private bool IfCanWallRun()
        {
            return (_leftWall || _rightWall) && (_playerVelocity.x > 0 || _playerVelocity.z > 0) && !_isGrounded &&
                   !_isWallRunning && !_isExitingWall;
        }

        private bool IfCanNoLongerWallRun()
        {
            return _isWallRunning && !_leftWall && !_rightWall;
        }

        private void CheckWalls()
        {
            var right = PlayerTransform.right;
            var position = PlayerTransform.position;
            _rightWall =
                Physics.Raycast(position, right, out _rightWallHit, maxWallDistance,
                    whatIsWall); // check for wall on right
            _leftWall = Physics.Raycast(position, -right, out _leftWallHit, maxWallDistance,
                whatIsWall); // check for wall on left

            // if (_rightWall)
            // {
            //     _mostRecentWall = _rightWallHit.transform.gameObject;
            // }
            // else if (_leftWall)
            // {
            //     _mostRecentWall = _leftWallHit.transform.gameObject;
            // }
        }

        private void StartWallRun()
        {
            _isWallRunning = true;
            _isJumping = false;
            ApplyCameraEffects();
        }

        private void StopWallRun()
        {
            _isWallRunning = false;
            _useGravity = true;
            ResetCameraEffects();
        }



        private void WallRunMovement()
        {
            // TODO - figure out how this function works
            if (_isJumping || _hasExceededWallRunTime || _isGrounded) return;
            _verticalVelocity.y = 0f;
            _useGravity = false;
            // get the raycast normal of the left or right wall, whatever has been hit (opposing direction of raycast and intersecting point)
            var wallNormal = _rightWall ? _rightWallHit.normal : _leftWallHit.normal;
            // cross product of two vectors perpendicular to the normal above and the players y vector
            var wallForward = Vector3.Cross(wallNormal, PlayerTransform.up);

            // if length of the player forward subtract the perpendicular vector is greater than the length of the player forward subtract the negative perpendicular vector
            if ((PlayerTransform.forward - wallForward).magnitude > (PlayerTransform.forward - -wallForward).magnitude)
            {
                wallForward = -wallForward; // inverse the value
            }

            // move the player along the wall
            _characterController.Move(wallForward * (wallRunForce * Time.deltaTime));

            // if there is no walls detected and player moving
            if (!_leftWall && _inputSystem.HorizontalInput > 0 && !_rightWall && _inputSystem.HorizontalInput < 0)
            {
                _characterController.Move(-wallNormal * (100 * Time.deltaTime)); // push the player off the wall (?)
            }

            activeWallRunTimer = StartCoroutine(nameof(WallRunTimer));
        }

        private void WallJump()
        {
            StopCoroutine(nameof(WallRunTimer));
            if (_hasExceededWallRunTime) _hasExceededWallRunTime = false;
            // exit wall state
            _isExitingWall = true;
            // get the normals again like in the movement function
            var wallNormal = _rightWall ? _rightWallHit.normal : _leftWallHit.normal;
            // calculate the force the player will jump off the wall at
            var playerForceToApply = PlayerTransform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;
            _verticalVelocity = playerForceToApply;
            // _characterController.Move(_verticalVelocity);
            _isJumping = false;
        }

        private IEnumerator ForgetPreviousWall()
        {
            yield return new WaitForSeconds(wallMemoryTime);
        }

        private IEnumerator JumpCooldown()
        {
            _canJump = false;
            yield return new WaitForSeconds(playerJumpCooldown);
            _canJump = true;
        }

        private IEnumerator ExitingWallTimer()
        {
            _exitWallTimerActive = true;
            yield return new WaitForSeconds(wallRunExitTime);
            _isExitingWall = false;
            _exitWallTimerActive = false;
        }

        private IEnumerator WallRunTimer()
        {
            yield return new WaitForSeconds(wallRunMaxDuration);
            _hasExceededWallRunTime = true;
        }

        public void CheckIfCanSlide()
        {
            Debug.LogWarning("Slide Pressed");
            var moveState = GetPlayerMovementState();
            // if (_isWallRunning || _isSliding || _isJumping || !_isGrounded) return;
            if (moveState is MoveStates.Wallrunning or MoveStates.Sliding or MoveStates.Jumping or MoveStates.Airborne) return;
            if (_inputSystem.HorizontalInput != 0 || _inputSystem.VerticalInput != 0)
                StartSlide();
        }

        public void CancelSlide()
        {
            Debug.LogWarning("Slide Unpressed");
            if (!_isSliding) return;
            StopSlide();
        }

        private void StartSlide()
        {
            _isSliding = true;
            var localScale = PlayerTransform.localScale;
            localScale = new Vector3(localScale.x, slideYScale, localScale.z);
            PlayerTransform.localScale = localScale;
        }

        private void StopSlide()
        {
            _isSliding = false;
            var localScale = PlayerTransform.localScale;
            localScale = new Vector3(localScale.x, _startYScale, localScale.z);
            PlayerTransform.localScale = localScale;
        }

        private void SlidingMovement()
        {
            var inputDir = PlayerTransform.forward * _inputSystem.VerticalInput +
                           PlayerTransform.right * _inputSystem.HorizontalInput;

            _characterController.Move(inputDir.normalized * slideForce);
            StartCoroutine(nameof(SlideTimer));
        }

        private IEnumerator SlideTimer()
        {
            yield return new WaitForSeconds(maxSlideTime);
            if (_isSliding) CancelSlide();
        }

        private static void DoFov(float endValue)
        {
            // MainCamera.DoFov(120f);
            }

        private static void DoTilt(float endValue)
        {
            // MainCamera.DoTilt(endValue);
        }

        private void ApplyCameraEffects()
        {
            if (_leftWall)
                DoTilt(-5f);
            else if (_rightWall)
                DoTilt(5f);

            DoFov(110f);
        }

        private void ResetCameraEffects()
        {
            DoTilt(0f);
            DoFov(90f);
        }


        public MoveStates GetPlayerMovementState()
        {
            return _isGrounded switch
            {
                false when _verticalVelocity.y > 0 => MoveStates.Jumping,
                false when _isWallRunning => MoveStates.Wallrunning,
                false when _isExitingWall && _verticalVelocity.y > 0 => MoveStates.Walljumping,
                false when !_isSliding => MoveStates.Airborne,
                true when _isSliding => MoveStates.Sliding,
                true when _playerVelocity.x > 0 || _playerVelocity.z > 0 => MoveStates.Walking,
                true when _isSprinting => MoveStates.Sprinting,
                _ => MoveStates.Idle
            };
        }
    }
}

