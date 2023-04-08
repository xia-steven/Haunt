using System.Collections;
using Events;
using UnityEngine;

namespace Tutorial {
    public class TutorialNightTrigger : MonoBehaviour {
        [SerializeField] private int nightStartTutorialMessageID = 4;
        [SerializeField] private int nightEndTutorialMessageID = 5;
        [SerializeField] private GameObject timerText;

        private bool sent;
        private bool messageFinished;

        private Subscription<PedestalDestroyedEvent> pedDestSub;
        private Subscription<MessageFinishedEvent> messFinSub;
        private Subscription<NightEndEvent> nightEndSub;

        // Start is called before the first frame update
        private void Start() {
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

        private void onPedestalDestroyed(PedestalDestroyedEvent pde) {
            switch (sent) {
                case false:
                    EventBus.Publish(new TutorialMessageEvent(nightStartTutorialMessageID, GetInstanceID(), false,
                        KeyCode.Mouse0, true));
                    sent = true;
                    StartCoroutine(startTutorialNight());
                    break;
            }
        }


        private IEnumerator startTutorialNight() {
            while (!messageFinished) yield return null;

            Game_Control.GameControl.StartNight();
            timerText.SetActive(true);
        }

        private void onMessageAcknowledged(MessageFinishedEvent mfe) {
            if (mfe.senderInstanceID == GetInstanceID()) messageFinished = true;
        }


        private void onNightEnd(NightEndEvent nee) {
            EventBus.Publish(new TutorialMessageEvent(nightEndTutorialMessageID, GetInstanceID(), false));
        }
    }
}