using System.Collections;
using EditorExtensions;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;


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

        public enum ShootingType
        {
            Hitscan,
            Projectile
        }
        public float weaponReloadTime;
        public float weaponFireRate;
        public int weaponDamage;
        public int weaponRange;
        public int maxPrimaryAmmo;
        public int maxSecondaryAmmo;
        public int shotgunPelletCount;
        public GameObject weaponProjectile;

        public Vector3 weaponSpread;
        public WeaponState weaponAction;
        public ShootingType shootingType;
        
        private int currentPrimaryAmmo;
        private int currentSecondaryAmmo;
        private bool needsToReload;

        [ShowIfProjectile("shootingType")][Header("Projectile Specific Settings")] 
        [ShowIfProjectile("shootingType")][SerializeField] private float projectileSpeed;
        [ShowIfProjectile("shootingType")][SerializeField] private float projectileDespawnTime;
        
        

        public Transform spawnPosition;

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

        public float ProjectileSpeed
        {
            get => projectileSpeed;
            set => projectileSpeed = value;
        }

        public float ProjectileDespawnTime
        {
            get => projectileDespawnTime;
            set => projectileDespawnTime = value;
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

        protected Vector3 GetWeaponSpread(Transform weaponSpawnPos)
        {
            var direction = weaponSpawnPos.forward;

            direction += new Vector3(
                Random.Range(-weaponSpread.x, weaponSpread.x),
                Random.Range(-weaponSpread.y, weaponSpread.y),
                Random.Range(-weaponSpread.z, weaponSpread.z)
            );

            return direction;
        }
    }
}