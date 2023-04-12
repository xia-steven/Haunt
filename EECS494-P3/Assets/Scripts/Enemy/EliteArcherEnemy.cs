using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EliteArcherEnemy : ArcherEnemy {
    protected override void Start() {
        base.Start();

        Bullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/EliteArcherBullet");
    }

    protected override int GetEnemyID() {
        return 3;
    }

    // Override attack function
    protected override IEnumerator EnemyAttack() {
        // While attacking
        while (state == EnemyState.Attacking) {
            // TODO: change me
            var targetPosition = IsPlayer.instance.transform.position;

            var direction = targetPosition - transform.position;
            direction.y = 0;

            fireBullet(Bullet, direction.normalized, Shooter.Enemy, attributes.projectileSpeed);

            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        // Let update know that we're done
        runningCoroutine = false;
    }
}