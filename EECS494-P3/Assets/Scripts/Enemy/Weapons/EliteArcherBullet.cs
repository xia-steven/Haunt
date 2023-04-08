using Player;
using UnityEngine;

namespace Enemy.Weapons {
    public class EliteArcherBullet : ArcherBullet {
        private Rigidbody rb;

        private new void Awake() {
            base.Awake();
            rb = GetComponent<Rigidbody>();
            bulletLife = 1.5f;
        }

        private void FixedUpdate() {
            var bulletDirection = rb.velocity;
            var playerDirection = IsPlayer.instance.transform.position - transform.position;
            bulletDirection.y = 0;
            playerDirection.y = 0;
            rb.AddForce(10 * (playerDirection - Vector3.Dot(playerDirection, bulletDirection.normalized) *
                bulletDirection.normalized));
        }
    }
}