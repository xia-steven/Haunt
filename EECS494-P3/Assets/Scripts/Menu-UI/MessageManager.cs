using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageManager : MonoBehaviour
{
    [SerializeField] float fadeTime = 1.0f;
    [SerializeField] TMP_Text text;
    [SerializeField] Image textBackground;
    [Tooltip("Number of frames to wait in between each character")]
    [SerializeField] int textDelay = 12;

    bool sendingMessage = false;
    Color backgroundOpaqueColor;

    Queue<MessageEvent> queuedMessages;

    Subscription<MessageEvent> messageSub;

    // Start is called before the first frame update
    void Start()
    {
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


    IEnumerator SendMessage(MessageEvent message)
    {
        // Set timescales to 0
        TimeManager.SetTimeScale(0);

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
        // Loop through each string
        for(int a = 0; a < message.messages.Count; ++a)
        {
            text.text = "";
            bool exitEarly = false;
            // Loop through each character for each string
            string currMessage = message.messages[a];
            for(int b = 0; b < currMessage.Length && !exitEarly; ++b)
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
                // Wait textDelay frames before showing the next character
                int count = 0;
                while(count < textDelay && !exitEarly)
                {
                    // Allow user to break early
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        text.text = currMessage;
                        exitEarly = true;
                    }

                    count++;
                    yield return null;
                }
            }

            // Wait for user to acknowledge the message
            while (!Input.GetKeyDown(KeyCode.Mouse0))
            {
                yield return null;
            }
            // Go to the next frame for keydown to be false
            yield return null;
        }


        // Fade out
        initial_time = Time.realtimeSinceStartup;
        progress = (Time.realtimeSinceStartup - initial_time) / fadeTime;
        Color textColor = text.color;

        while (progress < 1.0f)
        {
            progress = (Time.realtimeSinceStartup - initial_time) / fadeTime;
            // Fade out both the text background and text
            textBackground.color = new Color(backgroundOpaqueColor.r, backgroundOpaqueColor.g,
                    backgroundOpaqueColor.b, backgroundOpaqueColor.a * (1 - progress));
            text.color = new Color(textColor.r, textColor.g, textColor.b, textColor.a * (1 - progress));

            yield return null;
        }


        text.gameObject.SetActive(false);
        textBackground.gameObject.SetActive(false);

        // Set timescales back to 1
        TimeManager.ResetTimeScale();
    }
}
