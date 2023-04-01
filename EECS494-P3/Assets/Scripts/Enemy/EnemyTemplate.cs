using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

// TODO: Rename file and class to enemy name
public class EnemyTemplate : EnemyBase {
    // Override enemy ID to load from config
    public override int GetEnemyID() {
        // TODO: Change returned value to enemyID (index in config file)
        return 0;
    }

    // Override attack function
    public override IEnumerator EnemyAttack() {
        // TODO: Remove or change debug statement
        Debug.Log("Template Enemy starting attack");

        // While attacking
        while (state == EnemyState.Attacking) {
            // TODO: Perform attack here
            // (initialize bullets/projectiles, strafe, dash at target, etc.)


            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        // Let update know that we're done
        runningCoroutine = false;
    }
}