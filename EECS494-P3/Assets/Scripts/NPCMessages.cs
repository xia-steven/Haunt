using ConfigDataTypes;
using Events;
using JSON_Parsing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCMessages : MonoBehaviour {
    [SerializeField] private string configName = "REPLACE ME";

    private MessageList NPCMessageData;
    [SerializeField] private GameObject interactSprite;
    [SerializeField] private GameObject initialBubble;
    [SerializeField] private bool isGhost;
    private bool selected;
    private bool spoken;

    private Sprite eSprite;
    private SpritePromptEvent ePrompt;

    private Subscription<TryInteractEvent> interactSubscription;
    private Subscription<MessageFinishedEvent> finishedSub;

    // Start is called before the first frame update
    private void Start() {
        NPCMessageData = ConfigManager.GetData<MessageList>(configName);
        interactSubscription = EventBus.Subscribe<TryInteractEvent>(OnInteract);
        finishedSub = EventBus.Subscribe<MessageFinishedEvent>(OnMessageFinished);

        interactSprite.SetActive(false);
        var sprites = Resources.LoadAll("tilemap");
        eSprite = (Sprite)sprites[360];

        ePrompt = new SpritePromptEvent(eSprite, KeyCode.E);
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(interactSubscription);
    }

    private void OnTriggerStay(Collider other) {
        switch (spoken) {
            case true:
                interactSprite.SetActive(true);
                break;
        }

        selected = true;
        ePrompt.cancelPrompt = false;
        EventBus.Publish(ePrompt);
    }

    private void OnTriggerExit(Collider other) {
        switch (spoken) {
            case true:
                interactSprite.SetActive(false);
                break;
        }

        selected = false;
        ePrompt.cancelPrompt = true;
    }

    private void OnInteract(TryInteractEvent e) {
        switch (selected) {
            case true: {
                spoken = true;
                initialBubble.SetActive(false);
                if (SceneManager.GetActiveScene().name == "TutorialHubWorld")
                    // Ghost sends initial message
                    EventBus.Publish(new MessageEvent(NPCMessageData.initialTutorial, GetInstanceID(), false));
                else
                    // Standard dialogue for the night
                    EventBus.Publish(new MessageEvent(NPCMessageData.allMessages[Game_Control.GameControl.Day].messages, GetInstanceID(),
                        false));
                break;
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
        else if (mfe.senderInstanceID == GetInstanceID() && !isGhost && Game_Control.GameControl.Day == 0) { }
    }
}