using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpritePromptManager : MonoBehaviour
{
    static SpritePromptManager instance;

    Subscription<SpritePromptEvent> promptSub;

    SpriteRenderer spriteRend;

    Vector3 offset = new Vector3(-0.5f, 1.25f, 0);
    float fadeTime = 0.5f;

    bool prompting = false;
    Coroutine currCoroutine;

    Color initialColor;

    Stack<SpritePromptEvent> requestBacklog;

    SpritePromptEvent currRequest;

    void Awake()
    {
        //enforce singleton
        if (instance == null) instance = this;
        else Destroy(this.gameObject);

        DontDestroyOnLoad(gameObject);

        promptSub = EventBus.Subscribe<SpritePromptEvent>(onSpritePrompt);

        spriteRend = GetComponent<SpriteRenderer>();

        spriteRend.enabled = false;

        requestBacklog = new Stack<SpritePromptEvent>();
    }

    private void Start()
    {
        Transform player = IsPlayer.instance.transform;

        transform.parent = player;

        transform.localPosition = offset;


        initialColor = spriteRend.color;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(promptSub);
    }

    private void Update()
    {
        if(!prompting && requestBacklog.Count > 0)
        {
            // Send request if one in the backlog
            currRequest = requestBacklog.Pop();

            if(!currRequest.cancelPrompt)
            {
                prompting = true;
                currCoroutine = StartCoroutine(displayPrompt(currRequest));
            }
        }
    }


    void onSpritePrompt(SpritePromptEvent spe)
    {
        if(!prompting)
        {
            prompting = true;
            currRequest = spe;
            currCoroutine = StartCoroutine(displayPrompt(spe));
        }
        else
        {
            // Add current request to waiting list
            requestBacklog.Push(currRequest);
            StopCoroutine(currCoroutine);
            prompting = true;
            currRequest = spe;
            currCoroutine = StartCoroutine(displayPrompt(spe));
        }
    }

    IEnumerator displayPrompt(SpritePromptEvent spe)
    {
        spriteRend.sprite = spe.pressedSprite;
        spriteRend.enabled = true;

        // Reset color
        spriteRend.color = initialColor;

        int startTime = (int)(Time.time * 2);
        bool pressed = true;

        // Wait for dismiss key (or keys if W)
        if (spe.dismissKey == KeyCode.W)
        {
            while (!Input.GetKeyDown(spe.dismissKey) && !Input.GetKeyDown(KeyCode.A) &&
                !Input.GetKeyDown(KeyCode.S) && !Input.GetKeyDown(KeyCode.D) && !spe.cancelPrompt)
            {
                // If 0.5 seconds have passed, switch sprites
                if(startTime != (int)(Time.time * 2))
                {
                    startTime = (int)(Time.time * 2);
                    if(pressed)
                    {
                        spriteRend.sprite = spe.initialSprite;
                    }
                    else
                    {
                        spriteRend.sprite = spe.pressedSprite;
                    }
                    pressed = !pressed;
                }


                yield return null;
            }
        }
        else
        {
            while (!Input.GetKeyDown(spe.dismissKey) && !spe.cancelPrompt)
            {
                // If 0.5 seconds have passed, switch sprites
                if (startTime != (int)(Time.time * 2))
                {
                    startTime = (int)(Time.time * 2);
                    if (pressed)
                    {
                        spriteRend.sprite = spe.initialSprite;
                    }
                    else
                    {
                        spriteRend.sprite = spe.pressedSprite;
                    }
                    pressed = !pressed;
                }

                yield return null;
            }
        }

        // Fade out sprite
        float initialTime = Time.time;
        float progress = (Time.time - initialTime) / fadeTime;

        while (progress < 1.0f)
        {
            progress = (Time.time - initialTime) / fadeTime;

            spriteRend.color = new Color(initialColor.r, initialColor.g, initialColor.b, (1 - progress));

            yield return null;
        }

        spriteRend.enabled = false;
        spriteRend.color = initialColor;

        prompting = false;
    }
}
