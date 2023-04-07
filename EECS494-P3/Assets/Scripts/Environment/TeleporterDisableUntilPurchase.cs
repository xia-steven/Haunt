using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IsTeleporter))]
public class TeleporterDisableUntilPurchase : MonoBehaviour {
    IsTeleporter tp;
    bool activated = false;

    private void Start() {
        tp = GetComponent<IsTeleporter>();

        tp.Active = false;
        activated = false;
    }

    // Update is called once per frame
    void Update() {
        if (!activated && GameControl.NightEnding) {
            activated = true;
            tp.Active = true;
        }
        else if (!GameControl.NightEnding) {
            tp.Active = false;
            activated = false;
        }
    }
}