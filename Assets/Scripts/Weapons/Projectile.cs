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

        protected float ProjectileDamage { get; set; }
        private float ProjectileSpeed { get; set; }
        private float ProjectileDespawnTime { get; set; }
        private Vector3 ProjectileSpawnDirection { get; set; }

        protected Collider ProjectileCollider;
        private Rigidbody _projectileRigidbody;



        
        public void Initialize(float damage, float projSpeed, float despawnTime, Vector3 spawnDir)
        {
            ProjectileDamage = damage;
            ProjectileSpeed = projSpeed;
            ProjectileDespawnTime = despawnTime;
            ProjectileSpawnDirection = spawnDir;
            ProjectileCollider = GetComponent<Collider>();
            _projectileRigidbody = GetComponent<Rigidbody>();
            Invoke(nameof(Despawn), despawnTime);
            _projectileRigidbody.velocity = (spawnDir + transform.forward) * projSpeed;
        }

        public void Despawn()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            switch (other.transform.root.tag)
            {
                case "Player":
                    Physics.IgnoreCollision(other, ProjectileCollider);
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
