using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Image pip0;
    [SerializeField] Image pip1;
    [SerializeField] Image pip2;
    [SerializeField] GameObject gameOverUI;
    Color fullPipColor = new Color(212f / 255f, 75f / 255f, 116f / 255f, 1f);
    Color emptyPipColor = new Color(41f / 255f, 41f / 255f, 41f / 255f, 1f);
    List<Image> healthPips = new List<Image>();
    int health_counter = 3;

    Subscription<DamageEvent> damage_event_subscription;
    Subscription<HealEvent> heal_event_subscription;
    // Start is called before the first frame update
    void Start()
    {
        damage_event_subscription = EventBus.Subscribe<DamageEvent>(_OnDamage);
        heal_event_subscription = EventBus.Subscribe<HealEvent>(_OnHeal);

        healthPips.Add(pip0);
        healthPips.Add(pip1);
        healthPips.Add(pip2);
        StartCoroutine(loadHealth());

        
    }

    private IEnumerator loadHealth()
    {
        healthPips[0].color = emptyPipColor;
        healthPips[1].color = emptyPipColor;
        healthPips[2].color = emptyPipColor;

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.3f);
            healthPips[i].color = fullPipColor;
        }
    }

    void _OnDamage(DamageEvent e)
    {
        health_counter--;
        healthPips[health_counter].color = emptyPipColor;
        if (health_counter == 0)
        {
            foreach (Transform child in gameOverUI.transform)
            {
                if (child.name == "Restart")
                {
                    EventSystem.current.SetSelectedGameObject(child.gameObject);
                }
            }
            
            Time.timeScale = 0;
            gameOverUI.SetActive(true);
        }
    }

    void _OnHeal(HealEvent e)
    {
        healthPips[health_counter].color = fullPipColor;
        health_counter++;
    }
    private void OnDestroy()
    {
        EventBus.Unsubscribe(damage_event_subscription);
        EventBus.Unsubscribe(heal_event_subscription);
    }
}
