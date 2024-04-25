using UnityEngine;

namespace Tutorial
{
    public class TutorialEnemy : MonoBehaviour
    {
        [SerializeField] private TutorialController tutorialController;
        [SerializeField] private TutorialEnemyController tutorialEnemyController;
        [SerializeField] private bool isHostile;


        private Animator _enemyAnimator;
        private Collider _enemyCollider;
        private static readonly int IsDead = Animator.StringToHash("isDead");

        private void Start()
        {
            _enemyAnimator = GetComponent<Animator>();
            _enemyCollider = GetComponentInChildren<Collider>();
        }

        public void Die()
        {
            if (isHostile) return;
            _enemyAnimator.SetBool(IsDead, true);
            tutorialController.EnemyChecks["Killed"] = true;
            tutorialController.TutorialEnemyKilled();
            Destroy(_enemyCollider);
        }
    }
}