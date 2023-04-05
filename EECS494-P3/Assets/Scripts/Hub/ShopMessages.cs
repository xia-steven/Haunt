using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMessages : MonoBehaviour
{
    string configName = "ShopMessageData";

    MessageList shopData;
    [SerializeField] GameObject interactSprite;
    bool selected = false;

    Subscription<TryInteractEvent> interactSubscription;

    // Start is called before the first frame update
    void Start()
    {
        shopData = ConfigManager.GetData<MessageList>(configName);
        interactSubscription = EventBus.Subscribe<TryInteractEvent>(OnInteract);

        interactSprite.SetActive(false);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(interactSubscription);
    }

    private void OnTriggerStay(Collider other)
    {
        interactSprite.SetActive(true);
        selected = true;
    }

    private void OnTriggerExit(Collider other)
    {
        interactSprite.SetActive(false);
        selected = false;
    }

    private void OnInteract(TryInteractEvent e)
    {
        if(selected)
        {
            EventBus.Publish(new MessageEvent(shopData.allMessages[GameControl.Day].messages, GetInstanceID(), false));
        }
    }
}
