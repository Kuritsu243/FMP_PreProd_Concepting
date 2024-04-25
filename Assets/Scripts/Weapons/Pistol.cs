using AI;
using Tutorial;
using UnityEngine;

namespace Weapons
{
    public class Pistol : BaseWeapon
    {
        public override void Fire()
        {
            if (tutorialController && !tutorialController.hasFiredPistolYet)
                tutorialController.EnemyChecks["Fired"] = true;
            if (weaponAction != WeaponState.Idle) return;
            GetWeaponSpread(spawnPosition);
            playerController.activeCinemachineBrain.TryGetComponent<Camera>(out var activeCam);
            var rayOrigin = new Ray(activeCam.transform.position, activeCam.transform.forward);
            if (Physics.Raycast(rayOrigin, out RaycastHit hit, weaponRange, layersToHitScan) && shootingType == ShootingType.Hitscan)
            {
                switch (hit.transform.tag)
                {
                    case "EnemyMesh":
                        var collidedEnemyMesh = hit.transform.parent.gameObject;
                        collidedEnemyMesh.GetComponent<EnemyHealth>().Damage(weaponDamage);
                        break;
                    case "Enemy":
                        var collidedEnemy = hit.transform.gameObject;
                        collidedEnemy.GetComponent<EnemyHealth>().Damage(weaponDamage);
                        break;
                    case "TutorialEnemy":
                        var tutorialEnemy = hit.transform.gameObject;
                        tutorialEnemy.GetComponent<TutorialEnemy>().Die();
                        break;
                    case null when tutorialController:
                        tutorialController.ActuallyAim();
                        break;
                    default:
                        if (!tutorialController) break;
                        tutorialController.ActuallyAim();
                        break;
                }
            }
            else if (tutorialController)
                tutorialController.ActuallyAim();
            Instantiate(pistolBulletCasing, bulletCasingSpawnPos.position, transform.rotation);
            base.Fire();
        }
        
    }
    
}
