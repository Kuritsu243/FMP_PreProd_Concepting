using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using TMPro;
using Tutorial;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CanvasScript : MonoBehaviour
    {
        [Header("Images")] 
        [SerializeField] private Image reloadBar;

        [SerializeField] private Image healthBar;
        
        
        
        [Header("Panels")]
        [SerializeField] private GameObject ammoPanel;
        [SerializeField] private GameObject healthPanel;
        [SerializeField] private GameObject enemiesPanel;
        
        
        [Header("Text")] 
        [SerializeField] private TextMeshProUGUI ammoReporter;

        [SerializeField] private TextMeshProUGUI enemiesToKill;
        
        [Header("Required Components")] 
        [SerializeField] private TutorialEnemyController tutorialEnemyController;
        [SerializeField] private TutorialController tutorialController;
        
        

        private PlayerController _player;
        private PlayerShooting _playerShooting;
        private PlayerHealth _playerHealth;

        private bool _currentlyReloading;
        private bool _enemyKillChallenge;

        private int _numberOfEnemies;
        


        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            _playerShooting = _player.playerShooting;
            _playerHealth = _player.playerHealth;
            reloadBar.fillAmount = 0f;
            ammoPanel.SetActive(false);
            enemiesPanel.SetActive(false);
            healthPanel.SetActive(true);
        }

        private void FixedUpdate()
        {
            if (ammoPanel.activeSelf && Math.Abs(reloadBar.fillAmount - 1f) < 0.01f && !_currentlyReloading)
                reloadBar.gameObject.SetActive(false);
            
            if (_playerShooting.HasWeapon() && !ammoPanel.activeSelf)
                ammoPanel.SetActive(true);

            if (healthPanel.activeSelf)
                healthBar.fillAmount = _playerHealth.CurrentHealth / _playerHealth.MaxHealth;
            
            switch (_enemyKillChallenge)
            {
                case true when !enemiesPanel.activeSelf:
                    enemiesPanel.SetActive(true);
                    break;
                case true when enemiesPanel.activeSelf:
                    enemiesToKill.text = $"Remaining: \n{tutorialEnemyController.EnemiesRemaining} / {_numberOfEnemies}";
                    break;
                case false when enemiesPanel.activeSelf:
                    enemiesPanel.SetActive(false);
                    break;
            }

            if (_playerShooting.HasWeapon() && ammoPanel.activeSelf)
                ammoReporter.text =
                    $"{_playerShooting.CurrentWeapon.CurrentPrimaryAmmo} / {_playerShooting.CurrentWeapon.CurrentSecondaryAmmo}";

        }



        public void Reload(float reloadTime)
        {
            _currentlyReloading = true;
            reloadBar.gameObject.SetActive(true);
            LeanTween.value(reloadBar.gameObject, 0f, 1f, reloadTime).setOnUpdate(val =>
            {
                var i = reloadBar.fillAmount;
                i = val;
                reloadBar.fillAmount = i;
            }).setOnComplete(() =>
            {
                _currentlyReloading = false;
            });
        }

        public void ShowKillChallengeUI(int enemies)
        {
            _numberOfEnemies = enemies;
            StartCoroutine(EnemyKillChallenge());
        }

        private IEnumerator EnemyKillChallenge()
        {
            _enemyKillChallenge = true;
            yield return new WaitUntil(() => tutorialEnemyController.EnemiesRemaining == 0);
            _enemyKillChallenge = false;
            tutorialController.EnemyChallengeComplete();

        }
    }
}
