using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HasEnemyHealth : HasHealth
{
    private GameObject coinPrefab;
    private GameObject healthPrefab;
    private SpriteRenderer sr;
    private Color normalColor;

    private void Start()
    {
        coinPrefab = Resources.Load<GameObject>("Prefabs/Coin");
        healthPrefab = Resources.Load<GameObject>("Prefabs/Health");
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "Sprite")
            {
                sr = child.gameObject.GetComponent<SpriteRenderer>();
                normalColor = sr.color;
            }
        }

    }

    public override void AlterHealth(int healthDelta)
    {
        base.AlterHealth(healthDelta);
        StartCoroutine(FlashRed());
        if (health <= 0)
        {
            int roulletteBall = Random.Range(0, 100);
            if (roulletteBall < 18)
            {
                Instantiate(healthPrefab, transform.position, Quaternion.identity);
            } else if (roulletteBall >= 18 && roulletteBall < 55)
            {
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    private IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sr.color = normalColor;

    }
}
