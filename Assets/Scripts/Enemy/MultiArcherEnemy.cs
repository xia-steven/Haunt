using System.Collections;
using UnityEngine;

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
            direction.Normalize();
            var rotation1 = Quaternion.AngleAxis(-25, Vector3.up);
            var rotation2 = Quaternion.AngleAxis(-50, Vector3.up);
            var rotation3 = Quaternion.AngleAxis(25, Vector3.up);
            var rotation4 = Quaternion.AngleAxis(50, Vector3.up);

            var bullet1 = rotation1 * direction;
            var bullet2 = rotation2 * direction;
            var bullet3 = rotation3 * direction;
            var bullet4 = rotation4 * direction;

            fireBullet(Bullet, direction, Shooter.Enemy, attributes.projectileSpeed, attributes.projectileLifetime);
            fireBullet(Bullet, bullet1, Shooter.Enemy, attributes.projectileSpeed, attributes.projectileLifetime);
            fireBullet(Bullet, bullet2, Shooter.Enemy, attributes.projectileSpeed, attributes.projectileLifetime);
            fireBullet(Bullet, bullet3, Shooter.Enemy, attributes.projectileSpeed, attributes.projectileLifetime);
            fireBullet(Bullet, bullet4, Shooter.Enemy, attributes.projectileSpeed, attributes.projectileLifetime);

            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        // Let update know that we're done
        runningCoroutine = false;
    }
}