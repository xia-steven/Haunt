using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpritePromptManager : MonoBehaviour {
    static SpritePromptManager instance;

    Subscription<SpritePromptEvent> promptSub;

    SpriteRenderer sprite;

    Vector3 offset = new Vector3(-0.5f, 1.25f, 0);

    void Awake() {
        //enforce singleton
        if (instance == null) instance = this;
        else Destroy(this.gameObject);

        DontDestroyOnLoad(gameObject);

        promptSub = EventBus.Subscribe<SpritePromptEvent>(onSpritePrompt);

        sprite = GetComponent<SpriteRenderer>();

        sprite.enabled = false;
    }

    private void Start() {
        Transform player = IsPlayer.instance.transform;

        transform.parent = player;

        transform.localPosition = offset;
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(promptSub);
    }


    void onSpritePrompt(SpritePromptEvent spe) {
        StartCoroutine(displayPrompt(spe));
    }

    IEnumerator displayPrompt(SpritePromptEvent spe) {
        sprite.sprite = spe.sprite;


        sprite.enabled = true;

        // Wait for dismiss key (or keys if W)
        if (spe.dismissKey == KeyCode.W) {
            while (!Input.GetKeyDown(spe.dismissKey) && !Input.GetKeyDown(KeyCode.A) &&
                   !Input.GetKeyDown(KeyCode.S) && !Input.GetKeyDown(KeyCode.D) && !spe.cancelPrompt) {
                yield return null;
            }
        }
        else {
            while (!Input.GetKeyDown(spe.dismissKey) && !spe.cancelPrompt) {
                yield return null;
            }
        }


        sprite.enabled = false;
    }
}