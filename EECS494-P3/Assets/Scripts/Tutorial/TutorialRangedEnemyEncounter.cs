using System.Collections;
using Events;
using UnityEngine;

namespace Tutorial {
    public class TutorialRangedEnemyEncounter : MonoBehaviour {
        [SerializeField] private int tutorialMessageID = 1;

        private bool sent;

        private bool messageFinished;

        private Subscription<MessageFinishedEvent> messFinSub;

        private void Start() {
            messFinSub = EventBus.Subscribe<MessageFinishedEvent>(onMessageFinished);
        }

        private void OnDestroy() {
            EventBus.Unsubscribe(messFinSub);
        }


        private void OnTriggerEnter(Collider other) {
            switch (sent) {
                case false:
                    StartCoroutine(RangedEnemyCoroutine());
                    sent = true;
                    break;
            }
        }

        private IEnumerator RangedEnemyCoroutine() {
            //Disable player movement
            EventBus.Publish(new DisablePlayerEvent());

            //Lock the camera
            EventBus.Publish(new TutorialLockCameraEvent(new Vector3(-1, 12, -12)));

            // Wait for bullet to reach the right spot
            yield return new WaitForSeconds(3.25f);

            // Send the tutorial message
            EventBus.Publish(new TutorialMessageEvent(tutorialMessageID, GetInstanceID(), true, KeyCode.Space, true));

            // Wait for message to finish
            while (!messageFinished) yield return null;

            // Perform tutorial dodge
            EventBus.Publish(new TutorialDodgeStartEvent(Vector3.left));
            yield return new WaitForSecondsRealtime(0.4f);
            EventBus.Publish(new TutorialDodgeEndEvent());


            // Unlock the camera
            EventBus.Publish(new TutorialUnlockCameraEvent());

            //Enable player movement
            EventBus.Publish(new EnablePlayerEvent());
        }


        private void onMessageFinished(MessageFinishedEvent mfe) {
            if (mfe.senderInstanceID == GetInstanceID()) {
                Debug.Log("Message finished");
                messageFinished = true;
            }
        }
    }
}