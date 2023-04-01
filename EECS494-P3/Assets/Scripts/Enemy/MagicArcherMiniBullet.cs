using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicArcherMiniBullet : MonoBehaviour {
    MagicArcherProjectile parent;

    private void Awake() {
        parent = GetComponentInParent<MagicArcherProjectile>();
    }

    private void OnTriggerEnter(Collider other) {
        parent.OnTriggerEnter(other);
    }
}