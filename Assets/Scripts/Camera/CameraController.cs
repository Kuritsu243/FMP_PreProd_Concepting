using System;
using System.Collections.Generic;
using Camera.FSM;
using Camera.FSM.States;
using input;
using Player;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using CameraState = Camera.FSM.CameraState;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
#region Required Components
        public GameObject eventSystem;
        public inputSystem inputSystem;
        public PlayerInput playerInput;
        public MainCamera mainCamera;
        public PlayerController playerController;
        
#endregion

#region Camera States
        private CameraMachine _cameraStateMachine;
        public FirstPersonState FirstPersonState;
        public ThirdPersonState ThirdPersonState;
#endregion

//todo: figure out an easier way to implement this

#region Cameras for each state
    [SerializeField] private CinemachineCamera firstPersonCam;
    [SerializeField] private CinemachineCamera thirdPersonCam;
    
#endregion

        public CameraStateMachine CameraFsm => _cameraStateMachine;
        
        [SerializeField] public List<CinemachineCamera> cinemachineCameras;


        
        public void Awake()
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            eventSystem = playerController.eventSystem;
            inputSystem = playerController.inputSystem;
            mainCamera = playerController.mainCamera;
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
