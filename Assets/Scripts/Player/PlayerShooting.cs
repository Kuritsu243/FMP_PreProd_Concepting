using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Weapons;

namespace Player
{
    public class PlayerShooting : MonoBehaviour
    {
#region Weapon Game Objects

        [SerializeField] private GameObject pistolObject;

#endregion

        [SerializeField] private TextMeshProUGUI ammoText;


        public BaseWeapon activeWeapon;
        public WeaponScript activeWeaponScript;
        private bool _isReloading;
        private bool _needsToReload;

        private float _activeWeaponPrimaryAmmo;
        private float _activeWeaponSecondaryAmmo;

        public bool NeedsToReload => _needsToReload;
        public void Fire()
        {
            if (!activeWeapon) return;
            if (_needsToReload) StartCoroutine(Reload());
            if (activeWeapon.WeaponAction != BaseWeapon.WeaponState.Idle) return;
            if (_activeWeaponPrimaryAmmo <= 0)
            {
                _needsToReload = true;
                return;
            }

            switch (activeWeapon.WeaponVariant)
            {
                case BaseWeapon.WeaponType.Pistol:
                    activeWeaponScript.PistolFire();
                    break;
                case BaseWeapon.WeaponType.Rifle:
                    activeWeaponScript.RifleFire();
                    break;
                case BaseWeapon.WeaponType.Shotgun:
                    activeWeaponScript.ShotgunFire();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _activeWeaponPrimaryAmmo--;
            StartCoroutine(WeaponCooldown());

        }
        
        public void Equip(BaseWeapon weaponToEquip, WeaponScript weaponScriptToEquip)
        {
            activeWeaponScript = weaponScriptToEquip;
            activeWeapon = weaponToEquip;

            _activeWeaponPrimaryAmmo = activeWeapon.WeaponPrimaryAmmo;
            _activeWeaponSecondaryAmmo = activeWeapon.WeaponSecondaryAmmo;
            
            switch (activeWeapon.WeaponVariant)
            {
                case BaseWeapon.WeaponType.Pistol:
                    pistolObject.SetActive(true);
                    break;
                case BaseWeapon.WeaponType.Rifle:
                    break;
                case BaseWeapon.WeaponType.Shotgun:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IEnumerator Reload()
        {
            activeWeapon.WeaponAction = BaseWeapon.WeaponState.Reloading;

            if (_activeWeaponPrimaryAmmo <= 0 && _activeWeaponSecondaryAmmo <= 0)
            {
                activeWeapon.WeaponAction = BaseWeapon.WeaponState.NoAmmo;
                yield break;
            }
            
            var newAmmo = Mathf.Clamp(_activeWeaponPrimaryAmmo + _activeWeaponSecondaryAmmo, 0,
                activeWeapon.WeaponPrimaryAmmo);
            _activeWeaponSecondaryAmmo -= Mathf.Abs(newAmmo - _activeWeaponPrimaryAmmo);
            _activeWeaponPrimaryAmmo = newAmmo;
            
  
            yield return new WaitForSeconds(activeWeapon.WeaponReloadTime);

            activeWeapon.WeaponAction = BaseWeapon.WeaponState.Idle;
            _needsToReload = false;
        }


        private IEnumerator WeaponCooldown()
        {
            activeWeapon.WeaponAction = BaseWeapon.WeaponState.Firing;
            var cooldown = activeWeapon.WeaponFireRate / 10;
            yield return new WaitForSeconds(cooldown);
            activeWeapon.WeaponAction = BaseWeapon.WeaponState.Idle;
        }

        private void FixedUpdate()
        {
            if (!activeWeapon) return;         
            Debug.LogWarning(_activeWeaponPrimaryAmmo);
            ammoText.text = $"Primary Ammo: {_activeWeaponPrimaryAmmo}\n" +
                            $"Secondary Ammo: {_activeWeaponSecondaryAmmo}";
        }
    }
}
