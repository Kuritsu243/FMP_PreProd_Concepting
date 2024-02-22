using System.Collections.Generic;
using AI;
using UnityEngine;

namespace Weapons
{
    public class Shotgun : BaseWeapon
    {
        public override void Fire()
        {
            Debug.LogWarning(weaponAction);
            if (weaponAction != WeaponState.Idle) return;
            var direction = GetWeaponSpread(spawnPosition);
            Debug.Log("shotgun firing");
            if (Physics.Raycast(spawnPosition.position, direction, out RaycastHit hit, weaponRange) &&
                shootingType == ShootingType.Hitscan)
            {
                switch (hit.transform.root.tag)
                {
                    case "Enemy":
                        var collidedEnemy = hit.transform.root.gameObject;
                        collidedEnemy.GetComponent<EnemyHealth>().Damage(weaponDamage);
                        Debug.LogWarning("hit enemy!!");
                        break;
                }
            }
            else if (weaponProjectile != null && shootingType == ShootingType.Projectile)
            {
                var pellets = new List<Quaternion>(shotgunPelletCount);
                for (var i = 0; i < shotgunPelletCount; i++) pellets.Add(Quaternion.Euler(Vector3.zero));
                for (var h = 0; h < shotgunPelletCount; h++)
                {
                    pellets[h] = Random.rotation;
                    var pellet = Instantiate(weaponProjectile, spawnPosition.position, spawnPosition.rotation);
                    pellet.transform.rotation =
                        Quaternion.RotateTowards(pellet.transform.rotation, pellets[h], weaponSpread.x);
                    var pelletScript = pellet.GetComponent<Projectile>();
                    pelletScript.Initialize(weaponDamage, ProjectileSpeed, ProjectileDespawnTime, spawnPosition.transform.forward);
                }
            }
            base.Fire();
        }

        public override void Reload()
        {
            base.Reload();
        }
    }
}
