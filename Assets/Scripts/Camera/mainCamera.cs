using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

namespace Camera
{
    public static class CameraSwitcher
    {
        public enum CameraModes
        {
            FirstPerson,
            ThirdPerson
        }

        public enum ThirdPersonCameraModes
        {
            Basic,
            Combat,
            Freelook
        }
        
        public static CinemachineVirtualCamera FirstPersonCam { get; private set; }

        public static CinemachineFreeLook ThirdPersonCam { get; private set; }

        private static CameraModes _cameraTypes;
        private static ThirdPersonCameraModes _thirdPersonModes;
        
        public static void SwitchToFirstPerson()
        {
            Debug.Log("Switching from " + ThirdPersonCam + " to " + FirstPersonCam);
            ThirdPersonCam.Priority = 0;
            FirstPersonCam.Priority = 10;
            _cameraTypes = CameraModes.FirstPerson;
        }

        public static void SwitchToThirdPerson()
        {
            Debug.Log("Switching from " + FirstPersonCam + " to " + ThirdPersonCam);
            FirstPersonCam.Priority = 0;
            ThirdPersonCam.Priority = 10;
            _cameraTypes = CameraModes.ThirdPerson;
        }

        public static void ChangeThirdPerspective()
        {

        }
        

        public static CameraModes GetActiveCamera()
        {
            return _cameraTypes;
        }

        public static void GetActiveCams(out CinemachineFreeLook thirdCam, out CinemachineVirtualCamera firstCam)
        {
            thirdCam = ThirdPersonCam;
            firstCam = FirstPersonCam;
        }

        public static void SetCams(CinemachineFreeLook thirdCam, CinemachineVirtualCamera firstCam)
        {
            FirstPersonCam = firstCam;
            ThirdPersonCam = thirdCam;
        }
    }

    public static class CinemachineExtras
    {
        public static void LerpFirstDutch(this CinemachineVirtualCamera vcam, float endValue, float timeToTake)
        {
            vcam.StartCoroutine(DutchLerp());
            IEnumerator DutchLerp()
            {
                var start = vcam.m_Lens.Dutch;
                var timeElapsed = 0f;
                while (timeElapsed < timeToTake)
                {
                    var dutchValue = Mathf.Lerp(start, endValue, timeElapsed / timeToTake);
                    vcam.m_Lens.Dutch = dutchValue;
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }
        
        public static void LerpThirdDutch(this CinemachineFreeLook vcam, float endValue, float timeToTake)
        {
            vcam.StartCoroutine(DutchLerp());
            IEnumerator DutchLerp()
            {
                var start = vcam.m_Lens.Dutch;
                var timeElapsed = 0f;
                while (timeElapsed < timeToTake)
                {
                    var dutchValue = Mathf.Lerp(start, endValue, timeElapsed / timeToTake);
                    vcam.m_Lens.Dutch = dutchValue;
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }
        
        public static void LerpFirstFOV(this CinemachineVirtualCamera vcam, float endValue, float timeToTake)
        {
            vcam.StartCoroutine(DoLerp());
            IEnumerator DoLerp()
            {
                var start = vcam.m_Lens.FieldOfView;
                var timeElapsed = 0f;
                while (timeElapsed < timeToTake)
                {
                    var fovValue = Mathf.Lerp(start, endValue, timeElapsed / timeToTake);
                    vcam.m_Lens.FieldOfView = fovValue;
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }

        public static void LerpThirdFOV(this CinemachineFreeLook vcam, float endValue, float timeToTake)
        {
            vcam.StartCoroutine(DoLerp());
            IEnumerator DoLerp()
            {
                var start = vcam.m_Lens.FieldOfView;
                var timeElapsed = 0f;
                while (timeElapsed < timeToTake)
                {
                    var fovValue = Mathf.Lerp(start, endValue, timeElapsed / timeToTake);
                    vcam.m_Lens.FieldOfView = fovValue;
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
            }
            
        }



        
    }
    
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
        
        public static CameraSwitcher.CameraModes ActiveCameraMode => CameraSwitcher.GetActiveCamera();
        
        public MainCamera Instance { get; private set; }
        
        private void OnEnable()
        {
            CameraSwitcher.SetCams(thirdPersonCam, firstPersonCam);
            CameraSwitcher.SwitchToFirstPerson();
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
                case CameraSwitcher.CameraModes.FirstPerson:
                    CameraSwitcher.SwitchToThirdPerson();
                    break;
                case CameraSwitcher.CameraModes.ThirdPerson:
                    CameraSwitcher.SwitchToFirstPerson();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static CameraSwitcher.CameraModes GetActiveMode()
        {
            return CameraSwitcher.GetActiveCamera();
        }
        

        private void FixedUpdate()
        {
            if (isTesting)
                Testing();
        }

        private void Testing()
        {
            
        }

        public static void DoFov(float endValue)
        {
            switch (ActiveCameraMode)
            {
                case CameraSwitcher.CameraModes.FirstPerson:
                    CameraSwitcher.FirstPersonCam.LerpFirstFOV(endValue, 0.25f);
                    break;
                case CameraSwitcher.CameraModes.ThirdPerson:
                    CameraSwitcher.ThirdPersonCam.LerpThirdFOV(endValue, 0.25f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void DoTilt(float endValue)
        {
            switch (ActiveCameraMode)
            {
                case CameraSwitcher.CameraModes.FirstPerson:
                    CameraSwitcher.FirstPersonCam.LerpFirstDutch(endValue, 0.25f);
                    break;
                case CameraSwitcher.CameraModes.ThirdPerson:
                    CameraSwitcher.ThirdPersonCam.LerpThirdDutch(endValue, 0.25f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
    }
}
