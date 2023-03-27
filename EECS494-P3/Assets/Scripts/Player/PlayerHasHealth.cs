using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHasHealth : HasHealth {
    Subscription<PlayerDamagedEvent> damageSub;
    Subscription<PedestalDestroyedEvent> pedDestSub;
    Subscription<PedestalRepairedEvent> pedRepSub;
    Subscription<MessageFinishedEvent> messFinSub;

    [SerializeField] private float invincibilityTimer = 1f;
    [SerializeField] int tutorialDeathMessageID = 6;

    public int initialMaxHealth = 0;
    private int trueMaxHealth;
    private bool isInvincible = false;

    // Start is called before the first frame update
    void Start() {
        damageSub = EventBus.Subscribe<PlayerDamagedEvent>(_OnPlayerDamaged);
        pedDestSub = EventBus.Subscribe<PedestalDestroyedEvent>(_OnPedestalDied);
        pedRepSub = EventBus.Subscribe<PedestalRepairedEvent>(_OnPedestalRepaired);
        messFinSub = EventBus.Subscribe<MessageFinishedEvent>(_OnTutorialDeathMessageFinished);

        SceneManager.sceneLoaded += OnSceneLoaded;
        initialMaxHealth = maxHealth;
        trueMaxHealth = maxHealth;
    }


    void _OnPlayerDamaged(PlayerDamagedEvent pde) {
        if (!isInvincible)
        {
            
            AlterHealth(-pde.damageTaken);
            StartCoroutine(TriggerInvincibility());
        }
        

        Debug.Log("Game control day: " + GameControl.Day);
        if (health == 0 && GameControl.Day > 0) {
            EventBus.Publish(new GameLossEvent());
        }
        else if (health == 0 && GameControl.Day <= 0)
        {
            // Tutorial day death
            EventBus.Publish(new TutorialMessageEvent(tutorialDeathMessageID, GetInstanceID(), KeyCode.Mouse0));
        }
    }

    void _OnTutorialDeathMessageFinished(MessageFinishedEvent mfe)
    {
        if(mfe.senderInstanceID == GetInstanceID())
        {
            // Restart tutorial scene
            SceneManager.LoadScene("TutorialGameScene");
        }
    }

    void _OnPedestalDied(PedestalDestroyedEvent pde) {
        maxHealth += 2;
        
    }

    void _OnPedestalRepaired(PedestalRepairedEvent pre) {
        maxHealth -= 2;
        if (health > maxHealth) {
            AlterHealth(maxHealth - health);
        }
    }

    public void UpgradeHealth()
    {
        maxHealth += 2;
        trueMaxHealth += 2;
        health = maxHealth;
    }

    public void ResetHealthToInitial()
    {
        maxHealth = initialMaxHealth;
        trueMaxHealth = initialMaxHealth;
        health = initialMaxHealth;
    }
    
    private IEnumerator TriggerInvincibility()
    {
        isInvincible = true;
        float duration = 0;
        SpriteRenderer sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Color normalColor = sr.color;
        while (duration < invincibilityTimer)
        {
            //Debug.Log("Inv_timer:" + duration);
            duration += 0.1f;
            normalColor.a = 1 - normalColor.a;
            sr.color = normalColor;
            yield return new WaitForSeconds(0.1f);
        }

        normalColor.a = 1;
        sr.color = normalColor;
        isInvincible = false;

    }
    private void OnDestroy() {
        EventBus.Unsubscribe(pedDestSub);
        EventBus.Unsubscribe(pedRepSub);
        EventBus.Unsubscribe(messFinSub);

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if (s.name == "TutorialGameScene" || s.name == "TutorialHubWorld")
        {
            Debug.Log("TutorialGameScene Loaded");
            health = initialMaxHealth;
            maxHealth = initialMaxHealth;
            transform.position = new Vector3(0, 0.5f, 0);
        }
        else if (s.name == "GameScene")
        {
            Debug.Log("GameScene Loaded");
            transform.position = new Vector3(0, 0.5f, 0);
        }
    }

    public void ResetHealth()
    {
        maxHealth = trueMaxHealth;
    }
}