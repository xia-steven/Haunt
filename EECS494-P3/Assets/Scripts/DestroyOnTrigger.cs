using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTrigger : MonoBehaviour {
    [SerializeField] bool playerExclusive = false;

    private void OnTriggerEnter(Collider other) {
        if (playerExclusive) {
            if (other.gameObject.CompareTag("Player")) {
                Destroy(gameObject);
            }
        }
        else {
            Destroy(gameObject);
        }
    }
}