using Unity.Cinemachine;
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
        [SerializeField] private float rotationSpeed;


        private float _xRotation;
        private Vector3 _targetRotation;

        private CinemachineCamera _thirdPersonCam;
        private CinemachineCamera _firstPersonCam;
        
        private void FixedUpdate()
        {
            HandleRotation();
        }

        private void HandleRotation()
        {
        }
        
        
        
    }
}
