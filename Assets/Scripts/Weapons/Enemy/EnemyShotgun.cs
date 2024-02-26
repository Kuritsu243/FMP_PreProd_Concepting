using System.Collections.Generic;
using UnityEngine;

namespace Weapons.Enemy
{
    public class EnemyShotgun : EnemyBaseWeapon
    {
        public override void Fire()
        {
            if (weaponAction != WeaponState.Idle) return;
            var direction = GetWeaponSpread(spawnPosition);
            if (weaponProjectile != null && shootingType == ShootingType.Projectile)
            {
                var pellets = new List<Quaternion>(shotgunPelletCount);
                for (var i = 0; i < shotgunPelletCount; i++) pellets.Add(Quaternion.Euler(Vector3.zero));
                for (var h = 0; h < shotgunPelletCount; h++)
                {
                    pellets[h] = Random.rotation;
                    // var pellet = Instantiate(weaponProjectile, spawnPosition.position, spawnPosition.rotation);
                    // pellet.transform.rotation =
                    //     Quaternion.RotateTowards(pellet.transform.rotation, pellets[h], weaponSpread.x);
                    //
                    //

                    var pellet = enemyProjectilePool.GetPooledProjectile();
                    if (pellet != null)
                    {
                        pellet.transform.position = spawnPosition.position;
                        pellet.transform.rotation = spawnPosition.rotation;
                        pellet.SetActive(true);
                    }
                    
                    var pelletScript = pellet.GetComponent<EnemyProjectile>();
                    pelletScript.Initialize(weaponDamage, ProjectileSpeed, ProjectileDespawnTime,
                        spawnPosition.transform.forward);
                }
            }
            base.Fire();
        }
    }
}
