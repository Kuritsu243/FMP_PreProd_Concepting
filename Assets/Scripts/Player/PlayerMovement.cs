using System.Collections;
using input;
using Player.FSM;
using UnityEngine;

namespace Player
{
    #region stateMachine

    public class PlayerStateMachine : FiniteStateMachine
    {
    }

    #endregion


    public class PlayerMovement : MonoBehaviour
    {
        [Header("Player Movement")] 
        [SerializeField] private float playerSpeed;

        [SerializeField] private float sprintingSpeed;
        [SerializeField] private Quaternion maxWallRotation;

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
        private bool _isExitingWall;
        private bool _exitWallTimerActive;
        private bool _hasExceededWallRunTime;
        private bool _isSliding;
        private bool _isSprinting;
        private float _startYScale;


        private bool _isGrounded;
        private Transform PlayerTransform => _characterController.transform;


        private float JumpHeight => playerJumpHeight;

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _inputSystem = _playerController.inputSystem;
            _characterController = _playerController.characterController;
            _startYScale = PlayerTransform.localScale.y;
        }

        private void FixedUpdate()
        {
            _isGrounded = _characterController.isGrounded;
            CheckWalls();
            HandleMovement();
            if (_isSliding)
                SlidingMovement();
        }

        private void HandleMovement()
        {
            if (_isGrounded) ZeroVerticalVelocity();
            CalculatePlayerVelocity();
            if (_isWallRunning)
                WallRunMovement();
            else
                NormalPlayerMovement();
            if (IfCanWallRun())
                StartWallRun();
            if (IfCanNoLongerWallRun())
                StopWallRun();
            CheckExitWallState();
            ControlJumpingState();
            ControlGravityState();
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
                    whatIsWall);
            _leftWall = Physics.Raycast(position, -right, out _leftWallHit, maxWallDistance,
                whatIsWall);
        }

        private void StartWallRun()
        {
            _isWallRunning = true;
            _isJumping = false;
        }

        private void StopWallRun()
        {
            _isWallRunning = false;
            _useGravity = true;
        }


        private void WallRunMovement()
        {
            if (_isJumping || _hasExceededWallRunTime || _isGrounded) return;
            _verticalVelocity.y = 0f;
            _useGravity = false;
            var wallNormal = _rightWall ? _rightWallHit.normal : _leftWallHit.normal;
            var wallForward = Vector3.Cross(wallNormal, PlayerTransform.up);
            if ((PlayerTransform.forward - wallForward).magnitude > (PlayerTransform.forward - -wallForward).magnitude)
            {
                wallForward = -wallForward;
            }

            _characterController.Move(wallForward * (wallRunForce * Time.deltaTime));
            if (!_leftWall && _inputSystem.HorizontalInput > 0 && !_rightWall && _inputSystem.HorizontalInput < 0)
            {
                _characterController.Move(-wallNormal * (100 * Time.deltaTime));
            }

            StartCoroutine(nameof(WallRunTimer));
        }

        private void WallJump()
        {
            StopCoroutine(nameof(WallRunTimer));
            if (_hasExceededWallRunTime) _hasExceededWallRunTime = false;
            _isExitingWall = true;
            var wallNormal = _rightWall ? _rightWallHit.normal : _leftWallHit.normal;
            var playerForceToApply = PlayerTransform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;
            _verticalVelocity = playerForceToApply;
            _isJumping = false;
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

        private void CancelSlide()
        {
            if (!_isSliding) return;
            StopSlide();
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