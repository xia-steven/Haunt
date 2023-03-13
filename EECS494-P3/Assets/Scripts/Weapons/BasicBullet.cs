using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Most simple bullet
public class BasicBullet : Bullet {
    public static float bulletSpeed = 10;

    private void Awake() {
        damage = -1;
    }

    private void OnCollisionEnter(Collision collision) {
        GameObject collided = collision.gameObject;
        HasHealth health = collided.GetComponent<HasHealth>();

        if (health != null) {
            health.AlterHealth(damage);
        }

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        GameObject collided = other.gameObject;

        // Only check triggers for pedestals
        if (collided.layer != LayerMask.NameToLayer("Pedestal")) {
            return;
        }

        var health = collided.GetComponent<HasPedestalHealth>();

        if (health != null) {
            health.AlterHealth(-damage);
        }

        Destroy(gameObject);
    }
}