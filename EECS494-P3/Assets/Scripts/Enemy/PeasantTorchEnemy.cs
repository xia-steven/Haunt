using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PeasantTorchEnemy : EnemyBase {
    // Override enemy ID to load from config
    public override int GetEnemyID() {
        return 0;
    }

    // Override attack function
    public override IEnumerator EnemyAttack() {
        Debug.Log("Torch enemy starting attack");

        // While attacking
        while (state == EnemyState.Attacking) {
            // Perform attack here


            yield return null;
        }

        // Let update know that we're done
        runningCoroutine = false;
    }
}