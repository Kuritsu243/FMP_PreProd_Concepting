using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class EnemyHealth : MonoBehaviour
    {

        [SerializeField] private float maxHealth = 5f;
        private Animator _enemyAnimator;
        private Collider _enemyCollider;
        private NavMeshAgent _navMeshAgent;
        private EnemyShooting _enemyShooting;
        private EnemyController _enemyController;
        private static readonly int IsDead = Animator.StringToHash("isDead");
        public float CurrentHealth { get; set; }


        private void Start()
        {
            CurrentHealth = maxHealth;
            _enemyController = GetComponent<EnemyController>();
            _enemyShooting = GetComponent<EnemyShooting>();
            _enemyAnimator = GetComponentInChildren<Animator>();
            _enemyCollider = GetComponentInChildren<Collider>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
        
        private void FixedUpdate()
        {
            if (CurrentHealth <= 0) StartCoroutine(Die());
        }

        public void Damage(float value)
        {
            CurrentHealth -= value;
        }

        private IEnumerator Die()
        {
            _enemyAnimator.SetBool(IsDead, true);
            Destroy(_navMeshAgent);
            Destroy(_enemyShooting);
            Destroy(_enemyController);
            Destroy(_enemyCollider);
            yield return new WaitForSeconds(5f);
            Destroy(gameObject);
        }

        // private void Despawn()
        // {
        //     Destroy(this.gameObject);
        // }
    }
}
