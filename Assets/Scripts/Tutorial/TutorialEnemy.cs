using System;
using UnityEngine;
using AI;

namespace Tutorial
{
    public class TutorialEnemy : MonoBehaviour
    {

        [SerializeField] private TutorialController tutorialController;
        
        private Animator _enemyAnimator;
        private static readonly int IsDead = Animator.StringToHash("isDead");

        private void Start()
        {
            _enemyAnimator = GetComponent<Animator>();
        }

        public void Die()
        {
            _enemyAnimator.SetBool(IsDead, true);
            tutorialController.tutorialEnemyDead = true;
        }
    }
}
