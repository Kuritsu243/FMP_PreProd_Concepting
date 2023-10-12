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
        [Header("Layer Mask Settings")] 
        [SerializeField] private LayerMask groundMask;

        private inputSystem _inputSystem;
        private GameObject _eventSystem;
        private bool _isJumping;
        private Vector3 _playerVelocity;
        private Vector3 _verticalVelocity = Vector3.zero;
        private CharacterController _characterController;
        private PlayerController _playerController;
        
        private bool IsGrounded => _characterController.isGrounded;
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
            HandleMovement();
        }

        private void HandleMovement()
        {
            if (IsGrounded) _verticalVelocity.y = 0;

            _playerVelocity = (PlayerTransform.right * _inputSystem.HorizontalInput +
                               PlayerTransform.forward * _inputSystem.VerticalInput) * playerSpeed;

            _characterController.Move(_playerVelocity * Time.deltaTime);
        }

        public void Jump()
        {
            
        }
        
    }
}
