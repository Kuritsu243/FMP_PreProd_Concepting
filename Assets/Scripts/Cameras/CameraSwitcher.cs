using Unity.Cinemachine;

namespace Cameras
{
    
    // DEPRECATED
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

        public static CinemachineCamera FirstPersonCam
        {
            get; 
            private set; 
        }

        public static CinemachineCamera ThirdPersonCam
        {
            get;
            private set;
        }

        private static CameraModes _cameraModes;

        private static ThirdPersonCameraModes _thirdPersonCameraModes;

        public static void SwitchToFirstPerson()
        {
            ThirdPersonCam.Priority.Value = 0;
            FirstPersonCam.Priority.Value = 10;
            _cameraModes = CameraModes.FirstPerson;
        }

        public static void SwitchToThirdPerson()
        {
            FirstPersonCam.Priority.Value = 0;
            ThirdPersonCam.Priority.Value = 10;
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

        public static void GetActiveCams(out CinemachineCamera thirdCam, out CinemachineCamera firstCam)
        {
            thirdCam = ThirdPersonCam;
            firstCam = FirstPersonCam;
        }

        public static void SetCams(CinemachineCamera thirdCam, CinemachineCamera firstCam)
        {
            ThirdPersonCam = thirdCam;
            FirstPersonCam = firstCam;
        }
    }
}
