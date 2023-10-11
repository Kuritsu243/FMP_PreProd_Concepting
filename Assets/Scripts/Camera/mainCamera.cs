using System;
using System.Collections.Generic;
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
    
    
    
    public class mainCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera firstPersonCam;
        [SerializeField] private CinemachineVirtualCamera thirdPersonCam;
        [SerializeField] private CinemachineFreeLook freeLookCam;
        
        private void OnEnable()
        {
            CameraSwitcher.Register(firstPersonCam);
            CameraSwitcher.Register(thirdPersonCam);
            CameraSwitcher.RegisterFreeLook(freeLookCam);
            CameraSwitcher.SwitchCamera(firstPersonCam);
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
    }
}
