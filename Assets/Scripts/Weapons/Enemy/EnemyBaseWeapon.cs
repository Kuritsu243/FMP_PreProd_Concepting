using System;
using System.Collections;
using AI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons.Enemy
{
    
    public class EnemyBaseWeapon : BaseWeapon
    {
        public EnemyProjectilePool enemyProjectilePool;

        private void Awake()
        {
            enemyProjectilePool = transform.root.GetComponent<EnemyProjectilePool>();
        }
    }
}
