using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace Cameras
{
    [ExecuteAlways]
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Helpers/Cinemachine Mouse Look")]
    public class CinemachineMouseLook : InputAxisControllerBase<CinemachineMouseLook.MouseReader>
    {
        // value that is read by the mouse reader class
        private static float _mouseSensitivity;
        // value visible in inspector
        [SerializeField] private float mouseSens;
        // input override
        [SerializeField] private PlayerInput playerInput;
        
        private void Awake()
        {
            // non-static to static var
            UpdateSensAndSmoothing(mouseSens);
            // attempt to get player input override
            if (!playerInput)
                TryGetComponent(out playerInput);
            if (!playerInput)
                Debug.LogError("Cannot find input component");
            else
            {
                playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
                playerInput.onActionTriggered += value =>
                {
                    foreach (var controller in Controllers)
                    {
                        controller.Input.ProcessInput(value.action);
                    }
                };
            }
        }

        public void UpdateSensAndSmoothing(float newSens)
        {
            mouseSens = newSens;
            _mouseSensitivity = newSens;
            foreach (var controller in Controllers)
            {
                controller.Driver.AccelTime = (float) (0.1 * (1 / newSens));
                controller.Driver.DecelTime = (float) (0.1 * (1 / newSens));
            }
        }
        
        private void FixedUpdate()
        {
            if (Application.isPlaying)
                UpdateControllers();
        }

        [Serializable]
        public sealed class MouseReader : IInputAxisReader
        {
            public InputActionReference inputActionReference;
            private Vector2 _value;

            public void ProcessInput(InputAction action)
            {
                if (!inputActionReference || inputActionReference.action.id != action.id) return;
                if (action.expectedControlType == "Vector2")
                    _value = action.ReadValue<Vector2>();
                else
                    _value.x = _value.y = action.ReadValue<float>();
            }
            
            public float GetValue(Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
            {
                return hint == IInputAxisOwner.AxisDescriptor.Hints.Y
                    ? _value.y * -_mouseSensitivity
                    : _value.x * _mouseSensitivity;
            }
        }
  
    }
}