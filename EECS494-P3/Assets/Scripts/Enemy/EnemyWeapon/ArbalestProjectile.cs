using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArbalestProjectile : EnemyBasicBullet {
    Rigidbody rb;
    // Rotation amount in degrees per second
    public float rotationSpeed = 0f;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        Debug.Log("Arbalest proj destroyed...");
    }


    private new void Update() {
        base.Update();

        // Slight seeking code
        Vector3 playerPosition = IsPlayer.instance.transform.position;

        Vector3 targetDirection = (playerPosition - transform.position).normalized;

        Vector3 newDirection =
            Vector3.RotateTowards(rb.velocity, targetDirection, Mathf.Deg2Rad * rotationSpeed * Time.deltaTime, 0.0f);

        float currSpeed = rb.velocity.magnitude;

        rb.velocity = currSpeed * newDirection.normalized;
        Debug.Log("Arbalest proj velocity: " + rb.velocity);
        Debug.Log("Lifetime: " + bulletLife);
    }
}