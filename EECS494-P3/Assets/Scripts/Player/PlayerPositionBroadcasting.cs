using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionBroadcasting : MonoBehaviour {
    private void Start() {
        InvokeRepeating(nameof(PublishPosition), 0, 1f);
    }

    private void PublishPosition() {
        EventBus.Publish(new PlayerPositionEvent(transform.position));
    }
}