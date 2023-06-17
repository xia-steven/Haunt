using UnityEngine;

public class ThiefKnife : MonoBehaviour {
    protected void OnTriggerEnter(Collider other) {
        GameObject collided = other.gameObject;

        // Don't collide with specified items
        if (collided.layer == LayerMask.NameToLayer("Special")) {
            return;
        }

        PlayerHasHealth playerHealth = collided.GetComponent<PlayerHasHealth>();
        if (playerHealth != null) {
            Debug.Log("Player damaged from thief knife");
            playerHealth.AlterHealth(-1, DeathCauses.Enemy);
        }
    }
}