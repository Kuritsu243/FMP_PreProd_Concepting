using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cameras
{
    public class MainCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera firstPersonCam;
        [SerializeField] private CinemachineCamera thirdPersonCam;
        [FormerlySerializedAs("_cameraController")] [SerializeField] private CameraController cameraController;
        
        [Header("Testing")]
        [SerializeField] private bool isTesting;
        [Range(0, 360)] [SerializeField] private float xRotation;
        [Range(0, 360)] [SerializeField] private float yRotation;
        [Range(0, 360)] [SerializeField] private float zRotation;
        [Range(0, 360)] [SerializeField] private float dutch;
        [Range(0, 250)] [SerializeField] private float fov;
        [SerializeField] private bool lerpFOV;
        
        public static CameraChanger.CameraModes ActiveCameraMode => CameraChanger.GetActiveCamera();

        private static CinemachineCamera _previousCam;
        private static CinemachineCamera _activeCam;

        private CinemachineMouseLook _cinemachineMouseLook;
        public MainCamera Instance { get; private set; }

        
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        public static void SetActiveCamera(CinemachineCamera newCamera)
        {
            if (!_activeCam && !_previousCam) Debug.LogWarning("No active cam or previous cam assigned");
            else
            {
                _previousCam = _activeCam;
                _previousCam.Priority.Value = 0;
            }
            _activeCam = newCamera;
            _activeCam.Priority.Value = 10;
 

        }

        public static CameraChanger.CameraModes GetActiveMode()
        {
            return CameraChanger.GetActiveCamera();
        }
        
        public void SetSensitivity(float sensitivity)
        {
            if (!_cinemachineMouseLook)
                _cinemachineMouseLook = firstPersonCam.GetComponent<CinemachineMouseLook>();
            if (!_cinemachineMouseLook)
                throw new Exception("Cannot find the mouse look component!");
            _cinemachineMouseLook.UpdateSensAndSmoothing(sensitivity);

        }

        public static void DoFov(float endValue, float timeToTake)
        {
            switch (ActiveCameraMode)
            {   
                case CameraChanger.CameraModes.FirstPerson:
                    CameraChanger.FirstPersonCam.LerpFirstFOV(endValue, timeToTake);
                    break;
                case CameraChanger.CameraModes.ThirdPerson:
                    CameraChanger.ThirdPersonCam.LerpThirdFOV(endValue, timeToTake);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void DoTilt(float endValue, float timeToTake)
        {
            switch (ActiveCameraMode)
            {
                case CameraChanger.CameraModes.FirstPerson:
                    CameraChanger.FirstPersonCam.LerpFirstDutch(endValue, timeToTake);
                    break;
                case CameraChanger.CameraModes.ThirdPerson:
                    CameraChanger.ThirdPersonCam.LerpThirdDutch(endValue, timeToTake);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
    }
}
