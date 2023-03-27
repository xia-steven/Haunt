using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthUI : MonoBehaviour {
    [SerializeField] GameObject healthPipPrefab;

    [SerializeField] Sprite fullHeartImage;
    [SerializeField] Sprite halfHeartImage;
    [SerializeField] int tutorialDeathMessageID = 6;

    [SerializeField] float loadWaitTime = 0.2f;

    Color fullPipColor = new Color(212f / 255f, 75f / 255f, 116f / 255f, 1f);
    Color emptyPipColor = new Color(41f / 255f, 41f / 255f, 41f / 255f, 1f);

    List<Image> healthPips = new List<Image>();

    Subscription<PedestalDestroyedEvent> pedDestSub;
    Subscription<PedestalRepairedEvent> pedRepSub;
    Subscription<MessageFinishedEvent> messFinSub;

    enum HeartValue {
        empty,
        half,
        full
    }

    List<HeartValue> heartValueTracker = new List<HeartValue>();

    Subscription<PlayerDamagedEvent> damage_event_subscription;

    Subscription<HealEvent> heal_event_subscription;
    private Subscription<IncreaseMaxHealthEvent> inc_max_health_event_subscription;

    // Start is called before the first frame update
    void Start() {
        damage_event_subscription = EventBus.Subscribe<PlayerDamagedEvent>(_OnDamage);
        heal_event_subscription = EventBus.Subscribe<HealEvent>(_OnHeal);
        inc_max_health_event_subscription = EventBus.Subscribe<IncreaseMaxHealthEvent>(_OnMaxHealthIncrease);

        pedDestSub = EventBus.Subscribe<PedestalDestroyedEvent>(_OnPedestalDied);
        pedRepSub = EventBus.Subscribe<PedestalRepairedEvent>(_OnPedestalRepaired);
        messFinSub = EventBus.Subscribe<MessageFinishedEvent>(_OnTutorialDeathMessageFinished);

        SceneManager.sceneLoaded += OnSceneLoaded;

        
        InitializeHealth();
        StartCoroutine(loadHealth());
    }

    private IEnumerator loadHealth() {
        int maxHealth = IsPlayer.instance.GetMaxHealth();
        for (int i = 0; i < healthPips.Count; ++i) {
            yield return new WaitForSeconds(loadWaitTime);
            healthPips[i].enabled = true;
            healthPips[i].sprite = halfHeartImage;
            heartValueTracker[i] = HeartValue.half;
            if (maxHealth / 2 > i) {
                yield return new WaitForSeconds(loadWaitTime);
                healthPips[i].sprite = fullHeartImage;
                heartValueTracker[i] = HeartValue.full;
            }
        }
    }

    void InitializeHealth()
    {
        for (int i = healthPips.Count-1; i >= 0 ; --i)
        {
            Destroy(healthPips[i]);
            healthPips.RemoveAt(i);
            heartValueTracker.RemoveAt(i);
        }

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
    }

    void _OnDamage(PlayerDamagedEvent e) {
        StartCoroutine(waitForHealthUpdate());
    }

    IEnumerator waitForHealthUpdate()
    {
        yield return null;

        UpdatePips(IsPlayer.instance.GetHealth());
    }

    void _OnPedestalDied(PedestalDestroyedEvent pde)
    {
        GameObject newPip = Instantiate(healthPipPrefab, transform.localPosition, Quaternion.identity);
        newPip.transform.localScale = Vector3.one;
        newPip.transform.SetParent(transform, false);
        healthPips.Add(newPip.GetComponentsInChildren<Image>()[1]);
        healthPips[healthPips.Count - 1].enabled = false;
        heartValueTracker.Add(HeartValue.empty);
    }

    void _OnPedestalRepaired(PedestalRepairedEvent pre) {

        // Destroy health icon
        Destroy(healthPips[healthPips.Count - 1].transform.parent.gameObject);
        // Remove from data structures
        healthPips.RemoveAt(healthPips.Count - 1);
        heartValueTracker.RemoveAt(heartValueTracker.Count - 1);

        // Standard death
        if (healthPips.Count == 0 && GameControl.Day > 0)
        {
            EventBus.Publish(new GameLossEvent());
        }
        else if (healthPips.Count == 0 && GameControl.Day <= 0)
        {
            // Tutorial day death
            EventBus.Publish(new TutorialMessageEvent(tutorialDeathMessageID, GetInstanceID(), KeyCode.Mouse0));
        }
    }

    void _OnTutorialDeathMessageFinished(MessageFinishedEvent mfe)
    {
        if (mfe.senderInstanceID == GetInstanceID())
        {
            // Restart tutorial scene
            SceneManager.LoadScene("TutorialGameScene");
        }
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if(s.name == "TutorialHubWorld" || s.name == "TutorialGameScene")
        {
            InitializeHealth();
        }

    }

    void _OnHeal(HealEvent e) {
        var newHealth = IsPlayer.instance.GetHealth();
        UpdatePips(newHealth);
    }

    // combines the effects of a pedestal being destroyed with a heal event
    void _OnMaxHealthIncrease(IncreaseMaxHealthEvent e)
    {
        GameObject newPip = Instantiate(healthPipPrefab, transform.localPosition, Quaternion.identity);
        newPip.transform.localScale = Vector3.one;
        newPip.transform.SetParent(transform, false);
        healthPips.Add(newPip.GetComponentsInChildren<Image>()[1]);
        healthPips[healthPips.Count - 1].enabled = false;
        heartValueTracker.Add(HeartValue.empty);
        
        var newHealth = IsPlayer.instance.GetHealth();
        UpdatePips(newHealth);
    }

    private void UpdatePips(int newHealth) {
        Debug.Log("Health: " + newHealth);
        
        var changeIdx = (newHealth - 1) / 2;
        Debug.Log("UI Idx: " + changeIdx);
        for (var i = 0; i < changeIdx; ++i) {
            if (heartValueTracker[i] != HeartValue.full) {
                healthPips[i].enabled = true;
                healthPips[i].sprite = fullHeartImage;
                heartValueTracker[i] = HeartValue.full;
            }
        }

        if (newHealth % 2 == 1) {
            healthPips[changeIdx].enabled = true;
            healthPips[changeIdx].sprite = halfHeartImage;
            heartValueTracker[changeIdx] = HeartValue.half;
        }
        else if (newHealth == 0) {
            healthPips[changeIdx].enabled = false;
            heartValueTracker[changeIdx] = HeartValue.empty;
        }
        else {
            healthPips[changeIdx].enabled = true;
            healthPips[changeIdx].sprite = fullHeartImage;
            heartValueTracker[changeIdx] = HeartValue.full;
        }

        for (var i = changeIdx + 1; i < healthPips.Count; ++i) {
            healthPips[i].enabled = false;
            heartValueTracker[i] = HeartValue.empty;
        }
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(damage_event_subscription);
        EventBus.Unsubscribe(heal_event_subscription);
        EventBus.Unsubscribe(inc_max_health_event_subscription);
        EventBus.Unsubscribe(pedDestSub);
        EventBus.Unsubscribe(pedRepSub);
        EventBus.Unsubscribe(messFinSub);

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}