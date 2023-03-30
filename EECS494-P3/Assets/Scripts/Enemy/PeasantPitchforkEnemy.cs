using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PeasantPitchforkEnemy : EnemyBase {

    // Override enemy ID to load from config
    public override int GetEnemyID()
    {
        return 1;
    }

    // Override attack function
    public override IEnumerator EnemyAttack()
    {
        Debug.Log("Pitchfork enemy starting attack");

        // While attacking
        while(state == EnemyState.Attacking)
        {
            // TODO: Perform attack here

            yield return null;
        }

        // Let update know that we're done
        runningCoroutine = false;
    }
}