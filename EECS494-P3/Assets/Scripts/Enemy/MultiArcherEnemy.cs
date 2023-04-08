using System.Collections;
using Player;
using UnityEngine;

namespace Enemy {
    public class MultiArcherEnemy : ArcherEnemy {
        protected override void Start() {
            base.Start();
            Bullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/MultiArcherBullet");
        }

        protected override int GetEnemyID() {
            return 4;
        }

        // Override attack function
        protected override IEnumerator EnemyAttack() {
            // While attacking
            while (state == EnemyState.Attacking) {
                var targetPosition = IsPlayer.instance.transform.position;

                var direction = targetPosition - transform.position;
                direction.y = 0;
                var rotation1 = Quaternion.AngleAxis(-90 / Random.Range(2f, 5f), Vector3.up);
                var rotation2 = Quaternion.AngleAxis(90 / Random.Range(2f, 5f), Vector3.up);
                var rotationMid = Quaternion.AngleAxis(Random.Range(-10f, 10f), Vector3.up);

                var bullet1 = rotation1 * direction;
                var bullet2 = rotation2 * direction;
                var bulletMid = rotationMid * direction;

                bullet1.Normalize();
                bullet2.Normalize();
                bulletMid.Normalize();

                fireBullet(Bullet, bulletMid, Shooter.Enemy, attributes.projectileSpeed);
                fireBullet(Bullet, bullet1, Shooter.Enemy, attributes.projectileSpeed);
                fireBullet(Bullet, bullet2, Shooter.Enemy, attributes.projectileSpeed);

                yield return new WaitForSeconds(attributes.attackSpeed);
            }

            // Let update know that we're done
            runningCoroutine = false;
        }
    }
}