using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Cameras
{
    public class ThirdPersonAim : MonoBehaviour
    {
        [FormerlySerializedAs("_thirdPersonCam")] [SerializeField] private CinemachineCamera thirdPersonCam;
        [FormerlySerializedAs("_playerInput")] [SerializeField] private PlayerInput playerInput;
        [FormerlySerializedAs("_playerObject")] [SerializeField] private GameObject playerObject;
        private Vector2 _mouseInput;
        private Quaternion _nextRotation;
        private Vector3 _nextPosition;

        [SerializeField] private float rotationPower = 3f;
        [SerializeField] private float rotationLerp = 0.5f;
        
        
        private void Start()
        {
            playerInput.actions["Movement"].performed += i => i.ReadValue<Vector2>();
            playerInput.actions["Look"].performed += i => _mouseInput = i.ReadValue<Vector2>();
        }

        private void Update()
        {
            playerObject.transform.rotation *= Quaternion.AngleAxis(_mouseInput.x * rotationPower, Vector3.up);
            playerObject.transform.rotation *= Quaternion.AngleAxis(_mouseInput.y * rotationPower, Vector3.right);

            var angles = playerObject.transform.localEulerAngles;
            angles.z = 0;

            var angle = playerObject.transform.localEulerAngles.x;

            angles.x = angle switch
            {
                > 180 and < 340 => 340,
                > 40 and < 180 => 40,
                _ => angles.x
            };

            playerObject.transform.localEulerAngles = angles;

            _nextRotation = Quaternion.Lerp(playerObject.transform.rotation, _nextRotation,
                Time.deltaTime * rotationLerp);

            transform.rotation = Quaternion.Euler(0, playerObject.transform.rotation.eulerAngles.y, 0);
            playerObject.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        }
    }
    
}
