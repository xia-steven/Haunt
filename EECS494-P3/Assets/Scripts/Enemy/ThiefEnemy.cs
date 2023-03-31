using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

// TODO: Rename file and class to enemy name
public class ThiefEnemy : EnemyBase {
    ThiefKnife knife;
    GameObject knifeObject;

    protected override void Start()
    {
        base.Start();

        knife = GetComponentInChildren<ThiefKnife>();
        Debug.Log(knife);
        knifeObject = knife.gameObject;
        knifeObject.SetActive(false);
    }

    // Override enemy ID to load from config
    public override int GetEnemyID()
    {
        // TODO: Change returned value to enemyID (index in config file)
        return 7;
    }

    // Override attack function
    public override IEnumerator EnemyAttack()
    {
        // TODO: Remove or change debug statement
        Debug.Log("Thief Enemy starting attack");
        Vector3 direction = (IsPlayer.instance.transform.position - transform.position).normalized;

        knifeObject.SetActive(true);

        // While attacking
        while (state == EnemyState.Attacking)
        {
            // TODO: Perform attack here
            // (initialize bullets/projectiles, strafe, dash at target, etc.)

            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        knifeObject.SetActive(false);

        // Let update know that we're done
        runningCoroutine = false;
    }
}