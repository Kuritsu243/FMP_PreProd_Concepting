using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Debugging
{
    public class VisualiseStateMachine : MonoBehaviour
    {
        [Header("Enable Visualisation")][SerializeField]
        private bool enableVisualisation;
        [Header("State Images")]
        [SerializeField] private Image idleState;
        [SerializeField] private Image airborneState;
        [SerializeField] private Image jumpingState;
        [SerializeField] private Image slidingState;
        [SerializeField] private Image sprintingState;
        [SerializeField] private Image walkingState;
        [SerializeField] private Image wallRunState;
        [SerializeField] private Image wallJumpState;
        [Header("Input Texts")]
        [SerializeField] private TextMeshProUGUI movementInputText;
        [SerializeField] private TextMeshProUGUI jumpPressedText;
        [SerializeField] private TextMeshProUGUI slidePressedText;


        private Canvas _canvas;
        private PlayerController _playerController;
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _slideAction;
        


        private void Update()
        {
            if (!enableVisualisation) return;
        }

        private void Start()
        {
            if (!enableVisualisation) enabled = false;
            _moveAction = _playerController.playerInput.actions["Movement"];
            _jumpAction = _playerController.playerInput.actions["Jump"];
            _slideAction = _playerController.playerInput.actions["Slide"];

        }
    }
}
