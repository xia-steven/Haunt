using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class PedestalAttacker : EnemyBase {
    private List<Vector3> pedestalPositions;

    private void Start() {
        var pos = transform.position;
        pedestalPositions = new List<Vector3> { new(1, 0, 1), new(2, 0, 2), new(3, 0, 3) };
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Pedestal")) {
            EventBus.Publish(new PlayerDamagedEvent());
        }
    }
}