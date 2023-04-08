using Events;
using UnityEngine;

namespace Pedestal {
    [RequireComponent(typeof(IsPedestal))]
    public class HasPedestalHealth : HasHealth {
        private const int PedestalMaxHealth = 5;
        [SerializeField] private AudioClip damageSound;
        [SerializeField] private AudioClip breakSound;
        [SerializeField] private AudioClip healSound;
        [SerializeField] private AudioClip restoreSound;


        private Animator anim;
        private SpriteRenderer sr;

        private IsPedestal pedestal;
        private static readonly int TookDamage = Animator.StringToHash("TookDamage");
        private static readonly int Health = Animator.StringToHash("Health");

        private void Start() {
            pedestal = GetComponent<IsPedestal>();
            anim = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
            health = 0;
        }


        public override void AlterHealth(int healthDelta) {
            // NOTE: health delta is treated backwards of standard health components
            // due to the player destroying pedestals and enemies
            healthDelta = -healthDelta;
            Debug.Log("start" + health / PedestalMaxHealth);
            if ((health == 0 && healthDelta < 0) || (health == PedestalMaxHealth && healthDelta > 0) ||
                healthDelta == 0) {
                switch (healthDelta) {
                    case < 0:
                        anim.SetTrigger(TookDamage);
                        break;
                }

                anim.SetFloat(Health, health / PedestalMaxHealth);
                return;
            }

            health = Mathf.Clamp(health + healthDelta, 0, PedestalMaxHealth);

            switch (health) {
                case <= 0 when !pedestal.IsDestroyedByPlayer():
                    pedestal.PedestalDied();
                    AudioSource.PlayClipAtPoint(breakSound, transform.position);
                    break;
                default: {
                    if (health == PedestalMaxHealth && pedestal.IsDestroyedByPlayer()) {
                        health = PedestalMaxHealth;
                        // Let other systems know the enemies repaired a pedestal
                        pedestal.PedestalRepaired();
                        AudioSource.PlayClipAtPoint(restoreSound, transform.position);
                        EventBus.Publish(new PedestalPartialEvent(pedestal.getUUID(), false));
                    }
                    else {
                        switch (healthDelta) {
                            case > 0:
                                AudioSource.PlayClipAtPoint(healSound, transform.position);
                                EventBus.Publish(new PedestalPartialEvent(pedestal.getUUID(), true));
                                break;
                            case < 0:
                                anim.SetTrigger(TookDamage);
                                AudioSource.PlayClipAtPoint(damageSound, transform.position);
                                break;
                        }
                    }

                    break;
                }
            }

            switch (health) {
                case 0:
                    EventBus.Publish(new PedestalPartialEvent(pedestal.getUUID(), false));
                    break;
            }

            // Health should be a value between 0 and 1, ie pct of full health
            anim.SetFloat(Health, health / PedestalMaxHealth);
        }
    }
}