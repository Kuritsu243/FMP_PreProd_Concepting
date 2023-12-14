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
        
        [SerializeField] private float weaponFireRate;
        [SerializeField] private float weaponDamage;
        [SerializeField] private WeaponType weaponType;
        [SerializeField] private float weaponRange;
        [SerializeField] private float weaponPrimaryAmmo;
        [SerializeField] private float weaponSecondaryAmmo;

        public float WeaponFireRate => weaponFireRate;

        public float WeaponDamage => weaponDamage;

        public float WeaponRange => weaponRange;

        public float WeaponPrimaryAmmo => weaponPrimaryAmmo;

        public float WeaponSecondaryAmmo => weaponSecondaryAmmo;

        public WeaponType WeaponVariant => weaponType;


    }
}