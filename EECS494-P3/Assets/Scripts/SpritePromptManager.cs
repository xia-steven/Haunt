using System.Collections;
using Events;
using Player;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpritePromptManager : MonoBehaviour {
    private static SpritePromptManager instance;

    private Subscription<SpritePromptEvent> promptSub;

    private SpriteRenderer sprite;

    private readonly Vector3 offset = new(-0.5f, 1.25f, 0);

    private void Awake() {
        //enforce singleton
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        promptSub = EventBus.Subscribe<SpritePromptEvent>(onSpritePrompt);

        sprite = GetComponent<SpriteRenderer>();

        sprite.enabled = false;
    }

    private void Start() {
        var player = IsPlayer.instance.transform;

        transform.parent = player;

        transform.localPosition = offset;
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(promptSub);
    }


    private void onSpritePrompt(SpritePromptEvent spe) {
        StartCoroutine(displayPrompt(spe));
    }

    private IEnumerator displayPrompt(SpritePromptEvent spe) {
        sprite.sprite = spe.sprite;


        sprite.enabled = true;

        switch (spe.dismissKey) {
            // Wait for dismiss key (or keys if W)
            case KeyCode.W: {
                while (!Input.GetKeyDown(spe.dismissKey) && !Input.GetKeyDown(KeyCode.A) &&
                       !Input.GetKeyDown(KeyCode.S) && !Input.GetKeyDown(KeyCode.D) && !spe.cancelPrompt)
                    yield return null;
                break;
            }
            default: {
                while (!Input.GetKeyDown(spe.dismissKey) && !spe.cancelPrompt)
                    yield return null;
                break;
            }
        }


        sprite.enabled = false;
    }
}