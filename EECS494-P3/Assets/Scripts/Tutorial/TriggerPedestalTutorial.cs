using Events;
using UnityEngine;

namespace Tutorial {
    public class TriggerPedestalTutorial : MonoBehaviour {
        [SerializeField] private int tutorialMessageID = 3;

        private void OnDestroy() {
            // Send the tutorial message
            EventBus.Publish(new TutorialMessageEvent(tutorialMessageID, GetInstanceID(), false));
        }
    }
}