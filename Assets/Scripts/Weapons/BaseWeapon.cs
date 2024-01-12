using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Weapons/New Weapon")]
    public class BaseWeapon : ScriptableObject
    {
        public enum WeaponType
        {
            Pistol,
            Rifle,
            Shotgun
        }
        
        public enum WeaponState
        {
            Firing,
            Reloading,
            Idle,
            NoAmmo
        }
        
        [SerializeField] private float weaponFireRate;
        [SerializeField] private float weaponDamage;
        [SerializeField] private WeaponType weaponType;
        [SerializeField] private WeaponState weaponState;
        [SerializeField] private float weaponRange;
        [SerializeField] private float weaponPrimaryAmmo;
        [SerializeField] private float weaponSecondaryAmmo;
        [SerializeField] private float weaponReloadTime;
        
        

        public float WeaponFireRate => weaponFireRate;

        public float WeaponDamage => weaponDamage;

        public float WeaponRange => weaponRange;

        public float WeaponReloadTime => weaponReloadTime;

        public float WeaponPrimaryAmmo
        {
            get => weaponPrimaryAmmo;
            set => weaponPrimaryAmmo = value;
        }

        public float WeaponSecondaryAmmo
        {
            get => weaponSecondaryAmmo;
            set => weaponSecondaryAmmo = value;
        }

        public WeaponType WeaponVariant => weaponType;

        public WeaponState WeaponAction 
        {
            get => weaponState;
            set => weaponState = value;
        }


    }
}