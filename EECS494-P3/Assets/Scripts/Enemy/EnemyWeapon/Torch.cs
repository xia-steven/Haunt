using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        Debug.Log("Collision detected");
        var collided = other.gameObject;

        // Don't collide with specified items
        if (collided.layer == LayerMask.NameToLayer("Special")) {
            return;
        }

        var playerHealth = collided.GetComponent<PlayerHasHealth>();
        if (playerHealth != null) {
            Debug.Log("Player damaged from torch");
            playerHealth.AlterHealth(-1, DeathCauses.Enemy);
        }
    }
}