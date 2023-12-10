using System;
using Camera;
using Unity.Cinemachine;
using input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerLook : MonoBehaviour
    {
        [Header("Mouse Options")] 
        [SerializeField] private float mouseSensitivityX;
        [SerializeField] private float mouseSensitivityY;
        [Header("Camera")] 
        [SerializeField] private float xClamp;
        [SerializeField] private float rotationSpeed;
        
        
        private MainCamera _mainCamera;
        private PlayerController _playerController;
        private CharacterController _characterController;

        private inputSystem _inputSystem;
        private GameObject _playerMesh;

        private float _xRotation;
        private Vector3 _targetRotation;

        private CinemachineCamera thirdPersonCam;
        private CinemachineCamera firstPersonCam;

        private float MouseX => _inputSystem.MouseX * mouseSensitivityX;
        private float MouseY => _inputSystem.MouseY * mouseSensitivityY;
        
        private Transform PlayerTransform => _characterController.transform;
        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _mainCamera = _playerController.mainCamera.Instance;
            _inputSystem = _playerController.inputSystem.Instance;
            _playerMesh = _playerController.playerMesh;
            _characterController = _playerController.characterController;
        }
        
        private void FixedUpdate()
        {
            HandleRotation();
        }

        private void HandleRotation()
        {
            // CameraChanger.GetActiveCams(out thirdPersonCam, out firstPersonCam);
            // switch (MainCamera.ActiveCameraMode)
            // {
            //     case CameraChanger.CameraModes.FirstPerson:
            //         _playerMesh.transform.Rotate(Vector3.up, MouseX * Time.deltaTime);
            //         _xRotation -= MouseY;
            //         _xRotation = Mathf.Clamp(_xRotation, -xClamp, xClamp);
            //         _targetRotation = _playerMesh.transform.eulerAngles;
            //         _targetRotation.x = _xRotation;
            //         firstPersonCam.transform.eulerAngles = _targetRotation;
            //         break;
            //     case CameraChanger.CameraModes.ThirdPerson:
            //         // var cameraPos = thirdPersonCam.transform.position;
            //         // var playerPos = PlayerTransform.position;
            //         // var viewDir = playerPos - new Vector3(cameraPos.x, playerPos.y, cameraPos.z);
            //         // PlayerTransform.forward = viewDir.normalized;
            //         //
            //         //
            //         // var inputDir =
            //         //     PlayerTransform.forward * _inputSystem.VerticalInput +
            //         //     PlayerTransform.right * _inputSystem.HorizontalInput;
            //         //
            //         //
            //         // if (inputDir != Vector3.zero)
            //         //     _playerMesh.transform.forward = Vector3.Slerp(_playerMesh.transform.forward,
            //         //         inputDir.normalized, Time.deltaTime * rotationSpeed);
            //         // _playerMesh.transform.Rotate(Vector3.up, MouseX * Time.deltaTime);
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
            
        }
        
        
        
    }
}
