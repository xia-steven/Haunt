using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    [SerializeField] float explosionTime;
    private float droppedTime;

    private void Awake() {
        droppedTime = Time.time;
    }

    void Update() {
        // Destroy (explode) bomb after time has passed
        float passedTime = Time.time - droppedTime;
        if (passedTime >= explosionTime) {
            Destroy(gameObject);
        }
    }
}