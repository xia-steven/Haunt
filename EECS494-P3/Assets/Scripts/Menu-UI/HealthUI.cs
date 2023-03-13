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

    Subscription<PedestalDestroyedEvent> pedDestSub;
    Subscription<PedestalRepairedEvent> pedRepSub;

    enum HeartValue
    {
        empty,
        half,
        full
    }

    List<HeartValue> heartValueTracker = new List<HeartValue>();

    Subscription<PlayerDamagedEvent> damage_event_subscription;
    Subscription<HealEvent> heal_event_subscription;
    // Start is called before the first frame update
    void Start()
    {
        damage_event_subscription = EventBus.Subscribe<PlayerDamagedEvent>(_OnDamage);
        heal_event_subscription = EventBus.Subscribe<HealEvent>(_OnHeal);
        pedDestSub = EventBus.Subscribe<PedestalDestroyedEvent>(_OnPedestalDied);
        pedRepSub = EventBus.Subscribe<PedestalRepairedEvent>(_OnPedestalRepaired);

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

    
    void _OnDamage(PlayerDamagedEvent e)
    {
        StartCoroutine(waitForHealthUpdate());
    }
    
    IEnumerator waitForHealthUpdate()
    {
        yield return null;

        UpdatePips(IsPlayer.instance.GetHealth());
    }

    void _OnPedestalDied(PedestalDestroyedEvent pde)
    {
        // Destroy health icon
        Destroy(healthPips[healthPips.Count - 1].transform.parent.gameObject);
        // Remove from data structures
        healthPips.RemoveAt(healthPips.Count - 1);
        heartValueTracker.RemoveAt(healthPips.Count - 1);
    }

    void _OnPedestalRepaired(PedestalRepairedEvent pre)
    {
        GameObject newPip = GameObject.Instantiate(healthPipPrefab, transform.localPosition, Quaternion.identity);
        newPip.transform.localScale = Vector3.one;
        newPip.transform.SetParent(transform, false);
        healthPips.Add(newPip.GetComponentsInChildren<Image>()[1]);
        healthPips[healthPips.Count - 1].enabled = false;
        heartValueTracker.Add(HeartValue.empty);
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
        else if (newHealth == 0)
        {
            healthPips[changeIdx].enabled = false;
            heartValueTracker[changeIdx] = HeartValue.empty;
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
        EventBus.Unsubscribe(pedDestSub);
        EventBus.Unsubscribe(pedRepSub);
    }
}
