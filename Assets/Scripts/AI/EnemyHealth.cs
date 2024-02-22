using System;
using UnityEngine;

namespace AI
{
    public class EnemyHealth : MonoBehaviour
    {

        [SerializeField] private float maxHealth = 5f;
        public float CurrentHealth { get; set; }


        private void Start()
        {
            CurrentHealth = maxHealth;
        }
        
        private void FixedUpdate()
        {
            if (CurrentHealth <= 0) Die();
        }

        public void Damage(float value)
        {
            CurrentHealth -= value;
        }

        private void Die()
        {
            Destroy(this.gameObject);
        }
    }
}
