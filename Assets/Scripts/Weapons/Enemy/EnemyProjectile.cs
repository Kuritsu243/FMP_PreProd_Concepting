using Player;
using UnityEngine;

namespace Weapons.Enemy
{
    public class EnemyProjectile : Projectile
    {
        public override void OnTriggerEnter(Collider other)
        {
            switch (other.transform.root.tag)
            {
                case "Player":
                    Debug.LogWarning("Player hit!!!");
                    if (other.transform.root.TryGetComponent<PlayerHealth>(out var playerHealthScript))
                        playerHealthScript.Damage(ProjectileDamage);
                    Despawn();
                    break;
                case "Enemy":
                    Physics.IgnoreCollision(other, ProjectileCollider);
                    break;
            }
        }
    }
}
