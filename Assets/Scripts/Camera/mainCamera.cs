using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;


namespace Camera
{
    public class MainCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera firstPersonCam;
        [SerializeField] private CinemachineFreeLook thirdPersonCam;
        
        [Header("Testing")]
        [SerializeField] private bool isTesting;
        [Range(0, 360)] [SerializeField] private float xRotation;
        [Range(0, 360)] [SerializeField] private float yRotation;
        [Range(0, 360)] [SerializeField] private float zRotation;
        [Range(0, 360)] [SerializeField] private float dutch;
        [Range(0, 250)] [SerializeField] private float fov;
        [SerializeField] private bool lerpFOV;
        
        public static CameraChanger.CameraModes ActiveCameraMode => CameraChanger.GetActiveCamera();

        private CinemachinePOV _cinemachinePov;
        public MainCamera Instance { get; private set; }
        
        private void OnEnable()
        {
            CameraChanger.SetCams(thirdPersonCam, firstPersonCam);
            CameraChanger.SwitchToFirstPerson();
        }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        public static void ChangeCamera()
        {
            switch (ActiveCameraMode)
            {
                case CameraChanger.CameraModes.FirstPerson:
                    CameraChanger.SwitchToThirdPerson();
                    break;
                case CameraChanger.CameraModes.ThirdPerson:
                    CameraChanger.SwitchToFirstPerson();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static CameraChanger.CameraModes GetActiveMode()
        {
            return CameraChanger.GetActiveCamera();
        }
        

        private void FixedUpdate()
        {
            if (isTesting)
                Testing();
        }

        private void Testing()
        {
            
        }

        public void SetSensitivity(float sensitivity)
        {
            if (_cinemachinePov == null)
            {
                _cinemachinePov = firstPersonCam.GetCinemachineComponent<CinemachinePOV>();
                var currentSens = _cinemachinePov.m_VerticalAxis.m_MaxSpeed;
                var newSens = currentSens * sensitivity / 100;
                _cinemachinePov.m_VerticalAxis.m_MaxSpeed = newSens;
                _cinemachinePov.m_HorizontalAxis.m_MaxSpeed = newSens;
            }
            else
            {
                var currentSens = _cinemachinePov.m_VerticalAxis.m_MaxSpeed;
                var newSens = currentSens * sensitivity / 100;
                _cinemachinePov.m_VerticalAxis.m_MaxSpeed = newSens;
                _cinemachinePov.m_HorizontalAxis.m_MaxSpeed = newSens;   
            }
            
        }

        public static void DoFov(float endValue)
        {
            switch (ActiveCameraMode)
            {
                case CameraChanger.CameraModes.FirstPerson:
                    CameraChanger.FirstPersonCam.LerpFirstFOV(endValue, 0.25f);
                    break;
                case CameraChanger.CameraModes.ThirdPerson:
                    CameraChanger.ThirdPersonCam.LerpThirdFOV(endValue, 0.25f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void DoTilt(float endValue)
        {
            switch (ActiveCameraMode)
            {
                case CameraChanger.CameraModes.FirstPerson:
                    CameraChanger.FirstPersonCam.LerpFirstDutch(endValue, 0.25f);
                    break;
                case CameraChanger.CameraModes.ThirdPerson:
                    CameraChanger.ThirdPersonCam.LerpThirdDutch(endValue, 0.25f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
    }
}
