using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessageManager : MonoBehaviour
{
    [SerializeField] string configPath = "TutorialData";

    Subscription<TutorialMessageEvent> tutorMesSub;

    TutorialMessages data;
    int previousMessage = -1;

    // Start is called before the first frame update
    void Start()
    {
        tutorMesSub = EventBus.Subscribe<TutorialMessageEvent>(onTutorialMessageSent);

        // Load data
        data = ConfigManager.GetData<TutorialMessages>(configPath);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(tutorMesSub);
    }

    void onTutorialMessageSent(TutorialMessageEvent tme)
    {
        if(previousMessage < tme.messageID)
        {
            Debug.Log("Sending tutorial message");
            previousMessage = tme.messageID;
            // Send message event
            EventBus.Publish(new MessageEvent(data.allMessages[tme.messageID].messages, tme.senderInstanceID, tme.keyToWaitFor, tme.unpauseBeforeFade));
        } else
        {
            Debug.Log("Attempted to send an out of order message. Skipping");
        }
    }
}
