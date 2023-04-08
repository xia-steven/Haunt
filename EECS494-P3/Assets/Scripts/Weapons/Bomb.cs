using UnityEngine;

namespace Weapons {
    public class Bomb : MonoBehaviour {
        [SerializeField] private float explosionTime;
        private float droppedTime;

        private void Awake() {
            droppedTime = Time.time;
        }

        private void Update() {
            // Destroy (explode) bomb after time has passed
            var passedTime = Time.time - droppedTime;
            if (passedTime >= explosionTime) Destroy(gameObject);
        }
    }
}