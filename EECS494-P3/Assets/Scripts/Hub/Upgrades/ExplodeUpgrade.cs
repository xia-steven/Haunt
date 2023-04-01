using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeUpgrade : Upgrade
{
    [SerializeField] bool oneShotEnemies = false;
    private float explosiveRadius;
    private GameObject player;
    private Subscription<PlayerDodgeEvent> dodgeEvent;

    protected override void Awake()
    {
        dodgeEvent = EventBus.Subscribe<PlayerDodgeEvent>(_OnDodge);
        base.Awake();
    }

    protected override void Start()
    {
        thisData = typesData.types[(int)UpgradeType.dashExplode];
        base.Start();
    }

    protected override void Apply()
    {
        player = GameObject.Find("Player");
        explosiveRadius = thisData.rate1;
        base.Apply();
    }

    // Explode on dash finish
    private void _OnDodge(PlayerDodgeEvent e)
    {
        if (!e.start)
        {
            // Perform hit
            Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, explosiveRadius);

            foreach (Collider hit in hitColliders)
            {
                HasEnemyHealth enemyHit;

                if (hit.TryGetComponent<HasEnemyHealth>(out enemyHit))
                {
                    // Kill the enemy
                    int damageAmount = oneShotEnemies ? -1000 : -1;
                    enemyHit.AlterHealth(damageAmount);
                }
            }

            // Show visual
            GameObject visual = Resources.Load<GameObject>("Prefabs/EnemyWeapons/ExplosionRadius");
            GameObject spawnedVisual = Instantiate(visual, player.transform.position, Quaternion.identity);

            IsSprite spriteScaler = spawnedVisual.GetComponentInChildren<IsSprite>();
            spriteScaler.scale = explosiveRadius * 2;

            Destroy(spawnedVisual, 0.15f);
        }
    }

    protected override void OnDestroy()
    {
        EventBus.Unsubscribe(dodgeEvent);
        base.OnDestroy();
    }
}
