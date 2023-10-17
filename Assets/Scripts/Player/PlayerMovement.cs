using System;
using input;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Player Movement")]
        [SerializeField] private float playerSpeed;
        [SerializeField] private float sprintingSpeed;
        [Header("Player Jump")] 
        [SerializeField] private float playerJumpHeight;
        [SerializeField] private float playerGravity;
        [Header("Layer Mask Settings")] 
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private LayerMask whatIsWall;
        [Header("Wall Run Settings")] 
        [SerializeField] private float wallRunSpeed;
        [SerializeField] private float wallRunForce;
        [SerializeField] private float wallRunMaxDuration;
        [Header("Wall Run Detection Settings")] 
        [SerializeField] private float maxWallDistance;

        
        
        private inputSystem _inputSystem;
        private GameObject _eventSystem;
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
        
        
        private bool IsGrounded;
        private Transform PlayerTransform => _characterController.transform;
        
        
        public float JumpHeight => playerJumpHeight;
        public float PlayerSpeed => playerSpeed;
        public float SprintingSpeed => sprintingSpeed;
        

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _inputSystem = _playerController.inputSystem;
            _characterController = _playerController.characterController;

        }

        private void FixedUpdate()
        {
            IsGrounded = _characterController.isGrounded;
            CheckWalls();
            HandleMovement();
            
        }

        private void HandleMovement()
        {
            if (IsGrounded) _verticalVelocity.y = 0f;

            _playerVelocity = (PlayerTransform.right * _inputSystem.HorizontalInput +
                               PlayerTransform.forward * _inputSystem.VerticalInput) * playerSpeed;

            switch (_isWallRunning)
            {
                case false:
                    _characterController.Move(_playerVelocity * Time.deltaTime);
                    break;
                case true:
                    WallRunMovement();
                    break;
            }

            if ((_leftWall || _rightWall) && (_playerVelocity.x > 0 || _playerVelocity.z > 0) && !IsGrounded && !_isWallRunning)
            {
                StartWallRun();
            }
            else if (_isWallRunning && (!_leftWall || !_rightWall) && IsGrounded)
            {
                StopWallRun();
            }
            
            switch (_isJumping)
            {
                case true when !IsGrounded && !_isWallRunning: // if airborne and not wallrunning
                    _isJumping = false;
                    break;
                case true when IsGrounded && _useGravity: // if grounded, gravity enabled and is wallrunning
                    _verticalVelocity.y = Mathf.Sqrt(-2f * JumpHeight * playerGravity); // jump calculations
                    break;
                case true when !IsGrounded && _isWallRunning && !_useGravity: // if airborne, gravity disabled and is wallrunning
                    _useGravity = true; // enable gravity
                    _verticalVelocity.y = Mathf.Sqrt(-2f * JumpHeight * playerGravity); // jump calculation
                    _isJumping = false; // stop jumping
                    break;
                case false:
                    break;
            }

            if (!_useGravity && !_isWallRunning) return;
            _useGravity = true;
            _verticalVelocity.y += playerGravity * Time.deltaTime;
            _characterController.Move(_verticalVelocity * Time.deltaTime);

        }

        public void Jump()
        {
            if ((!IsGrounded  && !_isWallRunning) || _isJumping) return;
            Debug.Log("button pressed");
            _isJumping = true;
            _useGravity = true;
        }

        private void CheckWalls()
        {
            var right = PlayerTransform.right;
            var position = PlayerTransform.position;
            _rightWall = Physics.Raycast(position, right, out _rightWallHit, maxWallDistance, whatIsWall);
            _leftWall = Physics.Raycast(position, -right, out _leftWallHit, maxWallDistance, whatIsWall);
        }

        private void StartWallRun()
        {
            _isWallRunning = true;
            _isJumping = false;
        }

        private void StopWallRun()
        {
            _isWallRunning = false;
            _isJumping = false;
            _useGravity = true;
        }

        private void WallRunMovement()
        {
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
        }
        
    }
}
