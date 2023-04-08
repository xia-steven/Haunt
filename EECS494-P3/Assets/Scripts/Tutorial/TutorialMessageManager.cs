using ConfigDataTypes;
using Events;
using JSON_Parsing;
using UnityEngine;

namespace Tutorial {
    public class TutorialMessageManager : MonoBehaviour {
        [SerializeField] private string configPath = "TutorialData";

        private Subscription<TutorialMessageEvent> tutorMesSub;

        private MessageList tutorialData;
        private int previousMessage = -1;

        // Start is called before the first frame update
        private void Start() {
            tutorMesSub = EventBus.Subscribe<TutorialMessageEvent>(onTutorialMessageSent);

            // Load data
            tutorialData = ConfigManager.GetData<MessageList>(configPath);
        }

        private void OnDestroy() {
            EventBus.Unsubscribe(tutorMesSub);
        }

        private void onTutorialMessageSent(TutorialMessageEvent tme) {
            if (previousMessage < tme.messageID) {
                Debug.Log("Sending tutorial message");
                previousMessage = tme.messageID;
                // Send message event
                EventBus.Publish(new MessageEvent(tutorialData.allMessages[tme.messageID].messages, tme.senderInstanceID,
                    tme.pauseTime, tme.keyToWaitFor, tme.unpauseBeforeFade));
            }
            else {
                Debug.Log("Attempted to send an out of order message. Skipping");
            }
        }
    }
}