using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 5f;
        private Animator _enemyAnimator;
        private bool _hasDied;
        private Collider _enemyCollider;
        private EnemyShooting _enemyShooting;
        private EnemyController _enemyController;
        private float CurrentHealth { get; set; }
        private NavMeshAgent _navMeshAgent;
        private static readonly int IsDead = Animator.StringToHash("isDead");
        
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
            if (CurrentHealth <= 0 && !_hasDied) StartCoroutine(Die());
        }
        public void Damage(float value)
        {
            CurrentHealth -= value;
        }

        private IEnumerator Die()
        {
            _hasDied = true;
            if (_enemyController.IsTutorial)
                _enemyController.TutorialEnemyManager.EnemyKilled(_enemyController);
            
            _enemyAnimator.SetBool(IsDead, true);
            Destroy(_navMeshAgent);
            Destroy(_enemyShooting);
            Destroy(_enemyController);
            Destroy(_enemyCollider);
            yield return new WaitForSeconds(5f);
            Destroy(gameObject);
        }
    }
}
