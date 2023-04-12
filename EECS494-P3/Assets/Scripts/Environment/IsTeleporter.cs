using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IsTeleporter : MonoBehaviour {
    [SerializeField] string otherScene;
    [SerializeField] private GameObject selectedRing;

    MeshRenderer visualRenderer;
    private Animator anim;


    Sprite eSprite;
    SpritePromptEvent ePrompt;

    bool sentPrompt = false;

    bool isActive = true;

    public bool Active {
        get { return isActive; }
        set {
            bool tmp = isActive;
            isActive = value;
            if (isActive && !tmp) Activate();
            else if (tmp) Deactivate();
        }
    }

    bool isUsable = false;

    Subscription<TryInteractEvent> interactSub;

    // Start is called before the first frame update
    void Awake() {
        anim = GetComponent<Animator>();

        Activate();

        interactSub = EventBus.Subscribe<TryInteractEvent>(_Interact);

        UnityEngine.Object[] sprites = Resources.LoadAll("tilemap");
        eSprite = (Sprite)sprites[360];

        ePrompt = new SpritePromptEvent(eSprite, KeyCode.E);
    }

    private void OnTriggerStay(Collider other) {
        if (!isUsable && isActive && other.CompareTag("Player") && !sentPrompt) {
            selectedRing.SetActive(true);
            isUsable = true;
            ePrompt.cancelPrompt = false;
            EventBus.Publish<SpritePromptEvent>(ePrompt);
            sentPrompt = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            ePrompt.cancelPrompt = true;
            sentPrompt = false;
            selectedRing.SetActive(false);
            isUsable = false;
        }
    }

    private void Activate() {
        //activate other visuals here
        anim.SetBool("isActive", isActive);
        Debug.Log("Activating!");
    }

    private void Deactivate() {
        //deactivate other visuals here
        anim.SetBool("isActive", isActive);
    }

    public void _Interact(TryInteractEvent e) {
        if (isUsable && IsPlayer.instance.GetHealth() > 0) {
            IsPlayer.SetPosition(new Vector3(0, .5f, 0));
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Audio/Movement/Teleport"), transform.position);
            // Set player invincible
            // Disabled on scene load in playerhashealth
            EventBus.Publish(new ToggleInvincibilityEvent(true));
            SceneTransition.currentScene.FadeToScene();
        }
    }
}