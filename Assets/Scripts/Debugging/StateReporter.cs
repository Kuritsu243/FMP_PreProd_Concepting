using UnityEngine;
using Cameras;
using Player;
using TMPro;

namespace Debugging
{
    public class StateReporter : MonoBehaviour
    {
        [SerializeField] private CameraController cameraController;
        [SerializeField] private PlayerController playerController;

        private TextMeshProUGUI _text;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void FixedUpdate()
        {
            _text.text = $"Player State: {playerController.PlayerFsm.CurrentState}\n" +
                         $"Camera State: {cameraController.CameraFsm.CurrentState}";
        }
    }
}
