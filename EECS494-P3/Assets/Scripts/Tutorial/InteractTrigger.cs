using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    [SerializeField] Sprite initialSprite;
    [SerializeField] Sprite pressedSprite;
    [SerializeField] KeyCode dismissKey;

    bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(!triggered)
        {
            triggered = true;
            EventBus.Publish(new SpritePromptEvent(initialSprite, pressedSprite, dismissKey));
        }


    }
}
