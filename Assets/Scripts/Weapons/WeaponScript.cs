using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    public class WeaponScript : MonoBehaviour
    {

        [SerializeField] private BaseWeapon weapon;

        public BaseWeapon Weapon => weapon;

        public void PistolFire()
        {
            Debug.LogWarning("pow pow!");
        }

        public void RifleFire()
        {
            
        }

        public void ShotgunFire()
        {
            
        }

        public void Punch()
        {
            
        }

        public void Reload()
        {
            
        }







    }
}
