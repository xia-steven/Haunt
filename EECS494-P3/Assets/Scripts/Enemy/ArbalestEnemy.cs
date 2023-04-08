using System.Collections;
using Enemy.Weapons;
using Player;
using UnityEngine;

namespace Enemy {
    public class ArbalestEnemy : EnemyBase {
        private GameObject arbalestBullet;

        protected override void Start() {
            base.Start();

            arbalestBullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/ArbalestShot");

            var proj = arbalestBullet.GetComponent<ArbalestProjectile>();
            proj.setLifetime(attributes.projetileLifetime);
        }

        // Override enemy ID to load from config
        protected override int GetEnemyID() {
            return 5;
        }

        // Override attack function
        protected override IEnumerator EnemyAttack() {
            // TODO: Remove or change debug statement
            Debug.Log("Arbalest Enemy starting attack");

            // Slight attack delay
            yield return new WaitForSeconds(0.25f);

            // While attacking
            while (state == EnemyState.Attacking) {
                var targetPosition = IsPlayer.instance.transform.position;

                var direction = targetPosition - transform.position;
                direction.y = 0;
                direction = direction.normalized;


                fireBullet(arbalestBullet, direction, Shooter.Enemy, attributes.projectileSpeed);

                yield return new WaitForSeconds(attributes.attackSpeed);
            }

            // Let update know that we're done
            runningCoroutine = false;
        }
    }
}