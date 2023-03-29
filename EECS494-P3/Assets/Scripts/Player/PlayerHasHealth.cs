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

    private const int maxHealth = 6;
    private int lockedHealth = 0;
    private int shieldHealth = 0;
    private bool isInvincible = false;
    

    // Start is called before the first frame update
    void Start() {
        damageSub = EventBus.Subscribe<PlayerDamagedEvent>(_OnPlayerDamaged);
        pedDestSub = EventBus.Subscribe<PedestalDestroyedEvent>(_OnPedestalDied);
        pedRepSub = EventBus.Subscribe<PedestalRepairedEvent>(_OnPedestalRepaired);
        messFinSub = EventBus.Subscribe<MessageFinishedEvent>(_OnTutorialDeathMessageFinished);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void AlterHealth(int healthDelta)
    {
        health += healthDelta;
        if (healthDelta > 0 && health > maxHealth - lockedHealth)
        {
            health = maxHealth - lockedHealth;
        } else if (healthDelta < 0)
        {
            if (health <= 0)
            {
                health = 0;
                CheckIsDead();
            }
        }
        EventBus.Publish(new HealthUIUpdate(health, lockedHealth, shieldHealth));
    }

    private bool CheckIsDead()
    {
        Debug.Log("Game control day: " + GameControl.Day);
        if (health == 0 && GameControl.Day > 0)
        {
            EventBus.Publish(new GameLossEvent());
            return true;
        }
        else if (health == 0 && GameControl.Day <= 0)
        {
            // Tutorial day death
            EventBus.Publish(new TutorialMessageEvent(tutorialDeathMessageID, GetInstanceID(), KeyCode.Mouse0));
            return true;
        }

        return false;
    }
    void _OnPlayerDamaged(PlayerDamagedEvent pde) {
        if (!isInvincible)
        {
            if (shieldHealth > 0)
            {
                shieldHealth -= 1;
                EventBus.Publish(new HealthUIUpdate(health, lockedHealth, shieldHealth));
            }
            else
            {
                AlterHealth(-pde.damageTaken);
            }
            
            StartCoroutine(TriggerInvincibility());
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
        lockedHealth -= 2;
        Debug.Log("Player received pedestal death, locked: " + lockedHealth);
        EventBus.Publish(new HealthUIUpdate(health, lockedHealth, shieldHealth));
        
    }

    void _OnPedestalRepaired(PedestalRepairedEvent pre) {
        lockedHealth += 2;
        Debug.Log("Player received pedestal repair, locked: " + lockedHealth);

        if (health > maxHealth-lockedHealth) {
            AlterHealth(maxHealth-lockedHealth - health);
        }
        // AlterHealth will also publish an update to the ui--let's see if it's idempotent 
        EventBus.Publish(new HealthUIUpdate(health, lockedHealth, shieldHealth));

    }

    public void AddShield()
    {
        shieldHealth += 2;
        EventBus.Publish(new HealthUIUpdate(health, lockedHealth, shieldHealth));

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
            health = 4;
            lockedHealth = 2;
            shieldHealth = 0;
            EventBus.Publish(new HealthUIUpdate(health, lockedHealth, shieldHealth));
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
        lockedHealth = 0;
        health = maxHealth;
    }
}