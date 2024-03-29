using System.Collections;
using UnityEngine;

public class PeasantPitchforkEnemy : EnemyBase {
    [SerializeField] private GameObject pitchfork;
    
    private SpriteRenderer sprite;
    private Color initialCol;

    // Override enemy ID to load from config
    protected override int GetEnemyID() {
        return 1;
    }

    protected override void Start() {
        base.Start();
        sprite = GetComponentInChildren<SpriteRenderer>();
        initialCol = sprite.color;
    }

    // Override attack function
    protected override IEnumerator EnemyAttack() {
        // While attacking
        while (state == EnemyState.Attacking)
        {
            pitchfork.SetActive(true);
            float t = 0;
            var direction = (IsPlayer.instance.transform.position - transform.position).normalized;
            direction.y = 0;
            // Calculate the rotation that points in the direction of the intersection point
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Set the rotation of the pitchfork
            pitchfork.transform.rotation = rotation;
            pitchfork.transform.Rotate(-90, 0, 0);

            for (var i = 0; i < 50; ++i) {
                t += 0.02f;
                sprite.color = Color.Lerp(initialCol, Color.red, t);
                yield return new WaitForSeconds(0.02f);
            }

            direction = (IsPlayer.instance.transform.position - transform.position).normalized;
            direction.y = 0;

            // Calculate the rotation that points in the direction of the intersection point
            rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Set the rotation of the pitchfork (again)
            pitchfork.transform.rotation = rotation;
            pitchfork.transform.Rotate(-90, 0, 0);

            rb.velocity = attributes.dashSpeed * (attributes.targetDistance / attributes.dashTime ) * direction;
            yield return new WaitForSeconds(attributes.dashTime);
            rb.velocity = Vector3.zero;
            sprite.color = initialCol;

            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        // Let update know that we're done
        pitchfork.SetActive(false);
        runningCoroutine = false;
    }
}