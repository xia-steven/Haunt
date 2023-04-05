using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHasHealth : HasHealth {
    Subscription<PedestalDestroyedEvent> pedDestSub;
    Subscription<PedestalRepairedEvent> pedRepSub;
    Subscription<MessageFinishedEvent> messFinSub;

    public int id;

    [SerializeField] private float invincibilityTimer = 1f;
    [SerializeField] int tutorialDeathMessageID = 6;

    private const int maxHealth = 6;
    private int lockedHealth = 0;
    private int shieldHealth = 0;
    private bool isInvincible = false;
    

    // Start is called before the first frame update
    void Start() {
        pedDestSub = EventBus.Subscribe<PedestalDestroyedEvent>(_OnPedestalDied);
        pedRepSub = EventBus.Subscribe<PedestalRepairedEvent>(_OnPedestalRepaired);
        messFinSub = EventBus.Subscribe<MessageFinishedEvent>(_OnTutorialDeathMessageFinished);

        SceneManager.sceneLoaded += OnSceneLoaded;
        
        id = Random.Range(0, 1000);
    }

    public override void AlterHealth(int healthDelta)
    {
        Debug.Log("ALTERHEALTH: " + healthDelta);
        // healing
        if (healthDelta > 0)
        {
            health += healthDelta;
            if (health > maxHealth - lockedHealth)
                health = maxHealth - lockedHealth;

        }
        // damage
        else if (healthDelta < 0)
        {
            if (!isInvincible)
            {
                if (shieldHealth > 0)
                {
                    shieldHealth -= 1;
                }
                else
                {
                    health += healthDelta;
                }
                EventBus.Publish(new PlayerDamagedEvent(healthDelta));
                StartCoroutine(TriggerInvincibility());
            }
            
            // death check
            if (health <= 0)
            {
                health = 0;
                CheckIsDead();
            }

            // Invincibility if losing damage
            StartCoroutine(TriggerInvincibility());
        }

        EventBus.Publish(new HealthUIUpdate((int)health, lockedHealth, shieldHealth));
    }

    public void AlterHealth(int healthDelta, DeathCauses damager)
    {
        IsPlayer.instance.SetLastDamaged(damager);
        AlterHealth(healthDelta);
    }

    private bool CheckIsDead()
    {
        Debug.Log("Game control day: " + GameControl.Day);
        if (health == 0 && GameControl.Day > 0)
        {
            EventBus.Publish(new GameLossEvent(IsPlayer.instance.LastDamaged()));
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



    void _OnTutorialDeathMessageFinished(MessageFinishedEvent mfe)
    {
        if(mfe.senderInstanceID == GetInstanceID())
        {
            // Restart tutorial scene
            health = maxHealth;
            lockedHealth = 0;
            SceneManager.LoadScene("TutorialGameScene");
        }
    }

    void _OnPedestalDied(PedestalDestroyedEvent pde) {
        lockedHealth -= 2;
        Debug.Log("Player received pedestal death, locked: " + lockedHealth);
        EventBus.Publish(new HealthUIUpdate((int)health, lockedHealth, shieldHealth));
        
    }

    void _OnPedestalRepaired(PedestalRepairedEvent pre) {
        lockedHealth += 2;
        Debug.Log("Player received pedestal repair, locked: " + lockedHealth);

        IsPlayer.instance.SetLastDamaged(DeathCauses.Pedestal);

        if (health > maxHealth-lockedHealth) {
            health = maxHealth-lockedHealth;
            CheckIsDead();
        }
        // AlterHealth will also publish an update to the ui--let's see if it's idempotent 
        EventBus.Publish(new HealthUIUpdate((int)health, lockedHealth, shieldHealth));

    }

    public void AddShield()
    {
        shieldHealth += 2;
        EventBus.Publish(new HealthUIUpdate((int)health, lockedHealth, shieldHealth));

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
            shieldHealth = 0;
            transform.position = new Vector3(0, 0.5f, 0);
        }
        else if (s.name == "GameScene" || s.name == "HubWorld")
        {
            lockedHealth = 0;
            transform.position = new Vector3(0, 0.5f, 0);
        }
        StartCoroutine(DelayUIUpdateOnSceneLoad());
    }

    // waits for the new scene's UI to load before sending the update
    // ensuring correct # of hearts are displayed
    IEnumerator DelayUIUpdateOnSceneLoad()
    {
        yield return null;
        EventBus.Publish(new HealthUIUpdate((int)health, lockedHealth, shieldHealth));

    }
    public void ResetHealth()
    {
        Debug.Log("HERE!!!");
        lockedHealth = 0;
        health = maxHealth;
        shieldHealth = 0;
    }
}