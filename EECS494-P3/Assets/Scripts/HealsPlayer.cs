using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealsPlayer : MonoBehaviour {
    [SerializeField] private int healAmount = 1;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponent<PlayerHasHealth>().AlterHealth(healAmount);
        }
    }
}

public class HealEvent {
    public int heal_amt = 1;

    public HealEvent(int _heal_amt) {
        heal_amt = _heal_amt;
    }
}