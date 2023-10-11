using System;
using Camera;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace input
{
    public class inputSystem : MonoBehaviour
    {
        public static inputSystem Instance { get; private set; }
        
        public float InputX { get; set; }
        public float InputY { get; set; }
        public float InputZ { get; set; }


        private PlayerActions _input;
        private Vector2 _movementInput;
        private mainCamera _mainCamera;
        
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
                _input.main.Perspective.performed += _ => _mainCamera.ChangeCamera();
            }
            _input.Enable();

            _mainCamera = GetComponent<mainCamera>();
            Cursor.lockState = CursorLockMode.Locked;

        }
    }
}
