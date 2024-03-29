using System.Collections;
using UnityEngine;

public class HasEnemyHealth : HasHealth {
    private GameObject coinPrefab;
    private GameObject healthPrefab;
    private GameObject deathFlowersPrefab;
    private SpriteRenderer sr;
    private Color normalColor;
    private bool isCleric;

    private void Start() {
        coinPrefab = Resources.Load<GameObject>("Prefabs/Coin");
        healthPrefab = Resources.Load<GameObject>("Prefabs/Health");
        deathFlowersPrefab = Resources.Load<GameObject>("Prefabs/DeathFlowers");
        foreach (Transform child in transform) {
            if (child.gameObject.name == "Sprite") {
                sr = child.gameObject.GetComponent<SpriteRenderer>();
                normalColor = sr.color;
            }
        }
    }

    public override void AlterHealth(float healthDelta) {
        base.AlterHealth(healthDelta);
        StartCoroutine(FlashRed());
        if (health <= 0) {
            // Only drop collectibles if not the tutorial day
            if (isCleric && IsPlayer.instance.GetHealth() < IsPlayer.instance.GetMaxHealth()) {
                Instantiate(healthPrefab, transform.position, Quaternion.identity);
            }
            // Only drop collectibles if not the tutorial day
            else if (Random.Range(0, 100) is >= 40 and < 80 && GameControl.Day != 0 && !isCleric) {
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }
            Instantiate(deathFlowersPrefab, transform.position, Quaternion.identity);
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

    public void setClericStatus(bool cleric) {
        isCleric = cleric;
    }
}