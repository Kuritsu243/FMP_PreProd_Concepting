using UnityEngine;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {

        [SerializeField] private float maxHealth = 10;
        
        public float CurrentHealth { get; set; }


        public void Damage(float amount)
        {
            CurrentHealth -= amount;
            if (CurrentHealth <= 0) Die();
        }

        private void Die()
        {
            Debug.LogWarning("Player Dead");
        }

    }
}
