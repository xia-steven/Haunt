using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArbalestProjectile : EnemyBasicBullet {
    Rigidbody rb;
    float rotationSpeed = 1.3f;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }


    private void Update() {
        base.Update();

        // Slight seeking code
        Vector3 playerPosition = IsPlayer.instance.transform.position;

        Vector3 targetDirection = (playerPosition - transform.position).normalized;

        Vector3 newDirection =
            Vector3.RotateTowards(rb.velocity, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);

        float currSpeed = rb.velocity.magnitude;

        rb.velocity = currSpeed * newDirection.normalized;
    }
}