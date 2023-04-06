using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMessages : MonoBehaviour
{
    [SerializeField] string configName = "REPLACE ME";

    MessageList NPCMessageData;
    [SerializeField] GameObject interactSprite;
    [SerializeField] GameObject initialBubble;
    bool selected = false;

    bool spoken = false;

    Sprite eSprite;
    SpritePromptEvent ePrompt;

    Subscription<TryInteractEvent> interactSubscription;

    // Start is called before the first frame update
    void Start()
    {
        NPCMessageData = ConfigManager.GetData<MessageList>(configName);
        interactSubscription = EventBus.Subscribe<TryInteractEvent>(OnInteract);

        interactSprite.SetActive(false);
        Object [] sprites = Resources.LoadAll("tilemap");
        eSprite = (Sprite)sprites[360];

        ePrompt = new SpritePromptEvent(eSprite, KeyCode.E);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(interactSubscription);
    }

    private void OnTriggerStay(Collider other)
    {
        if(spoken)
        {
            interactSprite.SetActive(true);
        }
        selected = true;
        ePrompt.cancelPrompt = false;
        EventBus.Publish(ePrompt);
    }

    private void OnTriggerExit(Collider other)
    {
        if(spoken)
        {
            interactSprite.SetActive(false);
        }
        selected = false;
        ePrompt.cancelPrompt = true;
    }

    private void OnInteract(TryInteractEvent e)
    {
        if(selected)
        {
            spoken = true;
            initialBubble.SetActive(false);
            EventBus.Publish(new MessageEvent(NPCMessageData.allMessages[GameControl.Day].messages, GetInstanceID(), false));
        }
    }
}
