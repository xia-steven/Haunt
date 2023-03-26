using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRangedEnemyEncounter : MonoBehaviour
{
    [SerializeField] int tutorialMessageID = 1;

    bool sent = false;

    bool messageFinished = false;

    Subscription<MessageFinishedEvent> messFinSub;

    private void Start()
    {
        messFinSub = EventBus.Subscribe<MessageFinishedEvent>(onMessageFinished);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(messFinSub);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!sent)
        {
            StartCoroutine(RangedEnemyCoroutine());
            sent = true;
        }
    }

    IEnumerator RangedEnemyCoroutine()
    {
        //Disable player movement
        EventBus.Publish(new DisablePlayerEvent());

        //Lock the camera
        EventBus.Publish(new TutorialLockCameraEvent(new Vector3(-1, 12, -12)));

        // Wait for bullet to reach the right spot
        yield return new WaitForSeconds(4.25f);

        // Send the tutorial message
        EventBus.Publish(new TutorialMessageEvent(tutorialMessageID, GetInstanceID(), KeyCode.Space, true));

        // Wait for message to finish
        while(!messageFinished)
        {
            yield return null;
        }

        // Perform tutorial dodge
        EventBus.Publish(new TutorialDodgeStartEvent(Vector3.left));
        yield return new WaitForSecondsRealtime(0.35f);
        EventBus.Publish(new TutorialDodgeEndEvent());

        // Unlock the camera
        EventBus.Publish(new TutorialUnlockCameraEvent());

        //Enable player movement
        EventBus.Publish(new EnablePlayerEvent());
    }


    void onMessageFinished(MessageFinishedEvent mfe)
    {
        if(mfe.senderInstanceID == GetInstanceID())
        {
            Debug.Log("Message finished");
            messageFinished = true;
        }
    }

}
