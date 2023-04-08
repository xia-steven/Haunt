using Events;
using UnityEngine;

namespace Hub.Upgrades {
    public class HasReflectUpgrade : MonoBehaviour {
        private GameObject dashShield;
        private GameObject appliedShield;
        private Subscription<PlayerDodgeEvent> dodgeEvent;

        // Start is called before the first frame update
        private void Start() {
            dashShield = Resources.Load<GameObject>("Prefabs/DashShield");
            dodgeEvent = EventBus.Subscribe<PlayerDodgeEvent>(_OnDodge);
        }

        // Attach shield on dodge start and destroy it on dodge finish
        private void _OnDodge(PlayerDodgeEvent e) {
            switch (e.start) {
                case true:
                    appliedShield = Instantiate(dashShield, transform);
                    break;
                default:
                    Destroy(appliedShield);
                    break;
            }
        }

        protected void OnDestroy() {
            EventBus.Unsubscribe(dodgeEvent);
        }
    }
}