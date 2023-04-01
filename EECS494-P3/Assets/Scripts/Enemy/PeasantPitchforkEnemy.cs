using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PeasantPitchforkEnemy : EnemyBase {
    private float dashTime;
    private float dashSpeed;

    // Override enemy ID to load from config
    public override int GetEnemyID() {
        return 1;
    }

    protected override void Start() {
        base.Start();
        dashTime = 0.25f;
        dashSpeed = 2 * attributes.targetDistance / dashTime;
    }

    // Override attack function
    public override IEnumerator EnemyAttack() {
        var direction = (IsPlayer.instance.transform.position - transform.position).normalized;
        direction.y = 0;

        // While attacking
        while (state == EnemyState.Attacking) {
            yield return new WaitForSeconds(1);
            rb.velocity = dashSpeed * direction;
            yield return new WaitForSeconds(dashTime);
            rb.velocity = Vector3.zero;

            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        // Let update know that we're done
        runningCoroutine = false;
    }
}