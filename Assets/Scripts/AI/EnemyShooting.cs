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
        
        
        private EnemyBaseWeapon _currentWeapon;

        public EnemyBaseWeapon CurrentWeapon
        {
            get => _currentWeapon;
            set => _currentWeapon = value;
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
            _currentWeapon = newWeapon;
            _currentWeapon.CurrentPrimaryAmmo = _currentWeapon.maxPrimaryAmmo;
            _currentWeapon.CurrentSecondaryAmmo = _currentWeapon.maxSecondaryAmmo;
            
        }

        public void Reload()
        {
            if (_currentWeapon == null) return;
            _currentWeapon.Reload();
        }

        public void Fire()
        {
            if (_currentWeapon == null || !CanAttack) return;
            _currentWeapon.Fire();
        }
    }
}
