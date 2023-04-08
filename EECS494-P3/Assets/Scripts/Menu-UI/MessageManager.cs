using System.Collections;
using System.Collections.Generic;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Menu_UI {
    public class MessageManager : MonoBehaviour {
        [SerializeField] private float fadeTime = 1.0f;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image textBackground;
        [SerializeField] private Image clickIcon;

        [Tooltip("Number of frames to wait in between each character")]
        private const float textDelay = 0.04f;

        private bool sendingMessage;
        private Color backgroundOpaqueColor;

        private PlayerControls controls;
        private InputAction leftClickCallback;
        private bool exitEarly;

        private Queue<MessageEvent> queuedMessages;

        private Subscription<MessageEvent> messageSub;

        // Start is called before the first frame update
        private void Start() {
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

        private void OnDestroy() {
            EventBus.Unsubscribe(messageSub);
            leftClickCallback.Disable();
        }

        // Update is called once per frame
        private void Update() {
            switch (queuedMessages.Count) {
                case > 0 when !sendingMessage: {
                    sendingMessage = true;
                    var message = queuedMessages.Dequeue();
                    StartCoroutine(SendMessage(message));
                    break;
                }
            }
        }

        private void onMessageReceived(MessageEvent mes) {
            queuedMessages.Enqueue(mes);
        }

        private void SkipMessage(InputAction.CallbackContext context) {
            exitEarly = true;
        }


        private IEnumerator SendMessage(MessageEvent message) {
            switch (message.pauseTime) {
                // Set timescales to 0
                case true:
                    TimeManager.SetTimeScale(0);
                    break;
            }

            // Fade In
            var initial_time = Time.realtimeSinceStartup;
            var progress = (Time.realtimeSinceStartup - initial_time) / fadeTime;
            textBackground.gameObject.SetActive(true);

            while (progress < 1.0f) {
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
            foreach (var t in message.messages) {
                text.text = "";
                exitEarly = false;
                // Loop through each character for each string
                for (var b = 0; b < t.Length; ++b) {
                    text.text += t[b];
                    switch (t[b]) {
                        // Print the whole format code at once
                        case '<': {
                            while (t[b] != '>') {
                                b++;
                                if (b >= t.Length) {
                                    Debug.LogError("Message Manager: format code not closed");
                                    break;
                                }

                                text.text += t[b];
                            }

                            break;
                        }
                    }

                    switch (exitEarly) {
                        // Wait textDelay seconds before showing the next character
                        case false:
                            yield return new WaitForSecondsRealtime(textDelay);
                            break;
                    }
                }

                // Make sure don't accidentally get previous left click
                yield return null;

                // Wait for user to acknowledge the message
                while (!Input.GetKeyDown(message.keyToWaitFor)) yield return null;

                // Go to the next frame for keydown to be false
                yield return null;
            }

            switch (message.pauseTime) {
                // Set timescales back to 1
                case true:
                    TimeManager.ResetTimeScale();
                    break;
            }

            // Let other scripts know the message finished
            Debug.Log("Finished message from sender " + message.senderInstanceID);
            EventBus.Publish(new MessageFinishedEvent(message.senderInstanceID));

            // Fade out
            initial_time = Time.realtimeSinceStartup;
            progress = (Time.realtimeSinceStartup - initial_time) / fadeTime;
            var textColor = text.color;
            var prevIconColor = clickIcon.color;

            while (progress < 1.0f) {
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
}