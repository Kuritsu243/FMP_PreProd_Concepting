using System;
using AI;
using UnityEngine;

namespace Weapons
{
    public class Projectile : MonoBehaviour
    {
        public enum ProjectileType
        {
            ShotgunPellet,
            Bullet
        }

        public ProjectileType projType;

        private float ProjectileDamage { get; set; }
        private float ProjectileSpeed { get; set; }
        private float ProjectileDespawnTime { get; set; }
        private Vector3 ProjectileSpawnDirection { get; set; }

        private Collider _projectileCollider;
        private Rigidbody _projectileRigidbody;



        
        public void Initialize(float damage, float projSpeed, float despawnTime, Vector3 spawnDir)
        {
            ProjectileDamage = damage;
            ProjectileSpeed = projSpeed;
            ProjectileDespawnTime = despawnTime;
            ProjectileSpawnDirection = spawnDir;
            _projectileCollider = GetComponent<Collider>();
            _projectileRigidbody = GetComponent<Rigidbody>();
            Invoke(nameof(Despawn), despawnTime);
            _projectileRigidbody.velocity = (spawnDir + transform.forward) * projSpeed;
        }

        private void Despawn()
        {
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.transform.root.tag)
            {
                case "Player":
                    Physics.IgnoreCollision(other, _projectileCollider);
                    break;
                case "Enemy":
                    Debug.LogWarning("Enemy hit!!!");
                    if (other.transform.root.TryGetComponent<EnemyHealth>(out var enemyHealthScript))
                        enemyHealthScript.Damage(ProjectileDamage);
                    Despawn();
                    break;
            }
        }
    }
}
