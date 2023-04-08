using Events;
using Pedestal;
using Player;
using UnityEngine;

namespace Enemy.Weapons {
    public class IsExplosive : MonoBehaviour {
        [SerializeField] private float explosiveRadius = 2.0f;
        [SerializeField] private bool oneShotEnemies;
        [SerializeField] private bool fromPlayer;

        // Used to prevent spawning objects on quit
        private bool quitting;

        private void OnDestroy() {
            switch (quitting) {
                case false:
                    explode();
                    break;
            }
        }

        private void explode() {
            Debug.Log("Exploding");


            // Perform hit
            Collider[] hitColliders;
            switch (fromPlayer) {
                case true:
                    Debug.Log("Radius = " + explosiveRadius * PlayerModifiers.explosiveRadius);
                    hitColliders =
                        Physics.OverlapSphere(transform.position, explosiveRadius * PlayerModifiers.explosiveRadius);
                    break;
                default:
                    hitColliders = Physics.OverlapSphere(transform.position, explosiveRadius);
                    break;
            }

            foreach (var hit in hitColliders) {
                PlayerHasHealth playerHit;
                HasEnemyHealth enemyHit;
                HasPedestalHealth pedestalHit;

                if (hit.TryGetComponent(out playerHit) && !fromPlayer) {
                    playerHit.AlterHealth(-1, DeathCauses.Enemy);
                }
                else if (hit.TryGetComponent(out enemyHit)) {
                    // Kill the enemy
                    var damageAmount = oneShotEnemies ? -1000 : -1;
                    enemyHit.AlterHealth(damageAmount);
                }
                else if (hit.TryGetComponent(out pedestalHit)) {
                    pedestalHit.AlterHealth(1);
                }
            }

            // Show visual
            var visual = Resources.Load<GameObject>("Prefabs/EnemyWeapons/ExplosionRadius");
            var spawnedVisual = Instantiate(visual, transform.position, Quaternion.identity);

            var spriteScaler = spawnedVisual.GetComponentInChildren<IsSprite>();

            spriteScaler.scale = fromPlayer switch {
                true => explosiveRadius * 2 * PlayerModifiers.explosiveRadius,
                _ => explosiveRadius * 2
            };

            Destroy(spawnedVisual, 0.15f);
        }

        public void setExplosiveRadius(float newRadius) {
            explosiveRadius = newRadius;
        }

        private void OnApplicationQuit() {
            quitting = true;
        }
    }
}