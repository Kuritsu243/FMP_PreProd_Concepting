namespace Weapons.Enemy
{
    
    public class EnemyBaseWeapon : BaseWeapon
    {
        public EnemyProjectilePool enemyProjectilePool;

        private void Awake()
        {
            enemyProjectilePool = transform.root.GetComponentInChildren<EnemyProjectilePool>();
        }
    }
}
