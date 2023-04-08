using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IsTeleporter))]
public class TeleporterDisableDuringWave : MonoBehaviour {
    IsTeleporter tp;
    bool activated;

    private void Start() {
        tp = GetComponent<IsTeleporter>();

        if (!GameControl.NightEnding) {
            tp.Active = false;
            activated = false;
        }
    }

    // Update is called once per frame
    private void Update() {
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