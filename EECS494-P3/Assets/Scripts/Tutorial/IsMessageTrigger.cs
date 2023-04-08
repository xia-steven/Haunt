using Events;
using UnityEngine;

namespace Tutorial {
    public class IsMessageTrigger : MonoBehaviour {
        [SerializeField] private int tutorialMessageID = -1;

        private bool sent;


        private void OnTriggerEnter(Collider other) {
            switch (sent) {
                case false when other.gameObject.layer == LayerMask.NameToLayer("Player"):
                    EventBus.Publish(new TutorialMessageEvent(tutorialMessageID, GetInstanceID(), false));
                    sent = true;
                    break;
            }
        }
    }
}