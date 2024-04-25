using System.Collections;
using Player;
using Tutorial;
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
        public LayerMask layersToHitScan;
        public Vector3 weaponSpread;
        public WeaponState weaponAction;
        public ShootingType shootingType;
        public AudioClip weaponSound;
        public AudioClip reloadSound;

        [Header("Projectile Specific Settings")] 
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float projectileDespawnTime;

        [Header("Bullet Casing Settings")] 
        public GameObject pistolBulletCasing;
        public GameObject shotgunBulletCasing;
        public Transform bulletCasingSpawnPos;

        [Header("Tutorial Related Settings")] 
        public TutorialController tutorialController;
        public Transform spawnPosition;
        public PlayerController playerController;


        public int CurrentPrimaryAmmo { get; set; }

        public int CurrentSecondaryAmmo { get; set; }

        protected float ProjectileSpeed => projectileSpeed;

        protected float ProjectileDespawnTime => projectileDespawnTime;

        public virtual void Reload()
        {
            if (weaponAction == WeaponState.Reloading) return;
            if (CurrentPrimaryAmmo == maxPrimaryAmmo) return;
            weaponAction = WeaponState.Reloading;
            switch (CurrentPrimaryAmmo)
            {
                case <= 0 when CurrentSecondaryAmmo <= 0:
                    weaponAction = WeaponState.NoAmmo;
                    return;
            }

            playerController.audioSource.PlayOneShot(reloadSound);
            var newAmmo = Mathf.Clamp(CurrentPrimaryAmmo + CurrentSecondaryAmmo, 0, maxPrimaryAmmo);
            StartCoroutine(ReloadCooldown(newAmmo));
            if (playerController) playerController.canvasScript.Reload(weaponReloadTime);
        }

        public virtual void Fire()
        {
            if (weaponAction != WeaponState.Idle) return;
            if (CurrentPrimaryAmmo <= 0)
                return;
            if (playerController)
                playerController.audioSource.PlayOneShot(weaponSound);
            CurrentPrimaryAmmo--;

            StartCoroutine(WeaponCooldown());
        }

        private IEnumerator WeaponCooldown()
        {
            weaponAction = WeaponState.Firing;
            var cooldown = weaponFireRate / 10f;
            yield return new WaitForSeconds(cooldown);
            weaponAction = WeaponState.Idle;
        }

        private IEnumerator ReloadCooldown(int newPrimary)
        {
            yield return new WaitForSeconds(weaponReloadTime);
            CurrentSecondaryAmmo -= maxPrimaryAmmo - CurrentPrimaryAmmo;
            CurrentSecondaryAmmo = Mathf.Clamp(CurrentSecondaryAmmo, 0, maxSecondaryAmmo);
            CurrentPrimaryAmmo = newPrimary;
            CurrentPrimaryAmmo = Mathf.Clamp(CurrentPrimaryAmmo, 0, maxPrimaryAmmo);
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