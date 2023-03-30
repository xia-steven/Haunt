using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(EnemyPistol))]
public class ArbalestEnemy : EnemyBase {
    float projectileSpeed = 7.0f;
    float projectileLifetime = 2.0f;
    EnemyPistol weapon;
    GameObject arbalestBullet;

    protected override void Start()
    {
        base.Start();

        arbalestBullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/ArbalestShot");

        weapon = GetComponent<EnemyPistol>();

        ArbalestProjectile proj = arbalestBullet.GetComponent<ArbalestProjectile>();
        proj.setLifetime(projectileLifetime);
    }

    // Override enemy ID to load from config
    public override int GetEnemyID()
    {
        return 5;
    }

    // Override attack function
    public override IEnumerator EnemyAttack()
    {
        // TODO: Remove or change debug statement
        Debug.Log("Arbalest Enemy starting attack");

        // Slight attack delay
        yield return new WaitForSeconds(0.25f);

        // While attacking
        while(state == EnemyState.Attacking)
        {
            Vector3 targetPosition = IsPlayer.instance.transform.position;

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            direction = direction.normalized;


            weapon.FireProjectile(arbalestBullet, direction, transform, projectileSpeed, Shooter.Enemy);

            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        // Let update know that we're done
        runningCoroutine = false;
    }
}