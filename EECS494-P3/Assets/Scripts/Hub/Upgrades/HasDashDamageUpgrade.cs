using System.Collections;
using Events;
using Player;
using UnityEngine;

namespace Hub.Upgrades {
    public class HasDashDamageUpgrade : MonoBehaviour {
        public float cooldown;
        public float dmgMod;

        private float dodgeTime;

        private bool coolingDown;
        private bool increased;

        private Subscription<PlayerDodgeEvent> dodgeSub;
        private Subscription<FireEvent> fireSub;

        // Start is called before the first frame update
        private void Start() {
            EventBus.Subscribe<PlayerDodgeEvent>(_OnDash);
            EventBus.Subscribe<FireEvent>(_OnFire);
        }

        private void _OnDash(PlayerDodgeEvent e) {
            switch (coolingDown) {
                case false: {
                    StartCoroutine(Cooldown());
                    switch (increased) {
                        case false:
                            PlayerModifiers.damage *= dmgMod;
                            increased = true;
                            // TODO: add visual of increase here
                            break;
                    }

                    break;
                }
            }
        }

        private void _OnFire(FireEvent e) {
            StartCoroutine(DecreaseAfterTick());
        }

        private IEnumerator DecreaseAfterTick() {
            yield return null;
            switch (increased) {
                case true:
                    PlayerModifiers.damage /= dmgMod;
                    increased = false;
                    // TODO: remove visual of increase here
                    break;
            }
        }


        private IEnumerator Cooldown() {
            coolingDown = true;
            yield return new WaitForSeconds(cooldown);
            coolingDown = false;

            switch (increased) {
                case true:
                    PlayerModifiers.damage /= dmgMod;
                    increased = false;
                    // TODO: remove visual of increase here
                    break;
            }
        }
    }
}