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
    [SerializeField] GameObject winTrigger;
    [SerializeField] List<GameObject> Pedestals;
    Image healthBarImage;
    IsBoss boss;

    float lastClericSpawn = 0;
    int clericCount = 1;

    private void Awake()
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
        float progress = (Time.time - initial_time) / 1.0f;

        Color initialColor = normalColor;

        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / 1.0f;

            sr.color = new Color(initialColor.r, initialColor.g, initialColor.b, initialColor.a * (1 - progress));


            yield return null;
        }


        Destroy(this.gameObject);

        for(int a = 0; a < IsBoss.spawnedClerics.Count; ++a)
        {
            // Destroy any non killed clerics
            if(IsBoss.spawnedClerics[a] != null)
            {
                Destroy(IsBoss.spawnedClerics[a]);
            }
        }

        // Destroy any repaired pedestals
        for(int b = 0; b < Pedestals.Count; ++b)
        {
            // Get health component
            HasPedestalHealth health = Pedestals[b].GetComponent<HasPedestalHealth>();
            // Destroy pedestal
            health.AlterHealth(100);
        }


        // Reenable player (leave invincible)
        EventBus.Publish(new EnablePlayerEvent());

        // Enable the win trigger
        winTrigger.SetActive(true);
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
    }

}
