using Player;
using UnityEngine;

namespace Weapons.Enemy
{
    public class EnemyPistol : EnemyBaseWeapon
    {
        public override void Fire()
        {
            if (weaponAction != WeaponState.Idle) return;
            if (Physics.Raycast(spawnPosition.position, spawnPosition.forward * 10, out RaycastHit hit, weaponRange) &&
                shootingType == ShootingType.Hitscan)
            {
                switch (hit.transform.root.tag)
                {
                    case "Player":
                        var collidedPlayer = hit.transform.root.gameObject;
                        collidedPlayer.GetComponent<PlayerHealth>().Damage(weaponDamage);
                        break;
                }
            }
            base.Fire();
        }
        
        
    }
}
