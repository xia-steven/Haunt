using UnityEngine;

namespace Weapons {
    public class LauncherBullet : Bullet {
        private Rigidbody rb;
        private const float slowdownSpeed = 0.985f;
        private const float slowdownDelay = 0.6f;

        private void Start() {
            rb = GetComponent<Rigidbody>();
        }

        public void setLifetime(float lifetime) {
            bulletLife = lifetime;
        }

        private new void Update() {
            base.Update();

            // Slowdown over time, after a delay
            if (Time.time - firedTime >= slowdownDelay) rb.velocity *= slowdownSpeed;
        }
    }
}