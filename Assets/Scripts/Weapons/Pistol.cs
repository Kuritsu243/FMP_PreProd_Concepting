using System;
using UnityEngine;

namespace Weapons
{
    public class Pistol : BaseWeapon
    {
        public override void Fire()
        {
            Debug.LogWarning(weaponAction);
            if (weaponAction != WeaponState.Idle) return;
            Debug.Log("pistol firing");
            base.Fire();
        }

        public override void Reload()
        {
            base.Reload();
        }
    }
    
}
