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
            var direction = GetWeaponSpread(spawnPosition);
            if (Physics.Raycast(spawnPosition.position, direction, out RaycastHit hit, weaponRange) && shootingType == ShootingType.Hitscan)
            {
                switch (hit.transform.root.tag)
                {
                    case "Enemy":
                        var collidedEnemy = hit.transform.root.gameObject;
                        Debug.LogWarning("hit enemy!!");
                        break;
                }
            }
            else if (weaponProjectile != null)
            {
                // do projectile shit
            }

        }

        public override void Reload()
        {
            base.Reload();
        }
    }
    
}
