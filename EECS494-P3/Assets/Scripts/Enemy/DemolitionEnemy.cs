using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DemolitionEnemy : EnemyBase {
    GameObject demolitionBullet;


    protected override void Start()
    {
        base.Start();

        demolitionBullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/DemolitionShot");

        DemolitionProjectile proj = demolitionBullet.GetComponent<DemolitionProjectile>();
        proj.setLifetime(attributes.projetileLifetime);
    }

    // Override enemy ID to load from config
    protected override int GetEnemyID()
    {
        // TODO: Change returned value to enemyID (index in config file)
        return 6;
    }

    // Override attack function
    protected override IEnumerator EnemyAttack()
    {
        // TODO: Remove or change debug statement
        Debug.Log("Demolition Enemy starting attack");

        // While attacking
        while(state == EnemyState.Attacking)
        {
            // TODO: Perform attack here
            // (initialize bullets/projectiles, strafe, dash at target, etc.)
            Vector3 targetPosition = IsPlayer.instance.transform.position;

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            direction = direction.normalized;


            fireBullet(demolitionBullet, direction, Shooter.Enemy, attributes.projectileSpeed);

            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        // Let update know that we're done
        runningCoroutine = false;
    }
}