using Events;
using UnityEngine;

namespace Tutorial {
    public class InteractTrigger : MonoBehaviour {
        [SerializeField] private Sprite sprite;
        [SerializeField] private KeyCode dismissKey;

        private bool triggered;

        private void OnTriggerEnter(Collider other) {
            switch (triggered) {
                case false:
                    triggered = true;
                    EventBus.Publish(new SpritePromptEvent(sprite, dismissKey));
                    break;
            }
        }
    }
}