using System;
using AI;
using UnityEngine;

namespace Weapons
{
    public class Pistol : BaseWeapon
    {
        public override void Fire()
        {
            if (weaponAction != WeaponState.Idle) return;
            var direction = GetWeaponSpread(spawnPosition);
            if (Physics.Raycast(spawnPosition.position, direction, out RaycastHit hit, weaponRange) && shootingType == ShootingType.Hitscan)
            {
                Debug.LogWarning("Raycast success");
                switch (hit.transform.root.tag)
                {
                    case "Enemy":
                        var collidedEnemy = hit.transform.root.gameObject;
                        collidedEnemy.GetComponent<EnemyHealth>().Damage(weaponDamage);
                        Debug.LogWarning("hit enemy!!");
                        break;
                }
            }
            else if (shootingType == ShootingType.Projectile)
            {
                // do projectile shit
            }
            Instantiate(pistolBulletCasing, bulletCasingSpawnPos.position, transform.rotation);
            base.Fire();
        }

        public override void Reload()
        {
            base.Reload();
        }
    }
    
}
