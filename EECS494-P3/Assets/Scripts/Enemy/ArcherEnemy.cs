using System.Collections;
using UnityEngine;

public class ArcherEnemy : EnemyBase {
    protected GameObject Bullet;

    protected override void Start() {
        base.Start();

        Bullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/ArcherBullet");
    }


    // Override enemy ID to load from config
    public override int GetEnemyID() {
        return 2;
    }

    // Override attack function
    public override IEnumerator EnemyAttack() {
        // While attacking
        while (state == EnemyState.Attacking) {
            var targetPosition = IsPlayer.instance.transform.position;

            var direction = targetPosition - transform.position;
            direction.y = 0;
            direction = direction.normalized;


            fireBullet(Bullet, direction, Shooter.Enemy, attributes.projectileSpeed);

            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        // Let update know that we're done
        runningCoroutine = false;
    }
}