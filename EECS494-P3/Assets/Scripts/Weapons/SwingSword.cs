using Pedestal;
using UnityEngine;

namespace Weapons {
    public class SwingSword : MonoBehaviour {
        private bool rotating;

        // Rotation speed in degrees per second
        private float rotationSpeed = 10f;
        [SerializeField] private int damage = -1;

        public void SetUp(float speed) {
            rotationSpeed = speed;
            rotating = true;
        }

        private void FixedUpdate() {
            switch (rotating) {
                case true:
                    transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                    break;
            }
        }

        private void OnTriggerEnter(Collider other) {
            var collided = other.gameObject;

            // Don't collide with specified items
            if (collided.layer == LayerMask.NameToLayer("Special")) return;

            // Don't collide with player
            if (collided.layer == LayerMask.NameToLayer("Player")) return;

            // Reverse bullets that are hit
            if (collided.layer == LayerMask.NameToLayer("PlayerUtility")) collided.GetComponent<Rigidbody>().velocity = -collided.GetComponent<Rigidbody>().velocity;

            // Alter pedestal health if collided is pedestal and shot by player
            var pedHealth = collided.GetComponent<HasPedestalHealth>();
            if (pedHealth != null) pedHealth.AlterHealth(-damage);

            // Alter health if collided has health
            var health = collided.GetComponent<HasHealth>();
            if (health == null || pedHealth != null) return;
            Debug.Log("Health altered");
            health.AlterHealth(damage);
        }
    }
}