using System;
using System.Collections;
using input;
using UnityEngine;

namespace Player
{
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
        
        
        
        
        
        
        private inputSystem _inputSystem;
        private GameObject _eventSystem;
        private GameObject _mostRecentWall;
        private bool _isJumping;
        private Vector3 _playerVelocity;
        private Vector3 _verticalVelocity = Vector3.zero;
        private CharacterController _characterController;
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
        private float _startYScale;
        

        
        
        private bool _isGrounded;
        private Transform PlayerTransform => _characterController.transform;
        
        
        public float JumpHeight => playerJumpHeight;
        public float PlayerSpeed => playerSpeed;
        public float SprintingSpeed => sprintingSpeed;
        

        private void Start()
        {
            _playerController = GetComponent<PlayerController>(); // get player controller script
            _inputSystem = _playerController.inputSystem; // reference input system script / component
            _characterController = _playerController.characterController; // get character controller component
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
            if (_isGrounded) _verticalVelocity = Vector3.zero; // if grounded zero vertical velocity
            // player velocity calculations
            _playerVelocity = (PlayerTransform.right * _inputSystem.HorizontalInput +
                               PlayerTransform.forward * _inputSystem.VerticalInput) * playerSpeed;
            
            switch (_isWallRunning) // if wall running check
            {
                case false:
                    _characterController.Move(_playerVelocity * Time.deltaTime); // if not wall running then move normally
                    break;
                case true:
                    WallRunMovement(); // wallrun movement
                    break;
            }
            // left or right wall hit, player moving and is airborne, while not wall running
            if ((_leftWall || _rightWall) && (_playerVelocity.x > 0 || _playerVelocity.z > 0) && !_isGrounded && !_isWallRunning && !_isExitingWall)
            {
                StartWallRun();
            }
            else if (_isWallRunning && !_leftWall && !_rightWall) // is wall running, no walls and is touching ground
            {
                StopWallRun();
            }
            // Exiting Wall State
            else if (_isExitingWall)
            {
                if (_isWallRunning) StopWallRun();
                if (!_exitWallTimerActive) StartCoroutine(ExitingWallTimer());
            }
            else switch (_hasExceededWallRunTime)
            {
                case true when _isWallRunning:
                    StopWallRun();
                    _hasExceededWallRunTime = false;
                    break;
                case true when _isGrounded:
                    _hasExceededWallRunTime = false;
                    break;
            }
            
            switch (_isJumping)
            {
                case true when !_isGrounded && !_isWallRunning: // if airborne and not wallrunning
                    _isJumping = false;
                    break;
                case true when _isWallRunning:
                    WallJump();
                    break;
                case true when _isGrounded && _useGravity: // if grounded, gravity enabled and is wallrunning
                    _verticalVelocity.y = Mathf.Sqrt(-2f * JumpHeight * playerGravity);
                    break;
                case false:
                    break;
            }

            switch (_useGravity)
            {
                case false when !_isWallRunning:
                    return; // exit function is gravity disabled and isn't wall running
                case true:
                    _verticalVelocity.y += playerGravity * Time.deltaTime; // build vertical velocity to simulate gravity
                    _characterController.Move(_verticalVelocity * Time.deltaTime); // move player vertically
                    break;
            }
            

        }

        public void Jump()
        {
            if (!_canJump) return;
            _useGravity = true;
            // if airborne and not wallrunning OR if player is already jumping then exit function
            if (!_isGrounded  && !_isWallRunning) return;
            _isJumping = true;
            StartCoroutine(JumpCooldown());
        }

        private void CheckWalls()
        {
            var right = PlayerTransform.right; 
            var position = PlayerTransform.position;
            _rightWall = Physics.Raycast(position, right, out _rightWallHit, maxWallDistance, whatIsWall); // check for wall on right
            _leftWall = Physics.Raycast(position, -right, out _leftWallHit, maxWallDistance, whatIsWall); // check for wall on left

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
        }

        private void StopWallRun()
        {
            _isWallRunning = false;
            _useGravity = true;
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
        
        
    }
}
