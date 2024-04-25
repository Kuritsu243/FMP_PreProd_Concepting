using System.Collections.Generic;
using Cameras;
using Cameras.FSM;
using Cameras.FSM.States;
using Player;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cameras
{
    public class CameraController : MonoBehaviour
    {
#region Required Components

        public PlayerInput playerInput;
        public PlayerController playerController;
        
#endregion

#region Camera States
        private CameraMachine _cameraStateMachine;
        public FirstPersonState FirstPersonState;
        public ThirdPersonState ThirdPersonState;
#endregion

#region Cameras for each state
    [SerializeField] private CinemachineCamera firstPersonCam;
    [SerializeField] private CinemachineCamera thirdPersonCam;
#endregion

        public CameraStateMachine CameraFsm => _cameraStateMachine;
        
        [SerializeField] public List<CinemachineCamera> cinemachineCameras;


        
        public void Awake()
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            playerInput = playerController.playerInput;
            _cameraStateMachine = new CameraMachine();
            
            FirstPersonState = new FirstPersonState("FirstPerson", _cameraStateMachine, this, firstPersonCam);
            cinemachineCameras.Add(firstPersonCam);
            ThirdPersonState = new ThirdPersonState("ThirdPerson", _cameraStateMachine, this, thirdPersonCam);
            cinemachineCameras.Add(thirdPersonCam);
            
            _cameraStateMachine.Initialize(FirstPersonState);
        }

        private void Update()
        {
            _cameraStateMachine.CurrentState.HandleInput();
            _cameraStateMachine.CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            _cameraStateMachine.CurrentState.PhysicsUpdate();
        }
    }
}
