using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsMessageTrigger : MonoBehaviour
{
    [SerializeField] int tutorialMessageID = -1;

    bool sent = false;


    private void OnTriggerEnter(Collider other)
    {

        if(!sent && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            EventBus.Publish(new TutorialMessageEvent(tutorialMessageID, GetInstanceID(), false));
            sent = true;
        }
    }

}
