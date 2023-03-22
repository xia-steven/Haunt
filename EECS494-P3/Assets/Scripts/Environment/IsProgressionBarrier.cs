using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsProgressionBarrier : MonoBehaviour
{
    [SerializeField] int triggerID;

    Subscription<TutorialTriggerEvent> tutorSub;

    // Start is called before the first frame update
    void Start()
    {
        tutorSub = EventBus.Subscribe<TutorialTriggerEvent>(_OnTutorialTrigger);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(tutorSub);
    }

    void _OnTutorialTrigger(TutorialTriggerEvent tte)
    {
        if(tte.UUID == triggerID)
        {
            Destroy(this.gameObject);
        }
    }

}
