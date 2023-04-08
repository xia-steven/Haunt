using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCMessages : MonoBehaviour {
    [SerializeField] string configName = "REPLACE ME";

    MessageList NPCMessageData;
    [SerializeField] GameObject interactSprite;
    [SerializeField] GameObject initialBubble;
    [SerializeField] bool isGhost = false;
    bool selected = false;

    bool spoken = false;

    Sprite eSprite;
    SpritePromptEvent ePrompt;

    Subscription<TryInteractEvent> interactSubscription;
    Subscription<MessageFinishedEvent> finishedSub;

    // Start is called before the first frame update
    void Start() {
        NPCMessageData = ConfigManager.GetData<MessageList>(configName);
        interactSubscription = EventBus.Subscribe<TryInteractEvent>(OnInteract);
        finishedSub = EventBus.Subscribe<MessageFinishedEvent>(OnMessageFinished);

        interactSprite.SetActive(false);
        Object[] sprites = Resources.LoadAll("tilemap");
        eSprite = (Sprite)sprites[360];

        ePrompt = new SpritePromptEvent(eSprite, KeyCode.E);
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(interactSubscription);
    }

    private void OnTriggerStay(Collider other) {
        if (spoken) {
            interactSprite.SetActive(true);
        }

        selected = true;
        ePrompt.cancelPrompt = false;
        EventBus.Publish(ePrompt);
    }

    private void OnTriggerExit(Collider other) {
        if (spoken) {
            interactSprite.SetActive(false);
        }

        selected = false;
        ePrompt.cancelPrompt = true;
    }

    private void OnInteract(TryInteractEvent e) {
        if (selected) {
            spoken = true;
            initialBubble.SetActive(false);
            if (SceneManager.GetActiveScene().name == "TutorialHubWorld") {
                // Ghost sends initial message
                EventBus.Publish(new MessageEvent(NPCMessageData.initialTutorial, GetInstanceID(), false));
            }
            else {
                // Standard dialogue for the night
                EventBus.Publish(new MessageEvent(NPCMessageData.allMessages[GameControl.Day].messages, GetInstanceID(),
                    false));
            }
        }
    }

    private void OnMessageFinished(MessageFinishedEvent mfe) {
        // Activate teleporter after first message
        if (mfe.senderInstanceID == GetInstanceID() && isGhost &&
            SceneManager.GetActiveScene().name == "TutorialHubWorld") {
            EventBus.Publish(new ActivateTeleporterEvent());
        }
        // Activate sword after first shopkeeper message
        else if (mfe.senderInstanceID == GetInstanceID() && !isGhost && GameControl.Day == 0) { }
    }
}