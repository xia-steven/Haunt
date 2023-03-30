using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

// TODO: Rename file and class to enemy name
public class MagicArcherEnemy : EnemyBase {
    float projectileSpeed = 4.0f;
    float projectileLifetime = 3.0f;
    GameObject magicArcherBullet;

    protected override void Start()
    {
        base.Start();

        magicArcherBullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/MagicArcherShot");

        MagicArcherProjectile proj = magicArcherBullet.GetComponent<MagicArcherProjectile>();
        proj.setLifetime(projectileLifetime);
    }


    // Override enemy ID to load from config
    public override int GetEnemyID()
    {
        // TODO: Change returned value to enemyID (index in config file)
        return 8;
    }

    // Override attack function
    public override IEnumerator EnemyAttack()
    {
        // TODO: Remove or change debug statement
        Debug.Log("Magic Archer Enemy starting attack");

        // While attacking
        while(state == EnemyState.Attacking)
        {
            // TODO: Perform attack here
            // (initialize bullets/projectiles, strafe, dash at target, etc.)

            Vector3 targetPosition = IsPlayer.instance.transform.position;

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            direction = direction.normalized;


            fireBullet(magicArcherBullet, direction, Shooter.Enemy, projectileSpeed);

            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        // Let update know that we're done
        runningCoroutine = false;
    }
}