using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    [SerializeField] float fadeTime = 1.0f;

    bool sendingMessage = false;

    Queue<MessageEvent> queuedMessages;

    // Start is called before the first frame update
    void Start()
    {
        queuedMessages = new Queue<MessageEvent>();


    }

    // Update is called once per frame
    void Update()
    {
        if(queuedMessages.Count > 0 && !sendingMessage)
        {
            sendingMessage = true;
            MessageEvent message = queuedMessages.Dequeue();
            StartCoroutine(SendMessage(message));
        }
    }


    IEnumerator SendMessage(MessageEvent message)
    {
        // Fade In
        yield return null;

        // Display message(s)


        // Fade out
        yield return null;
        sendingMessage = false;
    }
}
