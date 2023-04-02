using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PeasantTorchEnemy : EnemyBase {
    private GameObject torchObject;
    private const float swingTime = 0.5f;
    private const float swingSpeed = 1;

    // Override enemy ID to load from config
    public override int GetEnemyID() {
        return 0;
    }

    protected override void Start() {
        base.Start();
        torchObject = GetComponentInChildren<ThiefKnife>().gameObject;
        torchObject.SetActive(false);
    }

    // Override attack function
    public override IEnumerator EnemyAttack() {
        Debug.Log("Torch enemy starting attack");

        var direction = (IsPlayer.instance.transform.position - transform.position).normalized;

        // Calculate the rotation that points in the direction of the intersection point
        var rotation = Quaternion.LookRotation(direction, Vector3.up);


        // Set the rotation of the knife
        torchObject.transform.rotation = rotation;
        torchObject.transform.Rotate(-90, 0, 0);

        torchObject.SetActive(true);

        // While attacking
        while (state == EnemyState.Attacking) {
            // slight backwards windup
            // rb.velocity = -direction * windupSpeed;
            // yield return new WaitForSeconds(windupTime);
            //
            // // dash towards the player
            // rb.velocity = direction * dashSpeed;
            // yield return new WaitForSeconds(dashTime);

            // teleport away
            rb.velocity = Vector3.zero;
            
            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        torchObject.SetActive(false);

        // Let update know that we're done
        runningCoroutine = false;
    }
}