using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttacker : EnemyBase {
    private Subscription<PlayerPositionEvent> positionSub;

    private new void Start() {
        base.Start();
        positionSub = EventBus.Subscribe<PlayerPositionEvent>(SetTargetPosition);
    }

    private new void OnTriggerEnter(Collider other) {
        Debug.Log("Collided");
        if (other.CompareTag("Player")) {
            EventBus.Publish(new PlayerDamagedEvent());
        }
        else {
            base.OnTriggerEnter(other);
        }
    }

    private void SetTargetPosition(PlayerPositionEvent event_) {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), event_.position);

        if (pathVectorList != null && pathVectorList.Count > 1) {
            pathVectorList.RemoveAt(0);
        }
    }
}