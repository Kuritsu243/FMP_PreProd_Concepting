using System.Collections;
using Player;
using Tutorial;
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
        public LayerMask layersToHitScan;
        public Vector3 weaponSpread;
        public WeaponState weaponAction;
        public ShootingType shootingType;
        
        private int currentPrimaryAmmo;
        private int currentSecondaryAmmo;
        public bool needsToReload;

        [Header("Projectile Specific Settings")] 
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float projectileDespawnTime;

        [Header("Bullet Casing Settings")] 
        public GameObject pistolBulletCasing;
        public GameObject shotgunBulletCasing;
        public Transform bulletCasingSpawnPos;

        [Header("Hands / Armature Settings")] 
        public GameObject playerArmature;
        public Animator armatureAnimator;

        [Header("Tutorial Related Settings")]
        public TutorialController tutorialController;
        
        public Transform spawnPosition;
        public ParticleSystem muzzleFlash;
        public Animator weaponAnimator;
        private static readonly int IsShooting = Animator.StringToHash("isShooting");
        public PlayerController playerController;


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
            if (weaponAction == WeaponState.Reloading) return;
            if (currentPrimaryAmmo == maxPrimaryAmmo) return;
            weaponAction = WeaponState.Reloading;
            if (currentPrimaryAmmo <= 0 && currentSecondaryAmmo <= 0)
            {
                weaponAction = WeaponState.NoAmmo;
                return;
            }
            var newAmmo = Mathf.Clamp(currentPrimaryAmmo + currentSecondaryAmmo, 0, maxPrimaryAmmo);
            StartCoroutine(ReloadCooldown(newAmmo));
            if (playerController) playerController.canvasScript.Reload(weaponReloadTime);
        }

        public virtual void Fire()
        {
            if (weaponAction != WeaponState.Idle) return;
            if (currentPrimaryAmmo <= 0)
            {
                needsToReload = true;
                return;
                
            }
            // armatureAnimator.SetInteger(IsShooting, 1);
            // weaponAnimator.SetInteger(IsShooting, 1);
            // muzzleFlash.Play();
            currentPrimaryAmmo--;

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
            currentSecondaryAmmo -= maxPrimaryAmmo - currentPrimaryAmmo;
            currentSecondaryAmmo = Mathf.Clamp(currentSecondaryAmmo, 0, maxSecondaryAmmo);
            currentPrimaryAmmo = newPrimary;
            currentPrimaryAmmo = Mathf.Clamp(currentPrimaryAmmo, 0, maxPrimaryAmmo);
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

        public void EndOfAnimation()
        {
            // weaponAnimator.SetInteger(IsShooting, 0);
            // armatureAnimator.SetInteger(IsShooting, 0);
        }
    }
}