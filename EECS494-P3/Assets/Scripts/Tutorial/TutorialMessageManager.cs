using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessageManager : MonoBehaviour
{
    [SerializeField] string configPath = "TutorialData";

    Subscription<TutorialMessageEvent> tutorMesSub;

    TutorialMessages data;
    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        tutorMesSub = EventBus.Subscribe<TutorialMessageEvent>(onTutorialMessageSent);

        // Load data
        data = ConfigManager.GetData<TutorialMessages>(configPath);
    }

    private void Update()
    {
        //DebugSendMessage();
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(tutorMesSub);
    }

    void onTutorialMessageSent(TutorialMessageEvent tme)
    {
        Debug.Log("Sending tutorial message");
        // Send message event
        EventBus.Publish(new MessageEvent(data.allMessages[tme.messageID].messages, tme.senderInstanceID, tme.keyToWaitFor, tme.unpauseBeforeFade));
    }

    void DebugSendMessage()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            EventBus.Publish(new TutorialMessageEvent(count, GetInstanceID()));
            count++;
            if (count >= data.allMessages.Count)
            {
                count = 0;
            }
        }
    }
}
