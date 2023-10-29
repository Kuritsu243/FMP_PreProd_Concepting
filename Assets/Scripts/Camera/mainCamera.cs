using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using Cinemachine;

namespace Camera
{
    public static class CameraSwitcher
    {
        private static readonly List<CinemachineVirtualCamera> Cameras = new List<CinemachineVirtualCamera>();
        private static readonly List<CinemachineFreeLook> FreeLookCamera = new List<CinemachineFreeLook>();
        private static CinemachineVirtualCamera _activeCamera;
        private static CinemachineFreeLook _activeFreeLookCamera;
        public static bool IsUsingFreeLook = false;
        public static CinemachineVirtualCamera ActiveCamera => _activeCamera;
        public static CinemachineFreeLook ActiveFreeLookCamera => _activeFreeLookCamera;

        public static void Register(CinemachineVirtualCamera camera)
        {
            Debug.Log("Registered " + camera);
            Cameras.Add(camera);
        }

        public static void RegisterFreeLook(CinemachineFreeLook camera)
        {
            Debug.Log("Registered " + camera);
            FreeLookCamera.Add(camera);
        }

        public static void Unregister(CinemachineVirtualCamera camera)
        {
            Debug.LogWarning("Unregistered " + camera);
            Cameras.Remove(camera);
        }

        public static void UnregisterFreeLook(CinemachineFreeLook camera)
        {
            Debug.LogWarning("Unregistered " + camera);
            FreeLookCamera.Remove(camera);
        }

        public static void SwitchCamera(CinemachineVirtualCamera camera)
        {
            Debug.Log("Switching from " + _activeCamera + " to " + camera);
            IsUsingFreeLook = false;
            camera.Priority = 10;
            _activeCamera = camera;
            foreach (var vcam in Cameras.Where(vcam => vcam != camera && vcam.Priority != 0))
            {
                vcam.Priority = 0;
            }
        }

        public static void SwitchToFreelookCamera(CinemachineVirtualCamera camera, CinemachineFreeLook freeLook)
        {
            Debug.Log("Switching from" + camera + " to " + freeLook);
            IsUsingFreeLook = true;
            freeLook.Priority = 10;
            foreach (var vcam in Cameras.Where(vcam => vcam.Priority != 0))
                vcam.Priority = 0;
            _activeFreeLookCamera = freeLook;
            _activeCamera = null;
        }

        public static void SwitchFromFreelookCamera(CinemachineVirtualCamera camera)
        {
            Debug.Log("Switching from " + _activeFreeLookCamera + " to " + camera);
            IsUsingFreeLook = false;
            _activeFreeLookCamera.Priority = 0;
            _activeFreeLookCamera = null;
            _activeCamera = camera;
            camera.Priority = 10;
            foreach (var vcam in Cameras.Where(vcam => vcam != camera && vcam.Priority != 0))
            {
                vcam.Priority = 0;
            }
            
        }

        public static bool IsActiveCamera(CinemachineVirtualCamera camera)
        {
            return camera == _activeCamera;
        }


    }

    public static class CinemachineExtras
    {
        public static void LerpDutch(this CinemachineVirtualCamera vcam, float endValue, float timeToTake)
        {
            vcam.StartCoroutine(LerpDutch());
            IEnumerator LerpDutch()
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
        
        public static void LerpFOV(this CinemachineVirtualCamera vcam, float endValue, float timeToTake)
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
    
    public class mainCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera firstPersonCam;
        [SerializeField] private CinemachineVirtualCamera thirdPersonCam;
        [SerializeField] private CinemachineFreeLook freeLookCam;
        
        [Header("Testing")]
        [SerializeField] private bool isTesting;
        [Range(0, 360)] [SerializeField] private float xRotation;
        [Range(0, 360)] [SerializeField] private float yRotation;
        [Range(0, 360)] [SerializeField] private float zRotation;
        [Range(0, 360)] [SerializeField] private float dutch;
        [Range(0, 250)] [SerializeField] private float fov;
        [SerializeField] private bool lerpFOV;
        


        
        
        
        

        
        public mainCamera Instance { get; private set; }
        
        private void OnEnable()
        {
            CameraSwitcher.Register(firstPersonCam);
            CameraSwitcher.Register(thirdPersonCam);
            CameraSwitcher.RegisterFreeLook(freeLookCam);
            CameraSwitcher.SwitchCamera(firstPersonCam);
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

        // private void OnDestroy()
        // {
        //     CameraSwitcher.Unregister(firstPersonCam);
        //     CameraSwitcher.Unregister(thirdPersonCam);
        //     CameraSwitcher.UnregisterFreeLook(freeLookCam);
        // }

        private void OnDisable()
        {
            CameraSwitcher.Unregister(firstPersonCam);
            CameraSwitcher.Unregister(thirdPersonCam);
            CameraSwitcher.UnregisterFreeLook(freeLookCam);
        }

        public void ChangeCamera()
        {
            if (CameraSwitcher.IsUsingFreeLook)
                CameraSwitcher.SwitchFromFreelookCamera(firstPersonCam);
            else
            {
                if (CameraSwitcher.IsActiveCamera(firstPersonCam))
                    CameraSwitcher.SwitchCamera(thirdPersonCam);
                else if (CameraSwitcher.IsActiveCamera(thirdPersonCam))
                    CameraSwitcher.SwitchToFreelookCamera(thirdPersonCam, freeLookCam);
            }
        }
        
        public static bool IsUsingFreelook()
        {
            return CameraSwitcher.IsUsingFreeLook;
        }

        public static CinemachineVirtualCamera GetActiveCamera()
        {
            return CameraSwitcher.ActiveCamera;
        }

        public static CinemachineFreeLook GetActiveFreelook()
        {
            return CameraSwitcher.ActiveFreeLookCamera;
            
        }

        private void FixedUpdate()
        {
            if (isTesting)
                Testing();
        }

        private void Testing()
        {
            if (IsUsingFreelook())
            {
                GetActiveFreelook().m_Lens.Dutch = dutch;
                var transformRotation = GetActiveFreelook().transform.localRotation;
                transformRotation.x = xRotation;
                transformRotation.y = yRotation;

                GetActiveFreelook().transform.rotation = transformRotation;
            }
            else
            {
                GetActiveCamera().m_Lens.Dutch = dutch;
                var transformRotation = GetActiveCamera().transform.rotation;
                transformRotation.x = xRotation;
                transformRotation.y = yRotation;
                transformRotation.z = zRotation;

                GetActiveCamera().transform.rotation = transformRotation;

                if (lerpFOV)
                {
                    GetActiveCamera().LerpFOV(120f, 0.45f);
                }

                
                
            }
        }

        public static void DoFov(float endValue)
        {
            GetActiveCamera().LerpFOV(endValue, 0.25f);
        }

        public static void DoTilt(float endValue)
        {
            GetActiveCamera().LerpDutch(endValue, 0.25f);
        }
        
        
    }
}
