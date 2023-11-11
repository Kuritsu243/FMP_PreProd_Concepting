using System;
using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerStamina : MonoBehaviour
    {
        [Header("Stamina Settings")]
        [SerializeField] private float maxStamina;
        [SerializeField] private float wallRunningDrainRate;
        [SerializeField] private float jumpingDrainRate;
        [SerializeField] private float staminaRegenRate;
        [SerializeField] private float slideStaminaCost;
        [SerializeField] private float pauseLengthUntilRegen;

        public float Stamina => _stamina;
        
        private float _stamina;
        private PlayerController _playerController;
        private PlayerMovement _playerMovement;
        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            // _playerMovement = _playerController.playerMovement;
            _stamina = maxStamina;
        }


        public IEnumerator DrainStamina(float drainRate)
        {
            if (_stamina <= 0) yield return null;
            _stamina -= drainRate * Time.deltaTime;
            
            yield return null;
        }

        private void FixedUpdate()
        {
            Debug.LogWarning(_playerMovement.GetPlayerMovementState());
        }
    }
}
