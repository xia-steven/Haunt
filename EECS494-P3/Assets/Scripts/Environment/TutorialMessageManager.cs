using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessageManager : MonoBehaviour
{
    [SerializeField] string configPath;

    Subscription<TutorialMessageEvent> tutorMesSub;

    // Start is called before the first frame update
    void Start()
    {
        tutorMesSub = EventBus.Subscribe<TutorialMessageEvent>(onTutorialMessageSent);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(tutorMesSub);
    }

    void onTutorialMessageSent(TutorialMessageEvent tme)
    {
        
        // Send message event
    }
}
