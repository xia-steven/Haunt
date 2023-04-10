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


    public override void AlterHealth(float healthDelta)
    {
        base.AlterHealth(healthDelta);
        StartCoroutine(FlashRed());
        if(health <= 0)
        {
            health = 0;
            // Hide health bar on death
            healthBarImage.gameObject.SetActive(false);

            StartCoroutine(DeathSequence());


        }
        else
        {
            healthBarImage.fillAmount = health / maxHealth;
            healthText.text = health.ToString();
        }

        // If we haven't spawned clerics in 20 health
        if(lastClericSpawn - health > 20)
        {
            boss.SpawnClerics(clericCount);
            ++clericCount;
            lastClericSpawn = health;
        }
    }



    IEnumerator DeathSequence()
    {
        // Disable player and set invincible
        EventBus.Publish(new DisablePlayerEvent());
        EventBus.Publish(new ToggleInvincibilityEvent(true));


        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / 2.0f;

        Color initialColor = normalColor;

        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / 2.0f;

            sr.color = new Color(initialColor.r, initialColor.g, initialColor.b, initialColor.a * (1 - progress));


            yield return null;
        }


        Destroy(this.gameObject);
        GameControl.WinGame();
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
