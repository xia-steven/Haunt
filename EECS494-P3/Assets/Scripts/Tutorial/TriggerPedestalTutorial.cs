using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPedestalTutorial : MonoBehaviour
{
    [SerializeField] int tutorialMessageID = 3;

    private void OnDestroy()
    {
        // Send the tutorial message
        EventBus.Publish(new TutorialMessageEvent(tutorialMessageID, GetInstanceID()));
    }
}
