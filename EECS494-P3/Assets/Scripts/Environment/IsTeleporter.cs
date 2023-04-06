using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class IsTeleporter : MonoBehaviour
{
    [SerializeField] string otherScene;
    
    MeshRenderer visualRenderer;
    private Animator anim;


    Sprite eSprite;
    SpritePromptEvent ePrompt;

    bool sentPrompt = false;

    bool isActive = true;
    public bool Active {
        get { return isActive; }
        set 
        {
            bool tmp = isActive;
            isActive = value;
            if (isActive && !tmp) Activate();
            else if (tmp) Deactivate();
        }
    }

    bool isUsable = false;

    Subscription<TryInteractEvent> interactSub;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();

        Activate();

        interactSub = EventBus.Subscribe<TryInteractEvent>(_Interact);

        Object[] sprites = Resources.LoadAll("tilemap");
        eSprite = (Sprite)sprites[360];

        ePrompt = new SpritePromptEvent(eSprite, KeyCode.E);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isActive && other.CompareTag("Player") && !sentPrompt)
        {
            isUsable = true;
            ePrompt.cancelPrompt = false;
            EventBus.Publish<SpritePromptEvent>(ePrompt);
            sentPrompt = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ePrompt.cancelPrompt = true;
            sentPrompt = false;
            isUsable = false;
        }
    }

    private void Activate()
    {
        Debug.Log((isActive));
        //activate other visuals here
        anim.SetBool("isActive",isActive);
        
    }

    private void Deactivate()
    {

        //deactivate other visuals here
        Debug.Log(isActive);
        anim.SetBool("isActive",isActive);
    }

    public void _Interact(TryInteractEvent e)
    {
        if (isUsable)
        {
            IsPlayer.SetPosition(new Vector3(0, .25f, 0));
            if(GameControl.Day != 3 || SceneManager.GetActiveScene().name != "HubWorld")
            {
                SceneManager.LoadScene(otherScene);
            }
            else if (SceneManager.GetActiveScene().name == "HubWorld")
            {
                SceneManager.LoadScene("LAB_BossTesting");
            }
        }
    }
}
