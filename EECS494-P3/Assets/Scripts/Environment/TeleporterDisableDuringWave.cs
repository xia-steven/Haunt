using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(IsTeleporter))]
public class TeleporterDisableDuringWave : MonoBehaviour {
    IsTeleporter tp;
    bool activated = false;

    private void Start() {
        tp = GetComponent<IsTeleporter>();

        if(!GameControl.NightEnding)
        {
            tp.Active = false;
            activated = false;
        }
    }

    // Update is called once per frame
    void Update() {
        if (!activated && GameControl.NightEnding && SceneManager.GetActiveScene().name != "TutorialGameScene") {
            activated = true;
            tp.Active = true;
        }
        else if (!GameControl.NightEnding && SceneManager.GetActiveScene().name != "TutorialGameScene") {
            tp.Active = false;
            activated = false;
        }
    }
}