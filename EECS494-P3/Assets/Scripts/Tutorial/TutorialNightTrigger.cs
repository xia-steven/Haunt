using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialNightTrigger : MonoBehaviour {
    [SerializeField] int nightStartTutorialMessageID = 4;
    [SerializeField] int nightEndTutorialMessageID = 5;
    [SerializeField] GameObject timerText;

    bool sent = false;
    bool messageFinished = false;

    Subscription<PedestalDestroyedEvent> pedDestSub;
    Subscription<MessageFinishedEvent> messFinSub;
    Subscription<NightEndEvent> nightEndSub;

    // Start is called before the first frame update
    void Start() {
        pedDestSub = EventBus.Subscribe<PedestalDestroyedEvent>(onPedestalDestroyed);
        messFinSub = EventBus.Subscribe<MessageFinishedEvent>(onMessageAcknowledged);
        nightEndSub = EventBus.Subscribe<NightEndEvent>(onNightEnd);
        timerText.SetActive(false);
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(pedDestSub);
        EventBus.Unsubscribe(messFinSub);
        EventBus.Unsubscribe(nightEndSub);
    }

    void onPedestalDestroyed(PedestalDestroyedEvent pde) {
        if (!sent) {
            EventBus.Publish(new TutorialMessageEvent(nightStartTutorialMessageID, GetInstanceID(), false,
                KeyCode.Mouse0, true));
            sent = true;
            StartCoroutine(startTutorialNight());
        }
    }


    IEnumerator startTutorialNight() {
        while (!messageFinished) {
            yield return null;
        }

        GameControl.StartNight();
        timerText.SetActive(true);
    }

    void onMessageAcknowledged(MessageFinishedEvent mfe) {
        if (mfe.senderInstanceID == GetInstanceID()) {
            messageFinished = true;
        }
    }


    void onNightEnd(NightEndEvent nee) {
        EventBus.Publish(new TutorialMessageEvent(nightEndTutorialMessageID, GetInstanceID(), false));
    }
}