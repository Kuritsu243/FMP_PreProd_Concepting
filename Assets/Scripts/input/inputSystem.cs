using System;
using Cameras;
using Cameras;
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
        


        public PlayerActions Input;
        private Vector2 _movementInput;
        private Vector2 _mouseInput;
        private MainCamera _mainCamera;
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
            if (Input == null)
            {
                // Input = new PlayerActions();
                // // movement
                // Input.main.Movement.performed += i => _movementInput = i.ReadValue<Vector2>();
                // // mouse look
                // Input.main.Look.performed += i => _mouseInput = i.ReadValue<Vector2>();
                // // camera changing
                // Input.main.Perspective.performed += _ => MainCamera.ChangeCamera();
                // // jumping
                // Input.main.Jump.performed += _ => _playerController.playerMovement.Jump();
                // // sliding
                // Input.main.Slide.performed += i => _playerController.playerMovement.CheckIfCanSlide();
                // Input.main.Slide.canceled += i => _playerController.playerMovement.CancelSlide();
            }
            Input.Enable();
            _mainCamera = GetComponent<MainCamera>();
            _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            Cursor.lockState = CursorLockMode.Locked;

        }
        
    }
}
