using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PeasantPitchforkEnemy : EnemyBase {
    private float dashTime;
    private float dashSpeed;
    private SpriteRenderer sprite;
    private Color initialCol;

    // Override enemy ID to load from config
    public override int GetEnemyID() {
        return 1;
    }

    protected override void Start() {
        base.Start();
        dashTime = 0.25f;
        dashSpeed = 2 * attributes.targetDistance / dashTime;
        sprite = GetComponentInChildren<SpriteRenderer>();
        initialCol = sprite.color;
    }

    // Override attack function
    public override IEnumerator EnemyAttack() {
        // While attacking
        while (state == EnemyState.Attacking) {
            float t = 0;
            for (var i = 0; i < 50; ++i) {
                t += 0.02f;
                sprite.color = Color.Lerp(initialCol, Color.red, t);
                yield return new WaitForSeconds(0.02f);
            }

            var direction = (IsPlayer.instance.transform.position - transform.position).normalized;
            direction.y = 0;
            rb.velocity = dashSpeed * direction;
            yield return new WaitForSeconds(dashTime);
            rb.velocity = Vector3.zero;
            sprite.color = initialCol;

            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        // Let update know that we're done
        runningCoroutine = false;
    }
}