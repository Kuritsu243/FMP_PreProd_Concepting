using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    public class BaseWeapon : MonoBehaviour
    {
        public enum WeaponState 
        {
            Firing,
            Reloading,
            Idle,
            NoAmmo
        }
        public float weaponReloadTime;
        public float weaponFireRate;
        public int weaponDamage;
        public int weaponRange;
        public int maxPrimaryAmmo;
        public int maxSecondaryAmmo;
        
        public WeaponState weaponAction;

        private int currentPrimaryAmmo;
        private int currentSecondaryAmmo;
        private bool needsToReload;

        public int CurrentPrimaryAmmo
        {
            get => currentPrimaryAmmo;
            set => currentPrimaryAmmo = value;
        }

        public int CurrentSecondaryAmmo
        {
            get => currentSecondaryAmmo;
            set => currentSecondaryAmmo = value;
        }
        
        public virtual void Reload()
        {
            weaponAction = WeaponState.Reloading;
            if (currentPrimaryAmmo <= 0 && currentSecondaryAmmo <= 0)
            {
                weaponAction = WeaponState.NoAmmo;
                return;
            }
            var newAmmo = Mathf.Clamp(currentPrimaryAmmo + currentSecondaryAmmo, 0, maxPrimaryAmmo);
            StartCoroutine(ReloadCooldown(newAmmo));
        }

        public virtual void Fire()
        {
            if (weaponAction != WeaponState.Idle) return;
            if (currentPrimaryAmmo <= 0)
            {
                needsToReload = true;
                return;
            }
            currentPrimaryAmmo--;
            StartCoroutine(WeaponCooldown());
        }

        private IEnumerator WeaponCooldown()
        {
            weaponAction = WeaponState.Firing;
            var cooldown = weaponFireRate / 10;
            yield return new WaitForSeconds(cooldown);
            weaponAction = WeaponState.Idle;
        }

        private IEnumerator ReloadCooldown(int newPrimary)
        {
            yield return new WaitForSeconds(weaponReloadTime);
            currentSecondaryAmmo -= Mathf.Abs(newPrimary - currentPrimaryAmmo);
            currentPrimaryAmmo = newPrimary;
            needsToReload = false;
            weaponAction = WeaponState.Idle;
        }
    }
}