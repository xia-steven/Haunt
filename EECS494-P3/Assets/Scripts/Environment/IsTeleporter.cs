using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class IsTeleporter : MonoBehaviour
{
    [SerializeField] string otherScene;

    [SerializeField] Material inactiveMat;
    [SerializeField] Material activeMat;

    Renderer visualRenderer;

    bool isActive = true;
    public bool Active {
        get { return isActive; }
        set 
        { 
            isActive = value;
            if (isActive) Activate();
            else Deactivate();
        }
    }

    bool isUsable = false;

    Subscription<TryInteractEvent> interactSub;

    // Start is called before the first frame update
    void Start()
    {
        visualRenderer = GetComponent<Renderer>();

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
        visualRenderer.material = activeMat;

        //activate other visuals here
    }

    private void Deactivate()
    {
        visualRenderer.material = inactiveMat;

        //deactivate other visuals here
    }

    public void _Interact(TryInteractEvent e)
    {
        if (isUsable)
        {
            IsPlayer.SetPosition(new Vector3(0, .25f, 0));
            SceneManager.LoadScene(otherScene);
        }
    }
}
