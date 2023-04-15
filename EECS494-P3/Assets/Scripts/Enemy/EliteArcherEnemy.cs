using System;
using System.Collections;
using UnityEngine;

public class EliteArcherEnemy : ArcherEnemy {
    private Vector3 previousPlayerPosition;
    private Vector3 playerVelocity;

    protected override void Start() {
        base.Start();
        playerVelocity = Vector3.zero;
        previousPlayerPosition = IsPlayer.instance.transform.position;
        Bullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/EliteArcherBullet");
        InvokeRepeating(nameof(Refresh), 0, 0.1f);
    }

    protected override int GetEnemyID() {
        return 3;
    }

    private void Refresh() {
        playerVelocity = (IsPlayer.instance.transform.position - previousPlayerPosition) / 0.1f;
        previousPlayerPosition = IsPlayer.instance.transform.position;
    }

    // Override attack function
    protected override IEnumerator EnemyAttack() {
        // While attacking
        while (state == EnemyState.Attacking) {
            var targetPosition = IsPlayer.instance.transform.position + playerVelocity;

            var direction = targetPosition - transform.position;
            direction.y = 0;

            fireBullet(Bullet, direction.normalized, Shooter.Enemy, attributes.projectileSpeed, attributes.projectileLifetime);

            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        // Let update know that we're done
        runningCoroutine = false;
    }
}