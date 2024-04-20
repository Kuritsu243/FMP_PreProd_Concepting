using System;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CanvasScript : MonoBehaviour
    {
        [Header("Images")] 
        [SerializeField] private Image reloadBar;
        
        
        
        [Header("Panels")]
        [SerializeField] private GameObject ammoPanel;
        
        [Header("Text")] 
        [SerializeField] private TextMeshProUGUI ammoReporter;


        private PlayerController _player;
        private PlayerShooting _playerShooting;
        private PlayerHealth _playerHealth;

        private bool _currentlyReloading;
        


        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            _playerShooting = _player.playerShooting;
            _playerHealth = _player.playerHealth;
            reloadBar.fillAmount = 0f;
            ammoPanel.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (ammoPanel.activeSelf && Math.Abs(reloadBar.fillAmount - 1f) < 0.01f && !_currentlyReloading)
                reloadBar.gameObject.SetActive(false);
            
            if (_playerShooting.HasWeapon() && !ammoPanel.activeSelf)
                ammoPanel.SetActive(true);

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
    }
}
