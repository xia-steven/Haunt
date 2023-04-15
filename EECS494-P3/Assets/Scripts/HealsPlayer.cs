using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealsPlayer : MonoBehaviour {
    [SerializeField] private int healAmount = 1;

    private void Start() {
        Invoke(nameof(Destroy), 8);
    }

    private void Destroy() {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            IsPlayer.instance.gameObject.GetComponent<PlayerHasHealth>().AlterHealth(healAmount);
            Destroy(gameObject);
        }
    }
}