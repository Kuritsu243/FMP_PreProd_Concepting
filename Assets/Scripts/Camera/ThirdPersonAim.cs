using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera
{
    public class ThirdPersonAim : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera _thirdPersonCam;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private GameObject _playerObject;
        private Vector2 _mouseInput;
        private Vector2 _moveInput;
        private Quaternion _nextRotation;
        private Vector3 _nextPosition;

        [SerializeField] private float rotationPower = 3f;
        [SerializeField] private float rotationLerp = 0.5f;
        
        
        private void Start()
        {
            _playerInput.actions["Movement"].performed += i => _moveInput = i.ReadValue<Vector2>();
            _playerInput.actions["Look"].performed += i => _mouseInput = i.ReadValue<Vector2>();
        }

        private void Update()
        {
            _playerObject.transform.rotation *= Quaternion.AngleAxis(_mouseInput.x * rotationPower, Vector3.up);
            _playerObject.transform.rotation *= Quaternion.AngleAxis(_mouseInput.y * rotationPower, Vector3.right);

            var angles = _playerObject.transform.localEulerAngles;
            angles.z = 0;

            var angle = _playerObject.transform.localEulerAngles.x;

            angles.x = angle switch
            {
                > 180 and < 340 => 340,
                > 40 and < 180 => 40,
                _ => angles.x
            };

            _playerObject.transform.localEulerAngles = angles;

            _nextRotation = Quaternion.Lerp(_playerObject.transform.rotation, _nextRotation,
                Time.deltaTime * rotationLerp);

            transform.rotation = Quaternion.Euler(0, _playerObject.transform.rotation.eulerAngles.y, 0);
            _playerObject.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        }
    }
    
}
