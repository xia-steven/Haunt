using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class IsTeleporter : MonoBehaviour
{
    [SerializeField] string otherScene;

    bool isActive = true;
    public bool Active {
        get { return isActive; }
        set { isActive = value; }
    }

    bool isUsable = false;

    Subscription<TryInteractEvent> interactSub;

    // Start is called before the first frame update
    void Start()
    {
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

    public void _Interact(TryInteractEvent e)
    {
        if (isUsable)
        {
            
            SceneManager.LoadScene(otherScene);
        }
    }
}
