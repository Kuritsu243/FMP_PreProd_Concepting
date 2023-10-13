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

        private inputSystem _inputSystem;
        private GameObject _eventSystem;
        private bool _isJumping;
        private Vector3 _playerVelocity;
        private Vector3 _verticalVelocity = Vector3.zero;
        private CharacterController _characterController;
        private PlayerController _playerController;
        
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
            HandleMovement();
            
        }

        private void HandleMovement()
        {
            if (IsGrounded) _verticalVelocity.y = 0f;

            _playerVelocity = (PlayerTransform.right * _inputSystem.HorizontalInput +
                               PlayerTransform.forward * _inputSystem.VerticalInput) * playerSpeed;

            _characterController.Move(_playerVelocity * Time.deltaTime);

            switch (_isJumping)
            {
                case true when !IsGrounded:
                    _isJumping = false;
                    Debug.Log("Cannot jump! is not grounded");
                    break;
                case true when IsGrounded:
                    _verticalVelocity.y = Mathf.Sqrt(-2f * JumpHeight * playerGravity);
                    break;
                case false:
                    break;
            }

            _verticalVelocity.y += playerGravity * Time.deltaTime;
            _characterController.Move(_verticalVelocity * Time.deltaTime);
        }

        public void Jump()
        {
            if (!IsGrounded || _isJumping) return;
            Debug.Log("button pressed");
            _isJumping = true;
        }
        
    }
}
