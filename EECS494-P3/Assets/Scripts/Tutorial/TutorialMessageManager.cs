using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessageManager : MonoBehaviour {
    [SerializeField] string configPath = "TutorialData";

    Subscription<TutorialMessageEvent> tutorMesSub;

    MessageList tutorialData;
    int previousMessage = -1;

    // Start is called before the first frame update
    void Start() {
        tutorMesSub = EventBus.Subscribe<TutorialMessageEvent>(onTutorialMessageSent);

        // Load data
        tutorialData = ConfigManager.GetData<MessageList>(configPath);
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(tutorMesSub);
    }

    void onTutorialMessageSent(TutorialMessageEvent tme) {
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