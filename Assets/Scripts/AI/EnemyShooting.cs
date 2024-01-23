using UnityEngine;

namespace AI
{
    public class EnemyShooting : MonoBehaviour
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

        
        [Header("Weapon Settings")]
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

  
        
        [Header("Projectile Specific Settings")] 
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float projectileDespawnTime;

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
        
        private int currentPrimaryAmmo;
        private int currentSecondaryAmmo;
        private bool needsToReload;


    }
}
