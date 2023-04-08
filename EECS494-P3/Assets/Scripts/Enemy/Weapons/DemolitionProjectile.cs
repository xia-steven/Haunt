using UnityEngine;
using Weapons;

namespace Enemy.Weapons {
    [RequireComponent(typeof(Rigidbody))]
    public class DemolitionProjectile : EnemyBasicBullet {
        private Rigidbody rb;
        private const float slowdownSpeed = 0.98f;
        private const float slowdownDelay = 0.5f;

        private void Start() {
            rb = GetComponent<Rigidbody>();
        }

        public new void setLifetime(float lifetime) {
            bulletLife = lifetime;
        }

        private new void Update() {
            base.Update();

            switch (Time.time - firedTime) {
                // Slowdown over time, after a delay
                case >= slowdownDelay:
                    rb.velocity *= slowdownSpeed;
                    break;
            }
        }
    }
}