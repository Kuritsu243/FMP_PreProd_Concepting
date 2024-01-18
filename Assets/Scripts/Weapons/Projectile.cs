using System;
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

        private float projectileDamage { get; set; }
        private float projectileSpeed { get; set; }
        private float projectileDespawnTime { get; set; }
        private Vector3 projectileSpawnDirection { get; set; }

        private Collider _projectileCollider;
        private Rigidbody _projectileRigidbody;



        
        public void Initialize(float damage, float projSpeed, float despawnTime, Vector3 spawnDir)
        {
            projectileDamage = damage;
            projectileSpeed = projSpeed;
            projectileDespawnTime = despawnTime;
            projectileSpawnDirection = spawnDir;
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
                    Despawn();
                    break;
            }
        }
    }
}
