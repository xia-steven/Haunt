using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class MessageManager : MonoBehaviour
{
    [SerializeField] float fadeTime = 1.0f;
    [SerializeField] TMP_Text text;
    [SerializeField] Image textBackground;
    [SerializeField] Image clickIcon;
    [Tooltip("Number of frames to wait in between each character")]
    float textDelay = 0.04f;

    bool sendingMessage = false;
    Color backgroundOpaqueColor;

    PlayerControls controls;
    InputAction leftClickCallback;
    bool exitEarly = false;

    Queue<MessageEvent> queuedMessages;

    Subscription<MessageEvent> messageSub;

    // Start is called before the first frame update
    void Start()
    {
        controls = new PlayerControls();
        leftClickCallback = controls.Player.Fire;
        leftClickCallback.Enable();
        leftClickCallback.performed += SkipMessage;

        queuedMessages = new Queue<MessageEvent>();
        text.gameObject.SetActive(false);
        textBackground.gameObject.SetActive(false);
        backgroundOpaqueColor = textBackground.color;
        textBackground.color = new Color(backgroundOpaqueColor.r, backgroundOpaqueColor.g, backgroundOpaqueColor.b, 0);

        messageSub = EventBus.Subscribe<MessageEvent>(onMessageReceived);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(messageSub);
        leftClickCallback.Disable();
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

    void onMessageReceived(MessageEvent mes)
    {
        queuedMessages.Enqueue(mes);
    }

    void SkipMessage(InputAction.CallbackContext context)
    {
        exitEarly = true;
    }


    IEnumerator SendMessage(MessageEvent message)
    {

        if(message.pauseTime)
        {
            // Set timescales to 0
            TimeManager.SetTimeScale(0);
        }

        // Fade In
        float initial_time = Time.realtimeSinceStartup;
        float progress = (Time.realtimeSinceStartup - initial_time) / fadeTime;
        textBackground.gameObject.SetActive(true);

        while (progress < 1.0f)
        {
            progress = (Time.realtimeSinceStartup - initial_time) / fadeTime;
            textBackground.color = new Color(backgroundOpaqueColor.r, backgroundOpaqueColor.g,
                    backgroundOpaqueColor.b, backgroundOpaqueColor.a * progress);

            yield return null;
        }

        // Display message(s)
        text.text = "";
        text.gameObject.SetActive(true);
        clickIcon.gameObject.SetActive(true);
        // Loop through each string
        for (int a = 0; a < message.messages.Count; ++a)
        {
            text.text = "";
            exitEarly = false;
            // Loop through each character for each string
            string currMessage = message.messages[a];
            for(int b = 0; b < currMessage.Length; ++b)
            {
                text.text += currMessage[b];
                if(currMessage[b] == '<')
                {
                    // Print the whole format code at once
                    while(currMessage[b] != '>')
                    {
                        b++;
                        if(b >= currMessage.Length)
                        {
                            Debug.LogError("Message Manager: format code not closed");
                            break;
                        }
                        text.text += currMessage[b];
                    }
                }
                // Wait textDelay seconds before showing the next character
                if(!exitEarly)
                {
                    yield return new WaitForSecondsRealtime(textDelay);
                }
            }

            // Make sure don't accidentally get previous left click
            yield return null;

            // Wait for user to acknowledge the message
            while (!Input.GetKeyDown(message.keyToWaitFor))
            {
                yield return null;
            }
            // Go to the next frame for keydown to be false
            yield return null;
        }

        if(message.pauseTime)
        {
            // Set timescales back to 1
            TimeManager.ResetTimeScale();
        }

        // Let other scripts know the message finished
        Debug.Log("Finished message from sender " + message.senderInstanceID);
        EventBus.Publish(new MessageFinishedEvent(message.senderInstanceID));

        // Fade out
        initial_time = Time.realtimeSinceStartup;
        progress = (Time.realtimeSinceStartup - initial_time) / fadeTime;
        Color textColor = text.color;
        Color prevIconColor = clickIcon.color;

        while (progress < 1.0f)
        {
            progress = (Time.realtimeSinceStartup - initial_time) / fadeTime;
            // Fade out both the text background and text
            textBackground.color = new Color(backgroundOpaqueColor.r, backgroundOpaqueColor.g,
                    backgroundOpaqueColor.b, backgroundOpaqueColor.a * (1 - progress));
            text.color = new Color(textColor.r, textColor.g, textColor.b, textColor.a * (1 - progress));
            clickIcon.color = new Color(prevIconColor.r, prevIconColor.g,
                    prevIconColor.b, prevIconColor.a * (1 - progress));

            yield return null;
        }

        text.gameObject.SetActive(false);
        clickIcon.gameObject.SetActive(false);
        textBackground.gameObject.SetActive(false);

        // reset text color
        text.color = textColor;
        sendingMessage = false;
    }


}
