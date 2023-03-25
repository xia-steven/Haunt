using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialNightTrigger : MonoBehaviour
{
    [SerializeField] int tutorialMessageID = 4;

    bool sent = false;
    bool messageFinished = false;

    Subscription<PedestalDestroyedEvent> pedDestSub;
    Subscription<MessageFinishedEvent> messFinSub;

    // Start is called before the first frame update
    void Start()
    {
        pedDestSub = EventBus.Subscribe<PedestalDestroyedEvent>(onPedestalDestroyed);
        messFinSub = EventBus.Subscribe<MessageFinishedEvent>(onMessageAcknowledged);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(pedDestSub);
        EventBus.Unsubscribe(messFinSub);
    }

    void onPedestalDestroyed(PedestalDestroyedEvent pde)
    {
        if(!sent)
        {
            EventBus.Publish(new TutorialMessageEvent(tutorialMessageID, GetInstanceID(), KeyCode.Mouse0, true));
            sent = true;
        }
    }


    IEnumerator startTutorialNight()
    {
        while(!messageFinished)
        {
            yield return null;
        }

        GameControl.StartGame();
    }

    void onMessageAcknowledged(MessageFinishedEvent mfe)
    {
        if(mfe.senderInstanceID == GetInstanceID())
        {
            messageFinished = true;
        }
    }
}
