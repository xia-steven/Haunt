using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttacker : EnemyBase {
    private new void Start() {
        base.Start();
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
}