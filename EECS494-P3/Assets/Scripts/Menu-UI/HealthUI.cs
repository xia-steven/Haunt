using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] GameObject healthPipPrefab;

    [SerializeField] Sprite fullHeartImage;
    [SerializeField] Sprite halfHeartImage;

    [SerializeField] float loadWaitTime = 0.2f;

    Color fullPipColor = new Color(212f / 255f, 75f / 255f, 116f / 255f, 1f);
    Color emptyPipColor = new Color(41f / 255f, 41f / 255f, 41f / 255f, 1f);

    List<Image> healthPips = new List<Image>();

    enum HeartValue
    {
        empty,
        half,
        full
    }

    List<HeartValue> heartValueTracker = new List<HeartValue>();

    Subscription<DamageEvent> damage_event_subscription;
    Subscription<HealEvent> heal_event_subscription;
    // Start is called before the first frame update
    void Start()
    {
        damage_event_subscription = EventBus.Subscribe<DamageEvent>(_OnDamage);
        heal_event_subscription = EventBus.Subscribe<HealEvent>(_OnHeal);

        int numPips = (IsPlayer.instance.GetMaxHealth() + 1) / 2;

        for (int i = 0; i < numPips; ++i)
        {
            GameObject newPip = GameObject.Instantiate(healthPipPrefab, transform.localPosition, Quaternion.identity);
            newPip.transform.localScale = Vector3.one;
            newPip.transform.SetParent(transform, false);
            healthPips.Add(newPip.GetComponentsInChildren<Image>()[1]);
            healthPips[i].enabled = false;
            heartValueTracker.Add(HeartValue.empty);
        }

        StartCoroutine(loadHealth());

        
    }

    private IEnumerator loadHealth()
    {
        int maxHealth = IsPlayer.instance.GetMaxHealth();
        for (int i = 0; i < healthPips.Count; ++i)
        {
            yield return new WaitForSeconds(loadWaitTime);
            healthPips[i].enabled = true;
            healthPips[i].sprite = halfHeartImage;
            heartValueTracker[i] = HeartValue.half;
            if (maxHealth/2 > i)
            {
                yield return new WaitForSeconds(loadWaitTime);
                healthPips[i].sprite = fullHeartImage;
                heartValueTracker[i] = HeartValue.full;
            }
        }
    }

    
    void _OnDamage(DamageEvent e)
    {
        int newHealth = IsPlayer.instance.GetHealth();
        UpdatePips(newHealth);

        if (newHealth == 0)
        {
            EventBus.Publish(new GameLossEvent());
        }
    }
    

    void _OnHeal(HealEvent e)
    {
        int newHealth = IsPlayer.instance.GetHealth();
        UpdatePips(newHealth);
    }

    private void UpdatePips(int newHealth)
    {
        int changeIdx = (newHealth - 1) / 2;
        for(int i = 0; i < changeIdx; ++i)
        {
            if(heartValueTracker[i] != HeartValue.full)
            {
                healthPips[i].enabled = true;
                healthPips[i].sprite = fullHeartImage;
                heartValueTracker[i] = HeartValue.full;
            }
        }

        if (newHealth % 2 == 1)
        {
            healthPips[changeIdx].enabled = true;
            healthPips[changeIdx].sprite = halfHeartImage;
            heartValueTracker[changeIdx] = HeartValue.half;
        }
        else
        {
            healthPips[changeIdx].enabled = true;
            healthPips[changeIdx].sprite = fullHeartImage;
            heartValueTracker[changeIdx] = HeartValue.full;
        }

        for (int i = changeIdx + 1; i < healthPips.Count; ++i)
        {
            healthPips[i].enabled = false;
            heartValueTracker[i] = HeartValue.empty;
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(damage_event_subscription);
        EventBus.Unsubscribe(heal_event_subscription);
    }
}
