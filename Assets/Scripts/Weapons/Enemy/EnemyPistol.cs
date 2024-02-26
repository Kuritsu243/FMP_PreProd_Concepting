using System;
using Player;
using UnityEngine;

namespace Weapons.Enemy
{
    public class EnemyPistol : EnemyBaseWeapon
    {
        public override void Fire()
        {
            if (weaponAction != WeaponState.Idle) return;
            var direction = GetWeaponSpread(spawnPosition);
            if (Physics.Raycast(spawnPosition.position, spawnPosition.forward * 10, out RaycastHit hit, weaponRange) &&
                shootingType == ShootingType.Hitscan)
            {
                switch (hit.transform.root.tag)
                {
                    case "Player":
                        var collidedPlayer = hit.transform.root.gameObject;
                        collidedPlayer.GetComponent<PlayerHealth>().Damage(weaponDamage);
                        Debug.LogWarning("hit player");
                        break;
                }
            }
            else if (shootingType == ShootingType.Projectile)
            {
                //
            }
            base.Fire();
        }


        public void FixedUpdate()
        {
            Debug.LogWarning(weaponAction);
        }

        public override void Reload()
        {
            base.Reload();
            
        }
        
    }
}
