using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsMessageTrigger : MonoBehaviour
{
    [SerializeField] string message = "REPLACE ME";
    [SerializeField] Color messageColor;
    [SerializeField] bool repeat = false;
    [SerializeField] bool waitForKey = false;
    [SerializeField] KeyCode keyToWaitFor = KeyCode.None;
    [SerializeField] float messageDelay = 0f;
    [SerializeField] bool disableMovementUntilSent = false;

    bool sent = false;


    private void OnTriggerEnter(Collider other)
    {
        if(!sent || repeat)
        {
            sent = true;
            StartCoroutine(sendMessage());
        }
    }

    IEnumerator sendMessage()
    {
        if(disableMovementUntilSent)
        {
            EventBus.Publish(new DisablePlayerEvent());
        }

        yield return new WaitForSeconds(messageDelay);
        if(disableMovementUntilSent)
        {
            EventBus.Publish(new EnablePlayerEvent());
        }

        EventBus.Publish(new ToastRequestEvent(messageColor, message, waitForKey, keyToWaitFor));
    }
}
