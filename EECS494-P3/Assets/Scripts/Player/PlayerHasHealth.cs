using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHasHealth : HasHealth {
    private Subscription<PedestalDestroyedEvent> pedDestSub;
    private Subscription<PedestalRepairedEvent> pedRepSub;
    private Subscription<ToggleInvincibilityEvent> invincibleSub;
    private Subscription<PlayerDodgeEvent> dodgeSub;
    private Subscription<NightEndEvent> nightEndSub;
    private Subscription<EnablePlayerEvent> playerEnabled;
    private Subscription<DisablePlayerEvent> playerDisabled;

    [SerializeField] private float invincibilityTimer = 1f;
    [SerializeField] private int tutorialDeathMessageID = 6;

    private new const int maxHealth = 6;
    private int lockedHealth;
    private int shieldHealth;
    private bool isInvincible;
    private bool isDodgingOrTeleporting;
    private bool immuneFromCutscene;

    private bool isEnabled = true;

    // Start is called before the first frame update
    private void Start() {
        pedDestSub = EventBus.Subscribe<PedestalDestroyedEvent>(_OnPedestalDied);
        pedRepSub = EventBus.Subscribe<PedestalRepairedEvent>(_OnPedestalRepaired);
        dodgeSub = EventBus.Subscribe<PlayerDodgeEvent>(_OnDodge);
        invincibleSub = EventBus.Subscribe<ToggleInvincibilityEvent>(_OnInvincibilityToggle);
        nightEndSub = EventBus.Subscribe<NightEndEvent>(NightEnd);
        playerEnabled = EventBus.Subscribe<EnablePlayerEvent>(_OnPlayerEnable);
        playerDisabled = EventBus.Subscribe<DisablePlayerEvent>(_OnPlayerDisable);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void AlterHealth(float healthDelta) {
        //Debug.Log("ALTERHEALTH: " + healthDelta);
        // healing
        if (healthDelta > 0) {
            health += healthDelta;
            if (health > maxHealth - lockedHealth)
                health = maxHealth - lockedHealth;
            // Play heal sound
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Audio/curemagic/Cure2"), transform.position);
        }
        // damage
        else if (healthDelta < 0) {
            if (!isInvincible && !isDodgingOrTeleporting && !immuneFromCutscene) {
                if (shieldHealth > 0) {
                    shieldHealth -= 1;
                }
                else {
                    health += healthDelta;
                }

                // Player will still have int health changes
                EventBus.Publish(new PlayerDamagedEvent((int)healthDelta));
                // Play damage sound
                AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Audio/Movement/Bones"), transform.position);
                StartCoroutine(TriggerInvincibility());
            }

            // death check
            if (health <= 0) {
                health = 0;
                CheckIsDead();
            }
        }

        EventBus.Publish(new HealthUIUpdate((int)health, lockedHealth, shieldHealth));
    }

    public void AlterHealth(float healthDelta, DeathCauses damager) {
        IsPlayer.instance.SetLastDamaged(damager);
        AlterHealth(healthDelta);
    }

    private bool CheckIsDead() {
        Debug.Log("Game control day: " + GameControl.Day);
        if (health == 0) {
            EventBus.Publish(new GameLossEvent(IsPlayer.instance.LastDamaged()));
            return true;
        }

        return false;
    }

    private void _OnPlayerDisable(DisablePlayerEvent dpe) {
        isEnabled = false;
    }

    private void _OnPlayerEnable(EnablePlayerEvent epe) {
        isEnabled = true;
    }


    private void _OnPedestalDied(PedestalDestroyedEvent pde) {
        lockedHealth -= 2;
        Debug.Log("Player received pedestal death, locked: " + lockedHealth);
        EventBus.Publish(new HealthUIUpdate((int)health, lockedHealth, shieldHealth));
    }

    private void _OnPedestalRepaired(PedestalRepairedEvent pre) {
        lockedHealth += 2;
        Debug.Log("Player received pedestal repair, locked: " + lockedHealth);

        IsPlayer.instance.SetLastDamaged(DeathCauses.Pedestal);

        if (health > maxHealth - lockedHealth) {
            health = maxHealth - lockedHealth;
            CheckIsDead();
        }

        // AlterHealth will also publish an update to the ui--let's see if it's idempotent 
        EventBus.Publish(new HealthUIUpdate((int)health, lockedHealth, shieldHealth));
    }

    private void _OnDodge(PlayerDodgeEvent pde) {
        // Enable and disable invincibility on dodge
        isDodgingOrTeleporting = pde.start;
    }

    private void _OnInvincibilityToggle(ToggleInvincibilityEvent tie) {
        immuneFromCutscene = tie.enable;
    }

    public void AddShield() {
        shieldHealth += 2;
        EventBus.Publish(new HealthUIUpdate((int)health, lockedHealth, shieldHealth));
    }

    private IEnumerator TriggerInvincibility() {
        isInvincible = true;
        float duration = 0;
        SpriteRenderer sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Color normalColor = sr.color;
        while (duration < invincibilityTimer && (isEnabled || health > 0)) {
            duration += 0.1f;
            normalColor.a = 1 - normalColor.a;
            sr.color = normalColor;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        normalColor.a = 1;
        sr.color = normalColor;
        isInvincible = false;
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(pedDestSub);
        EventBus.Unsubscribe(pedRepSub);
        EventBus.Unsubscribe(dodgeSub);
        EventBus.Unsubscribe(invincibleSub);
        EventBus.Unsubscribe(playerEnabled);
        EventBus.Unsubscribe(playerDisabled);

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m) {
        if (s.name is "TutorialGameScene" or "TutorialHubWorld") {
            shieldHealth = 0;
            transform.position = new Vector3(0, 0.5f, 0);
        }
        else if (s.name is "GameScene" or "HubWorld") {
            lockedHealth = 0;
            transform.position = new Vector3(0, 0.5f, 0);
        }

        // Disable invincibility if enabled from the previous scene
        isInvincible = false;
        // Remove dodging or teleporting if enabled from the previous scene
        isDodgingOrTeleporting = false;
        // Remove cutscene invincibility if enabled from the previous scene
        immuneFromCutscene = false;

        StartCoroutine(DelayUIUpdateOnSceneLoad());
        EventBus.Publish(new EnablePlayerEvent());
    }

    // waits for the new scene's UI to load before sending the update
    // ensuring correct # of hearts are displayed
    private IEnumerator DelayUIUpdateOnSceneLoad() {
        yield return null;
        EventBus.Publish(new HealthUIUpdate((int)health, lockedHealth, shieldHealth));
    }

    public void ResetHealth() {
        lockedHealth = 0;
        health = maxHealth;
        shieldHealth = 0;
    }

    private void NightEnd(NightEndEvent e) {
        if (e.valid) {
            StartCoroutine(DamagePlayerOnNightEnd());
        }
    }

    private IEnumerator DamagePlayerOnNightEnd() {
        yield return new WaitForSeconds(5);
        while (SceneManager.GetActiveScene().name is "GameScene" or "TutorialGameScene") {
            AlterHealth(-1);
            yield return new WaitForSeconds(8);
        }

        yield return null;
    }
}