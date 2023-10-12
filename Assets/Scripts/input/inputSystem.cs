using System;
using Camera;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace input
{
    public class inputSystem : MonoBehaviour
    {
        public inputSystem Instance { get; private set; }

        public float HorizontalInput => _movementInput.x;
        public float VerticalInput => _movementInput.y;

        public float MouseX => _mouseInput.x;
        public float MouseY => _mouseInput.y;
        


        private PlayerActions _input;
        private Vector2 _movementInput;
        private Vector2 _mouseInput;
        private mainCamera _mainCamera;
        private PlayerController _playerController;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }


        private void OnEnable()
        {
            if (_input == null)
            {
                _input = new PlayerActions();
                _input.main.Movement.performed += i => _movementInput = i.ReadValue<Vector2>();
                _input.main.Look.performed += i => _mouseInput = i.ReadValue<Vector2>();
                _input.main.Perspective.performed += _ => _mainCamera.ChangeCamera();
                _input.main.Jump.performed += _ => _playerController.playerMovement.Jump();
            }
            _input.Enable();
            _mainCamera = GetComponent<mainCamera>();
            _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            Cursor.lockState = CursorLockMode.Locked;

        }
        
    }
}
