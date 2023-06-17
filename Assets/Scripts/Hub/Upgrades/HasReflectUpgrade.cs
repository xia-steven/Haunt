using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasReflectUpgrade : MonoBehaviour {
    private GameObject dashShield;
    private GameObject appliedShield;
    private Subscription<PlayerDodgeEvent> dodgeEvent;

    // Start is called before the first frame update
    void Start() {
        dashShield = Resources.Load<GameObject>("Prefabs/DashShield");
        dodgeEvent = EventBus.Subscribe<PlayerDodgeEvent>(_OnDodge);
    }

    // Attach shield on dodge start and destroy it on dodge finish
    private void _OnDodge(PlayerDodgeEvent e) {
        if (e.start) {
            appliedShield = Instantiate(dashShield, transform);
        }
        else {
            Destroy(appliedShield);
        }
    }

    protected void OnDestroy() {
        EventBus.Unsubscribe(dodgeEvent);
    }
}