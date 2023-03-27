using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HasEnemyHealth : HasHealth
{
    private GameObject coinPrefab;
    private GameObject healthPrefab;

    private void Start()
    {
        coinPrefab = Resources.Load<GameObject>("Prefabs/Coin");
        healthPrefab = Resources.Load<GameObject>("Prefabs/Health");

    }

    public override bool AlterHealth(int healthDelta)
    {
        base.AlterHealth(healthDelta);
        if (health <= 0)
        {
            
            if (Random.Range(0, 100) < 20)
            {
                Instantiate(healthPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
        return true;
    }
}
