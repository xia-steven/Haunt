using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealsPlayer : MonoBehaviour {
    [SerializeField] private int healAmount = 1;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            //alter before publishing in case a subscriber references the new health
            //only publish event if health increase was successful
            if (other.gameObject.GetComponent<HasHealth>().AlterHealth(healAmount))
                EventBus.Publish(new HealEvent(healAmount));
        }
    }
}

public class HealEvent {
    public int heal_amt = 1;

    public HealEvent(int _heal_amt) {
        heal_amt = _heal_amt;
    }
}