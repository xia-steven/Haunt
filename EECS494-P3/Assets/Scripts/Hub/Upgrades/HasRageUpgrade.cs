using Events;
using Player;
using UnityEngine;

namespace Hub.Upgrades {
    public class HasRageUpgrade : MonoBehaviour {
        public float duration;
        public float moveMod;
        public float dmgMod;

        private float hitTime;

        private bool active;
        private bool statsChanged;

        private Subscription<PlayerDamagedEvent> dmgSub;

        // Start is called before the first frame update
        private void Start() {
            EventBus.Subscribe<PlayerDamagedEvent>(_OnDamage);
        }

        // Update is called once per frame
        private void Update() {
            switch (active) {
                case true: {
                    switch (statsChanged) {
                        case false:
                            PlayerModifiers.damage *= dmgMod;
                            PlayerModifiers.moveSpeed *= moveMod;
                            statsChanged = true;
                            //TODO: change player visuals here
                            break;
                    }

                    if (Time.time - hitTime > duration) active = false;
                    break;
                }
            }

            switch (active) {
                case false when statsChanged:
                    PlayerModifiers.damage /= dmgMod;
                    PlayerModifiers.moveSpeed /= moveMod;
                    statsChanged = false;
                    //TODO: change player visuals here
                    break;
            }
        }

        private void _OnDamage(PlayerDamagedEvent e) {
            active = true;
            hitTime = Time.time;
        }
    }
}