using System;
using UnityEngine;
using Weapons.Enemy;

namespace AI
{
    public class EnemyShooting : MonoBehaviour
    {
        private enum WeaponType
        {
            Pistol,
            Shotgun
        }

        [SerializeField] private WeaponType weaponType;
        [SerializeField] private EnemyPistol pistol;
        [SerializeField] private EnemyShotgun shotgun;

        public EnemyBaseWeapon CurrentWeapon { get; private set; }

        public bool CanAttack { get; set; }

        public void Start()
        {
            CanAttack = true;
            switch (weaponType)
            {
                case WeaponType.Pistol:
                    EquipWeapon(pistol);
                    break;
                case WeaponType.Shotgun:
                    EquipWeapon(shotgun);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void EquipWeapon(EnemyBaseWeapon newWeapon)
        {
            CurrentWeapon = newWeapon;
            CurrentWeapon.CurrentPrimaryAmmo = CurrentWeapon.maxPrimaryAmmo;
            CurrentWeapon.CurrentSecondaryAmmo = CurrentWeapon.maxSecondaryAmmo;
            
        }

        public void Reload()
        {
            if (!CurrentWeapon) return;
            CurrentWeapon.Reload();
        }

        public void Fire()
        {
            if (!CurrentWeapon || !CanAttack) return;
            CurrentWeapon.Fire();
        }
    }
}
