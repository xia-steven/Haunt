using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ArbalestEnemy : EnemyBase
{
    GameObject arbalestBullet;

    protected override void Start()
    {
        base.Start();

        arbalestBullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/ArbalestShot");

        ArbalestProjectile proj = arbalestBullet.GetComponent<ArbalestProjectile>();
        proj.setLifetime(attributes.projetileLifetime);
        proj.rotationSpeed = attributes.arbalestRotationSpeed;
    }

    // Override enemy ID to load from config
    protected override int GetEnemyID()
    {
        return 5;
    }

    // Override attack function
    protected override IEnumerator EnemyAttack()
    {
        // Slight attack delay
        yield return new WaitForSeconds(0.25f);

        // While attacking
        while (state == EnemyState.Attacking)
        {
            Vector3 targetPosition = IsPlayer.instance.transform.position;

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            direction = direction.normalized;


            fireBullet(arbalestBullet, direction, Shooter.Enemy, attributes.projectileSpeed);

            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        // Let update know that we're done
        runningCoroutine = false;
    }
}