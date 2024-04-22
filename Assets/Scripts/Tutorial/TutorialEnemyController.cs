using System;
using System.Collections.Generic;
using AI;
using UI;
using UnityEngine;

namespace Tutorial
{
    public class TutorialEnemyController : MonoBehaviour
    {
        [SerializeField] private List<EnemyController> tutorialEnemies;
        [SerializeField] private CanvasScript canvasScript;
        
        private TutorialController _tutorialController;
        
        public int EnemiesRemaining { get; set; }

        private void Start()
        {
            EnemiesRemaining = tutorialEnemies.Count;
            _tutorialController = GetComponent<TutorialController>();
        }

        public void EnemyKilled(EnemyController enemyKilled)
        {
            EnemiesRemaining--;
            tutorialEnemies.Remove(enemyKilled);
        }


        public void StartKillChallenge()
        {
            canvasScript.ShowKillChallengeUI(EnemiesRemaining);
            foreach (var enemy in tutorialEnemies)
                enemy.EnableEnemy();
        }
    }
}
