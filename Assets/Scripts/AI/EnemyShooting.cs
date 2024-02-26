using System;
using System.Collections;
using UnityEngine;
using Weapons;
using Weapons.Enemy;

namespace AI
{
    public class EnemyShooting : MonoBehaviour
    {
        public enum WeaponType
        {
            Pistol,
            Shotgun
        }

        [SerializeField] private WeaponType weaponType;
        [SerializeField] private EnemyPistol pistol;
        [SerializeField] private EnemyShotgun shotgun;
        
        
        private EnemyBaseWeapon currentWeapon;

        public EnemyBaseWeapon CurrentWeapon
        {
            get => currentWeapon;
            set => currentWeapon = value;
        }
        
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
            currentWeapon = newWeapon;
            currentWeapon.CurrentPrimaryAmmo = currentWeapon.maxPrimaryAmmo;
            currentWeapon.CurrentSecondaryAmmo = currentWeapon.maxSecondaryAmmo;
            
        }

        public void Reload()
        {
            if (currentWeapon == null) return;
            currentWeapon.Reload();
        }

        public void Fire()
        {
            if (currentWeapon == null || !CanAttack) return;
            currentWeapon.Fire();
        }
    }
}
