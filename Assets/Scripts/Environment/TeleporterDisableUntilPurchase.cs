using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IsTeleporter))]
public class TeleporterDisableUntilPurchase : MonoBehaviour {
    IsTeleporter tp;
    bool activated = false;

    Subscription<ActivateTeleporterEvent> activateSub;

    private void Start() {
        tp = GetComponent<IsTeleporter>();

 
        tp.Active = false;
        activated = false;

        if (GameControl.Day == 3)
        {
            StartCoroutine(activateOnDelay(5f));
        }

        activateSub = EventBus.Subscribe<ActivateTeleporterEvent>(onTeleporterActivate);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(activateSub);
    }

    // Update is called once per frame
    private void onTeleporterActivate(ActivateTeleporterEvent ate)
    {
        activated = true;
        tp.Active = true;
    }   


    private IEnumerator activateOnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        activated = true;
        tp.Active = true;
    }
}