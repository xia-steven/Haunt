using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DemolitionProjectile : EnemyBasicBullet {
    Rigidbody rb;
    float slowdownSpeed = 0.98f;
    float slowdownDelay = 0.5f;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    public void setLifetime(float lifetime) {
        bulletLife = lifetime;
    }

    private new void Update() {
        base.Update();

        // Slowdown over time, after a delay
        if (Time.time - firedTime >= slowdownDelay) {
            rb.velocity *= slowdownSpeed;
        }
    }
}