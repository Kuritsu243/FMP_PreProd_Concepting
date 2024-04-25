using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 10;
        public float CurrentHealth { get; private set; }

        public float MaxHealth => maxHealth;


        public void Damage(float amount)
        {
            CurrentHealth -= amount;
            if (CurrentHealth <= 0) Die();
        }

        private void Start()
        {
            CurrentHealth = maxHealth;
        }

        private static void Die()
        {
            LoadNextScene();
        }

        private static void LoadNextScene()
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(2);
        }
    }
}