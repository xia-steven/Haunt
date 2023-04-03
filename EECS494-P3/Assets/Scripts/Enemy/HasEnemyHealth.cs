using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HasEnemyHealth : HasHealth {
    private GameObject coinPrefab;
    private GameObject healthPrefab;
    private SpriteRenderer sr;
    private Color normalColor;
    private bool isCleric = false;

    private void Start() {
        coinPrefab = Resources.Load<GameObject>("Prefabs/Coin");
        healthPrefab = Resources.Load<GameObject>("Prefabs/Health");
        foreach (Transform child in transform) {
            if (child.gameObject.name == "Sprite") {
                sr = child.gameObject.GetComponent<SpriteRenderer>();
                normalColor = sr.color;
            }
        }
    }

    public override void AlterHealth(int healthDelta) {
        base.AlterHealth(healthDelta);
        StartCoroutine(FlashRed());
        if (health <= 0) {
            var roulletteBall = Random.Range(0, 100);
            // Only drop collectibles if not the tutorial day
            if (roulletteBall < 40 && GameControl.Day != 0 && isCleric) {
                Instantiate(healthPrefab, transform.position, Quaternion.identity);
            }
            // Only drop collectibles if not the tutorial day
            else if (roulletteBall >= 40 && roulletteBall < 80 && GameControl.Day != 0) {
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called by enemybase to initialize the enemy's max health
    /// </summary>
    /// <param name="newMaxHealth"> New max health for the enemy</param>
    public void setMaxHealth(int newMaxHealth) {
        maxHealth = newMaxHealth;
        health = maxHealth;
    }

    private IEnumerator FlashRed() {
        sr.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sr.color = normalColor;
    }

    public void setClericStatus(bool cleric)
    {
        isCleric = cleric;
    }
}