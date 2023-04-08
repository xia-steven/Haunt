using System.Collections;
using Enemy.Weapons;
using Player;
using UnityEngine;

namespace Enemy {
    public class MagicArcherEnemy : EnemyBase {
        private GameObject magicArcherBullet;

        protected override void Start() {
            base.Start();

            magicArcherBullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/MagicArcherShot");

            var proj = magicArcherBullet.GetComponent<MagicArcherProjectile>();
            proj.setLifetime(attributes.projetileLifetime);
        }


        // Override enemy ID to load from config
        protected override int GetEnemyID() {
            return 8;
        }

        // Override attack function
        protected override IEnumerator EnemyAttack() {
            // TODO: Remove or change debug statement
            Debug.Log("Magic Archer Enemy starting attack");

            // While attacking
            while (state == EnemyState.Attacking) {
                // TODO: Perform attack here
                // (initialize bullets/projectiles, strafe, dash at target, etc.)

                var targetPosition = IsPlayer.instance.transform.position;

                var direction = targetPosition - transform.position;
                direction.y = 0;
                direction = direction.normalized;


                fireBullet(magicArcherBullet, direction, Shooter.Enemy, attributes.projectileSpeed);

                yield return new WaitForSeconds(attributes.attackSpeed);
            }

            // Let update know that we're done
            runningCoroutine = false;
        }
    }
}