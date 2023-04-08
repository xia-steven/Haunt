using Events;
using Pedestal;
using Player;
using UnityEngine;

// Base class for all bullet types
namespace Weapons {
    public abstract class Bullet : MonoBehaviour {
        protected int damage = -1;
        private Shooter shooter;
        protected float firedTime;
        private int pierced;

        [SerializeField] protected float bulletLife = 1.0f;

        protected virtual void Awake() {
            firedTime = Time.time;
        }

        public void SetShooter(Shooter entity) {
            shooter = entity;
        }

        public Shooter GetShooter() {
            return shooter;
        }

        protected void OnTriggerEnter(Collider other) {
            var collided = other.gameObject;

            // Don't collide with specified items
            if (collided.layer == LayerMask.NameToLayer("Special")) return;

            // Make sure player can't shoot themselves
            if (collided.layer == LayerMask.NameToLayer("Player") && shooter == Shooter.Player) return;

            // Make sure ememy can't shoot themselves or other enemies
            if (collided.layer == LayerMask.NameToLayer("Enemy") && shooter == Shooter.Enemy) return;

            // Alter pedestal health if collided is pedestal and shot by player
            var pedHealth = collided.GetComponent<HasPedestalHealth>();
            if (pedHealth != null && shooter == Shooter.Player) pedHealth.AlterHealth(-damage);

            var playerHealth = collided.GetComponent<PlayerHasHealth>();
            if (playerHealth != null) {
                Debug.Log("Player damaged");
                playerHealth.AlterHealth(damage, DeathCauses.Enemy);
            }

            // Alter health if collided has health
            var health = collided.GetComponent<HasHealth>();
            if (health != null && pedHealth == null && playerHealth == null) {
                Debug.Log("Health altered");
                health.AlterHealth(damage);
                pierced++;
            }

            // Don't destroy upon melee collision
            if (collided.layer == LayerMask.NameToLayer("Melee")) return;

            // If collided with enemy (from player shooter), check piercing amount otherwise destroy outright if not hitting enemy
            if (collided.layer == LayerMask.NameToLayer("Enemy") && shooter == Shooter.Player &&
                pierced >= PlayerModifiers.maxPierce)
                Destroy(gameObject);
            else if (collided.layer != LayerMask.NameToLayer("Enemy")) Destroy(gameObject);
        }

        protected void Update() {
            // Destroy bullet after a certain amount of time
            var passedTime = Time.time - firedTime;
            if (passedTime >= bulletLife) Destroy(gameObject);
        }
    }
}