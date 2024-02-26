using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private void Start()
        {
            CurrentHealth = maxHealth;
        }

        private void Die()
        {
            LoadNextScene();
        }

        private void LoadNextScene()
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(2);
        }

        private IEnumerator LoadLevel(int levelIndex)
        {
            // todo: add crossfade
            
            SceneManager.LoadSceneAsync(levelIndex);
            yield break;
        }

    }
}
