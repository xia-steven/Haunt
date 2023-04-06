using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    [SerializeField] Sprite sprite;
    [SerializeField] KeyCode dismissKey;

    bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(!triggered)
        {
            //triggered = true;
            EventBus.Publish(new SpritePromptEvent(sprite, dismissKey));
        }


    }
}
