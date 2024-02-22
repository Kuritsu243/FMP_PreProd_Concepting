using Player;
using UnityEngine;

namespace Weapons.Enemy
{
    public class EnemyPistol : EnemyBaseWeapon
    {
        public override void Fire()
        {
            Debug.LogWarning("Enemy shooting");
            if (weaponAction != WeaponState.Idle) return;
            var direction = GetWeaponSpread(spawnPosition);
            if (Physics.Raycast(spawnPosition.position, direction, out RaycastHit hit, weaponRange) &&
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
        }


        public override void Reload()
        {
            base.Reload();
            
        }
        
    }
}
