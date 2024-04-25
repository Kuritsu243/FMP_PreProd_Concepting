using AI;
using UnityEngine;

namespace Weapons
{
    public class Projectile : MonoBehaviour
    {
        protected float ProjectileDamage { get; private set; }
        protected Collider ProjectileCollider;
        private Rigidbody _projectileRigidbody;

        public void Initialize(float damage, float projSpeed, float despawnTime, Vector3 spawnDir)
        {
            ProjectileDamage = damage;
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
                    if (other.transform.root.TryGetComponent<EnemyHealth>(out var enemyHealthScript))
                        enemyHealthScript.Damage(ProjectileDamage);
                    Despawn();
                    break;
            }
        }
    }
}