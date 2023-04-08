using UnityEngine;

namespace Enemy.Weapons {
    public class MagicArcherMiniBullet : MonoBehaviour {
        private MagicArcherProjectile parent;

        private void Awake() {
            parent = GetComponentInParent<MagicArcherProjectile>();
        }

        private void OnTriggerEnter(Collider other) {
            parent.OnTriggerEnter(other);
        }

        public Rigidbody GetParentRB() {
            return parent.GetComponent<Rigidbody>();
        }

        public void SetLastReverse() {
            parent.lastReverse = Time.time;
        }

        public float GetLastReverse() {
            return parent.lastReverse;
        }

        public void ChangeParentShooter(Shooter newShooter) {
            parent.SetShooter(newShooter);
        }
    }
}