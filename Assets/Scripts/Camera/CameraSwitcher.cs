using Cinemachine;
using UnityEngine;

namespace Camera
{
    public static class CameraChanger
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
            FreeLook
        }

        public static CinemachineVirtualCamera FirstPersonCam
        {
            get; 
            private set; 
        }

        public static CinemachineFreeLook ThirdPersonCam
        {
            get;
            private set;
        }

        private static CameraModes _cameraModes;

        private static ThirdPersonCameraModes _thirdPersonCameraModes;

        public static void SwitchToFirstPerson()
        {
            ThirdPersonCam.Priority = 0;
            FirstPersonCam.Priority = 10;
            _cameraModes = CameraModes.FirstPerson;
        }

        public static void SwitchToThirdPerson()
        {
            FirstPersonCam.Priority = 0;
            ThirdPersonCam.Priority = 10;
            _cameraModes = CameraModes.ThirdPerson;
        }

        public static void ChangeThirdPersonPerspective()
        {
            // todo: Implement a finite state machine for camera
            // todo: Implement third person camera modes
        }

        public static CameraModes GetActiveCamera()
        {
            return _cameraModes;
        }

        public static void GetActiveCams(out CinemachineFreeLook thirdCam, out CinemachineVirtualCamera firstCam)
        {
            thirdCam = ThirdPersonCam;
            firstCam = FirstPersonCam;
        }

        public static void SetCams(CinemachineFreeLook thirdCam, CinemachineVirtualCamera firstCam)
        {
            ThirdPersonCam = thirdCam;
            FirstPersonCam = firstCam;
        }
    }
}
