using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPathfinding : MonoBehaviour {
    private Pathfinding pathfinding;

    private void Start() {
        pathfinding = new Pathfinding(35, 21);
        InvokeRepeating(nameof(PublishPosition), 0, 1f);
    }

    private void PublishPosition() {
        EventBus.Publish(new PlayerPositionEvent(transform.position));
    }
}