using System.Collections;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player {
    public class PlayerHasHealth : HasHealth {
        private Subscription<PedestalDestroyedEvent> pedDestSub;
        private Subscription<PedestalRepairedEvent> pedRepSub;
        private Subscription<MessageFinishedEvent> messFinSub;

        public int id;

        [SerializeField] private float invincibilityTimer = 1f;
        [SerializeField] private int tutorialDeathMessageID = 6;

        private new const int maxHealth = 6;
        private int lockedHealth;
        private int shieldHealth;
        private bool isInvincible;


        // Start is called before the first frame update
        private void Start() {
            pedDestSub = EventBus.Subscribe<PedestalDestroyedEvent>(_OnPedestalDied);
            pedRepSub = EventBus.Subscribe<PedestalRepairedEvent>(_OnPedestalRepaired);
            messFinSub = EventBus.Subscribe<MessageFinishedEvent>(_OnTutorialDeathMessageFinished);

            SceneManager.sceneLoaded += OnSceneLoaded;

            id = Random.Range(0, 1000);
        }

        public override void AlterHealth(int healthDelta) {
            Debug.Log("ALTERHEALTH: " + healthDelta);
            switch (healthDelta) {
                // healing
                case > 0: {
                    health += healthDelta;
                    if (health > maxHealth - lockedHealth)
                        health = maxHealth - lockedHealth;
                    break;
                }
                // damage
                case < 0: {
                    switch (isInvincible) {
                        case false: {
                            switch (shieldHealth) {
                                case > 0:
                                    shieldHealth -= 1;
                                    break;
                                default:
                                    health += healthDelta;
                                    break;
                            }

                            EventBus.Publish(new PlayerDamagedEvent(healthDelta));
                            StartCoroutine(TriggerInvincibility());
                            break;
                        }
                    }

                    switch (health) {
                        // death check
                        case <= 0:
                            health = 0;
                            CheckIsDead();
                            break;
                    }

                    // Invincibility if losing damage
                    StartCoroutine(TriggerInvincibility());
                    break;
                }
            }

            EventBus.Publish(new HealthUIUpdate((int)health, lockedHealth, shieldHealth));
        }

        public void AlterHealth(int healthDelta, DeathCauses damager) {
            IsPlayer.instance.SetLastDamaged(damager);
            AlterHealth(healthDelta);
        }

        private bool CheckIsDead() {
            Debug.Log("Game control day: " + Game_Control.GameControl.Day);
            switch (health) {
                case 0 when Game_Control.GameControl.Day > 0:
                    EventBus.Publish(new GameLossEvent(IsPlayer.instance.LastDamaged()));
                    return true;
                case 0 when Game_Control.GameControl.Day <= 0:
                    // Tutorial day death
                    EventBus.Publish(new TutorialMessageEvent(tutorialDeathMessageID, GetInstanceID(), true));
                    return true;
                default:
                    return false;
            }
        }


        private void _OnTutorialDeathMessageFinished(MessageFinishedEvent mfe) {
            if (mfe.senderInstanceID == GetInstanceID()) {
                // Restart tutorial scene
                health = maxHealth;
                lockedHealth = 0;
                SceneManager.LoadScene("TutorialGameScene");
            }
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

        public void AddShield() {
            shieldHealth += 2;
            EventBus.Publish(new HealthUIUpdate((int)health, lockedHealth, shieldHealth));
        }

        private IEnumerator TriggerInvincibility() {
            isInvincible = true;
            float duration = 0;
            var sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
            var normalColor = sr.color;
            while (duration < invincibilityTimer) {
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

        private void OnSceneLoaded(Scene s, LoadSceneMode m) {
            switch (s.name) {
                case "TutorialGameScene" or "TutorialHubWorld":
                    Debug.Log("TutorialGameScene Loaded");
                    shieldHealth = 0;
                    transform.position = new Vector3(0, 0.5f, 0);
                    break;
                case "GameScene" or "HubWorld":
                    lockedHealth = 0;
                    transform.position = new Vector3(0, 0.5f, 0);
                    break;
            }

            StartCoroutine(DelayUIUpdateOnSceneLoad());
        }

        // waits for the new scene's UI to load before sending the update
        // ensuring correct # of hearts are displayed
        private IEnumerator DelayUIUpdateOnSceneLoad() {
            yield return null;
            EventBus.Publish(new HealthUIUpdate((int)health, lockedHealth, shieldHealth));
        }

        public void ResetHealth() {
            Debug.Log("HERE!!!");
            lockedHealth = 0;
            health = maxHealth;
            shieldHealth = 0;
        }
    }
}