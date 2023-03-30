using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealsPlayer : MonoBehaviour {
    [SerializeField] private int healAmount = 1;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerPhysical")) {
            IsPlayer.instance.gameObject.GetComponent<PlayerHasHealth>().AlterHealth(healAmount);
            Destroy(gameObject);
        }
    }
}
