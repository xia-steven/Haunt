using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsExplosive : MonoBehaviour
{
    [SerializeField] float explosiveRadius = 2.0f;

    // Used to prevent spawning objects on quit
    bool quitting = false;

    private void OnDestroy()
    {
        if(!quitting)
        {
            explode();
        }
    }

    void explode()
    {
        Debug.Log("Exploding");

        
        // Perform hit
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosiveRadius);

        foreach (Collider hit in hitColliders)
        {
            PlayerHasHealth playerHit;
            HasEnemyHealth enemyHit;

            if(hit.TryGetComponent<PlayerHasHealth>(out playerHit))
            {
                EventBus.Publish(new PlayerDamagedEvent(1));
            }
            else if (hit.TryGetComponent<HasEnemyHealth>(out enemyHit))
            {
                // Kill the enemy
                enemyHit.AlterHealth(-1000);
            }

        }

        // Show visual
        GameObject visual = Resources.Load<GameObject>("Prefabs/EnemyWeapons/ExplosionRadius");
        GameObject spawnedVisual = Instantiate(visual, transform.position, Quaternion.identity);

        IsSprite spriteScaler = spawnedVisual.GetComponentInChildren<IsSprite>();
        spriteScaler.scale = explosiveRadius * 2;

        Destroy(spawnedVisual, 0.15f);
    }

    private void OnApplicationQuit()
    {
        quitting = true;
    }
}
