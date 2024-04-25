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

        private float _stamina;
        private PlayerMovement _playerMovement;
        private void Start()
        {
            GetComponent<PlayerController>();
            _stamina = maxStamina;
        }
        
    }
}
