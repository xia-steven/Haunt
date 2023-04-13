using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsExplosive : MonoBehaviour {
    [SerializeField] float explosiveRadius = 2.0f;
    [SerializeField] bool oneShotEnemies = false;
    [SerializeField] bool fromPlayer = false;
    [SerializeField] float damage = -1f;
    AudioClip explosionSound;

    // Used to prevent spawning objects on quit
    bool quitting = false;

    private void OnDestroy() {
        if (!quitting) {
            explode();
        }
    }

    void explode() {
        Debug.Log("Exploding");
        // Play explosion sound
        explosionSound = Resources.Load<AudioClip>("Audio/Weapons/explosion");
        AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        // Perform hit
        Collider[] hitColliders;
        if (fromPlayer)
        {
            hitColliders = Physics.OverlapSphere(transform.position, explosiveRadius * PlayerModifiers.explosiveRadius);
        } else
        {
            hitColliders = Physics.OverlapSphere(transform.position, explosiveRadius);
        }

        foreach (Collider hit in hitColliders) {
            PlayerHasHealth playerHit;
            HasEnemyHealth enemyHit;
            HasPedestalHealth pedestalHit;

            if (hit.TryGetComponent<PlayerHasHealth>(out playerHit) && !fromPlayer) {
                playerHit.AlterHealth(-1, DeathCauses.Enemy);
            }
            else if (hit.TryGetComponent<HasEnemyHealth>(out enemyHit)) {
                // Kill the enemy
                float damageAmount = oneShotEnemies ? -1000 : damage;
                enemyHit.AlterHealth(damageAmount * PlayerModifiers.damage);
            }
            else if (hit.TryGetComponent<HasPedestalHealth>(out pedestalHit))
            {
                pedestalHit.AlterHealth(1 * PlayerModifiers.damage);
            }
        }

        // Show visual
        GameObject visual = Resources.Load<GameObject>("Prefabs/EnemyWeapons/ExplosionRadius");
        GameObject spawnedVisual = Instantiate(visual, transform.position, Quaternion.identity);

        IsSprite spriteScaler = spawnedVisual.GetComponentInChildren<IsSprite>();

        if (fromPlayer)
        {
            spriteScaler.scale = explosiveRadius * 1.75f * PlayerModifiers.explosiveRadius;
        } else
        {
            spriteScaler.scale = explosiveRadius * 1.75f;
        }

        Destroy(spawnedVisual, 0.7f);
    }

    public void setExplosiveRadius(float newRadius)
    {
        explosiveRadius = newRadius;
    }

    private void OnApplicationQuit() {
        quitting = true;
    }
}