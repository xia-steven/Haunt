using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealsPlayer : MonoBehaviour {
    [SerializeField] private int healAmount = 1;

    private void Start() {
        StartCoroutine(WaitAndDestroy());
    }

    private IEnumerator WaitAndDestroy() {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            IsPlayer.instance.gameObject.GetComponent<PlayerHasHealth>().AlterHealth(healAmount);
            Destroy(gameObject);
        }
    }
}