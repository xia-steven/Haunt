using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasExplodeUpgrade : MonoBehaviour
{
    [SerializeField] bool oneShotEnemies = false;
    public float explosiveRadius;
    private Subscription<PlayerDodgeEvent> dodgeEvent;

    // Start is called before the first frame update
    void Start()
    {
        dodgeEvent = EventBus.Subscribe<PlayerDodgeEvent>(_OnDodge);
    }

    // Explode on dash finish
    private void _OnDodge(PlayerDodgeEvent e)
    {
        if (!e.start)
        {
            // Perform hit
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosiveRadius);

            foreach (Collider hit in hitColliders)
            {
                HasEnemyHealth enemyHit;
                HasPedestalHealth pedestalHit;

                if (hit.TryGetComponent<HasEnemyHealth>(out enemyHit))
                {
                    // Kill the enemy
                    int damageAmount = oneShotEnemies ? -1000 : -1;
                    enemyHit.AlterHealth(damageAmount);
                }
                else if (hit.TryGetComponent<HasPedestalHealth>(out pedestalHit))
                {
                    // Damage pedestal
                    pedestalHit.AlterHealth(-1);
                }
            }

            // Show visual
            GameObject visual = Resources.Load<GameObject>("Prefabs/EnemyWeapons/ExplosionRadius");
            GameObject spawnedVisual = Instantiate(visual, transform.position, Quaternion.identity);

            IsSprite spriteScaler = spawnedVisual.GetComponentInChildren<IsSprite>();
            spriteScaler.scale = explosiveRadius * 2;

            Destroy(spawnedVisual, 0.15f);
        }
    }

    protected void OnDestroy()
    {
        EventBus.Unsubscribe(dodgeEvent);
    }
}
