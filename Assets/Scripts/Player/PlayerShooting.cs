using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace Player
{
    public class PlayerShooting : MonoBehaviour
    {
        [Header("Fists Class for game start or when no weapon")]
        [SerializeField] private BaseWeapon fists;

        [SerializeField] private TextMeshProUGUI ammoReporter;
        [SerializeField] private GameObject ammoPanel;

        [SerializeField] private Image reloadBar;
        
        
        private BaseWeapon currentWeapon;
        private BaseWeapon previousWeapon;
        
        
        
        public BaseWeapon CurrentWeapon
        {
            get => currentWeapon;
            set => currentWeapon = value;
        }

        public bool HasWeapon()
        {
            return currentWeapon;
        }

        public void Start()
        {
            // EquipWeapon(fists);
        }

        public void EquipWeapon(BaseWeapon newWeapon)
        {
            currentWeapon = newWeapon;
            currentWeapon.CurrentPrimaryAmmo = currentWeapon.maxPrimaryAmmo;
            currentWeapon.CurrentSecondaryAmmo = currentWeapon.maxSecondaryAmmo;
        }

        public void Reload()
        {
            if (currentWeapon == null) return;
            currentWeapon.Reload();
        }

        public void Fire()
        {
            if (currentWeapon == null || currentWeapon.CurrentPrimaryAmmo <= 0) return;
            currentWeapon.Fire();
        }

        private void FixedUpdate()
        {
            // if (ammoPanel.activeSelf && Mathf.Approximately(reloadBar.fillAmount, 100))
            //     reloadBar.gameObject.SetActive(false);
            //
            //
            // if (!currentWeapon && ammoPanel.activeSelf)
            //     ammoPanel.SetActive(false);
            // else if (currentWeapon)
            // {
            //     if (!ammoPanel.activeSelf) ammoPanel.SetActive(true);
            //     ammoReporter.text = $"{currentWeapon.CurrentPrimaryAmmo} / {currentWeapon.CurrentSecondaryAmmo}";
            // }
           
        }
    }
}
//     {
// #region Weapon Game Objects
//         [Header("Weapon Game Objects")]
//         [SerializeField] private GameObject pistolObject;
//         [SerializeField] private GameObject shotgunObject;
// #endregion
//
//
//
//         [Header("Visual UI")]
//         [SerializeField] private TextMeshProUGUI ammoText;
//
//
//         [Header("Active Weapon and their Script")]
//         public BaseWeapon activeWeapon;
//         public WeaponScript activeWeaponScript;
//         private bool _isReloading;
//         private bool _needsToReload;
//
//         private float _activeWeaponPrimaryAmmo;
//         private float _activeWeaponSecondaryAmmo;
//
//
//
//         public bool NeedsToReload => _needsToReload;
//         
//
//
//         public void Fire()
//         {
//             if (!activeWeapon) return;
//             if (_needsToReload) StartCoroutine(Reload());
//             if (activeWeapon.WeaponAction != BaseWeapon.WeaponState.Idle) return;
//             if (_activeWeaponPrimaryAmmo <= 0 && activeWeapon.WeaponVariant != BaseWeapon.WeaponType.Hands)
//             {
//                 _needsToReload = true;
//                 return;
//             }
//
//             switch (activeWeapon.WeaponVariant)
//             {
//                 case BaseWeapon.WeaponType.Pistol:
//                     activeWeaponScript.PistolFire();
//                     break;
//                 case BaseWeapon.WeaponType.Rifle:
//                     activeWeaponScript.RifleFire();
//                     break;
//                 case BaseWeapon.WeaponType.Shotgun:
//                     activeWeaponScript.ShotgunFire();
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException();
//             }
//
//             _activeWeaponPrimaryAmmo--;
//             StartCoroutine(WeaponCooldown());
//
//         }
//         
//         public void Equip(BaseWeapon weaponToEquip, WeaponScript weaponScriptToEquip)
//         {
//             activeWeaponScript = weaponScriptToEquip;
//             activeWeapon = weaponToEquip;
//
//             _activeWeaponPrimaryAmmo = activeWeapon.MaxPrimaryAmmo;
//             _activeWeaponSecondaryAmmo = activeWeapon.MaxSecondaryAmmo;
//             
//             switch (activeWeapon.WeaponVariant)
//             {
//                 case BaseWeapon.WeaponType.Hands:
//                     break;
//                 case BaseWeapon.WeaponType.Pistol:
//                     pistolObject.SetActive(true);
//                     break;
//                 case BaseWeapon.WeaponType.Rifle:
//                     break;
//                 case BaseWeapon.WeaponType.Shotgun:
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException();
//             }
//         }
//
//         public IEnumerator Reload()
//         {
//             activeWeapon.WeaponAction = BaseWeapon.WeaponState.Reloading;
//
//             if (_activeWeaponPrimaryAmmo <= 0 && _activeWeaponSecondaryAmmo <= 0)
//             {
//                 activeWeapon.WeaponAction = BaseWeapon.WeaponState.NoAmmo;
//                 yield break;
//             }
//             
//             var newAmmo = Mathf.Clamp(_activeWeaponPrimaryAmmo + _activeWeaponSecondaryAmmo, 0,
//                 activeWeapon.MaxPrimaryAmmo);
//             _activeWeaponSecondaryAmmo -= Mathf.Abs(newAmmo - _activeWeaponPrimaryAmmo);
//             _activeWeaponPrimaryAmmo = newAmmo;
//             
//             yield return new WaitForSeconds(activeWeapon.WeaponReloadTime);
//
//             activeWeapon.WeaponAction = BaseWeapon.WeaponState.Idle;
//             _needsToReload = false;
//         }
//
//
//         private IEnumerator WeaponCooldown()
//         {
//             activeWeapon.WeaponAction = BaseWeapon.WeaponState.Firing;
//             var cooldown = activeWeapon.WeaponFireRate / 10;
//             yield return new WaitForSeconds(cooldown);
//             activeWeapon.WeaponAction = BaseWeapon.WeaponState.Idle;
//         }
//
//         private void FixedUpdate()
//         {
//             if (!activeWeapon) return;         
//             Debug.LogWarning(_activeWeaponPrimaryAmmo);
//             ammoText.text = $"Primary Ammo: {_activeWeaponPrimaryAmmo}\n" +
//                             $"Secondary Ammo: {_activeWeaponSecondaryAmmo}";
//         }
//     }
