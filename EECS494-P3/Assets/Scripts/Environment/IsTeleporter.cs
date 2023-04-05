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
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("IS Active: " + isActive);
        if (isActive && other.CompareTag("Player"))
        {
            isUsable = true;
            EventBus.Publish(new ToastRequestEvent("Press E To Teleport", true, KeyCode.E));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
