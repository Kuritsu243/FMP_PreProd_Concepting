using Camera;
using Cinemachine;
using input;
using UnityEngine;

namespace Player
{
    public class PlayerLook : MonoBehaviour
    {
        [Header("Mouse Options")] 
        [SerializeField] private float mouseSensitivityX;
        [SerializeField] private float mouseSensitivityY;
        [Header("Camera")] 
        [SerializeField] private float xClamp;
        
        private mainCamera _mainCamera;
        private PlayerController _playerController;
        private inputSystem _inputSystem;
        private GameObject _playerMesh;

        private float _xRotation;
        private Vector3 _targetRotation;

        private float MouseX => _inputSystem.MouseX * mouseSensitivityX;
        private float MouseY => _inputSystem.MouseY * mouseSensitivityY;
        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _mainCamera = _playerController.mainCamera.Instance;
            _inputSystem = _playerController.inputSystem.Instance;
            _playerMesh = _playerController.playerMesh;
        }
        
        private void FixedUpdate()
        {
            GetCurrentCamera();
            HandleRotation();
        }

        private void HandleRotation()
        {
            if (mainCamera.IsUsingFreelook())
            {
                var activeCam = mainCamera.GetActiveFreelook();
                _playerMesh.transform.Rotate(Vector3.up, MouseX * Time.deltaTime);
            }
            else
            {
                var activeCam = mainCamera.GetActiveCamera();
                _playerMesh.transform.Rotate(Vector3.up, MouseX * Time.deltaTime);
                _xRotation -= MouseY;
                _xRotation = Mathf.Clamp(_xRotation, -xClamp, xClamp);
                _targetRotation = _playerMesh.transform.eulerAngles;
                _targetRotation.x = _xRotation;
                activeCam.transform.eulerAngles = _targetRotation;
            }
        }

        private void GetCurrentCamera()
        {

            
            
        }
        
        
    }
}
