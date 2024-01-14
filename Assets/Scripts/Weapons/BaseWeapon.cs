using System.Collections;
using UnityEngine;

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
        public float WeaponReloadTime;
        public float WeaponFireRate;
        public int WeaponDamage;
        public int WeaponRange;
        public int MaxPrimaryAmmo;
        public int MaxSecondaryAmmo;
        
        public WeaponState WeaponAction;

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
        
        public IEnumerator Reload()
        {
            WeaponAction = WeaponState.Reloading;
            if (MaxPrimaryAmmo <= 0 && MaxSecondaryAmmo <= 0)
            {
                WeaponAction = WeaponState.NoAmmo;
                yield break;
            }

            var newAmmo = Mathf.Clamp(currentPrimaryAmmo + currentSecondaryAmmo, 0, MaxPrimaryAmmo);
            yield return new WaitForSeconds(WeaponReloadTime);
            currentSecondaryAmmo -= Mathf.Abs(newAmmo - currentPrimaryAmmo);
            currentPrimaryAmmo = newAmmo;
            needsToReload = false;
        }

        public virtual void Fire()
        {
            if (WeaponAction != WeaponState.Idle) return;
            if (currentPrimaryAmmo <= 0)
            {
                needsToReload = true;
                return;
            }
            currentPrimaryAmmo--;
        }

        public IEnumerator WeaponCooldown()
        {
            WeaponAction = WeaponState.Firing;
            var cooldown = WeaponFireRate / 10;
            yield return new WaitForSeconds(cooldown);
            WeaponAction = WeaponState.Idle;
        }
    }
}