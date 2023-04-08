using Player;
using UnityEngine;
using Weapons;

namespace Enemy.Weapons {
    [RequireComponent(typeof(Rigidbody))]
    public class ArbalestProjectile : EnemyBasicBullet {
        private Rigidbody rb;
        private readonly float rotationSpeed = 1.3f;

        private void Start() {
            rb = GetComponent<Rigidbody>();
        }


        private new void Update() {
            base.Update();

            // Slight seeking code
            var playerPosition = IsPlayer.instance.transform.position;

            var targetDirection = (playerPosition - transform.position).normalized;

            var newDirection =
                Vector3.RotateTowards(rb.velocity, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);

            var currSpeed = rb.velocity.magnitude;

            rb.velocity = currSpeed * newDirection.normalized;
        }
    }
}