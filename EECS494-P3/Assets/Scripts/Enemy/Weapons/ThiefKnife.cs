using Events;
using Player;
using UnityEngine;

namespace Enemy.Weapons {
    public class ThiefKnife : MonoBehaviour {
        protected void OnTriggerEnter(Collider other) {
            var collided = other.gameObject;

            // Don't collide with specified items
            if (collided.layer == LayerMask.NameToLayer("Special")) return;

            var playerHealth = collided.GetComponent<PlayerHasHealth>();
            if (playerHealth == null) return;
            Debug.Log("Player damaged from thief knife");
            playerHealth.AlterHealth(-1, DeathCauses.Enemy);
        }
    }
}