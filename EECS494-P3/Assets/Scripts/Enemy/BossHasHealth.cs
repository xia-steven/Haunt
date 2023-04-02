using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHasHealth : HasHealth
{
    private SpriteRenderer sr;
    private Color normalColor;
    [SerializeField] GameObject healthBar;
    Image healthBarImage;
    TMP_Text healthText;
    IsBoss boss;

    float lastClericSpawn = 0;
    int clericCount = 1;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "Sprite")
            {
                sr = child.gameObject.GetComponent<SpriteRenderer>();
                normalColor = sr.color;
            }
        }
        healthBarImage = healthBar.GetComponent<Image>();
        healthText = healthBar.GetComponentInChildren<TMP_Text>();
        healthText.text = health.ToString();

        boss = GetComponent<IsBoss>();

        lastClericSpawn = maxHealth;
    }


    public override void AlterHealth(int healthDelta)
    {
        base.AlterHealth(healthDelta);
        StartCoroutine(FlashRed());
        if(health <= 0)
        {
            Destroy(this.gameObject);
            // Hide health bar on death
            healthBarImage.gameObject.SetActive(false);

            GameControl.WinGame();
        }
        else
        {
            healthBarImage.fillAmount = health / maxHealth;
            healthText.text = health.ToString();
        }

        if(health / maxHealth < 0.33)
        {
            boss.enabledLaser = true;
        }
        else if (health / maxHealth < 0.66)
        {
            boss.enabledGroundPound = true;
        }

        // If we haven't spawned clerics in 20 health
        if(lastClericSpawn - health > 20)
        {
            boss.SpawnClerics(clericCount);
            ++clericCount;
            lastClericSpawn = health;
        }
    }



    private IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sr.color = normalColor;
    }

    public void setMaxHealth(float health_in)
    {
        maxHealth = health_in;
        health = maxHealth;
        lastClericSpawn = health;
        healthText.text = health.ToString();
    }

}
