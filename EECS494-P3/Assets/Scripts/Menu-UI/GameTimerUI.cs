using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimerUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text;

    // Update is called once per frame
    void Update() {
        text.text = "Remaining: " + (int)GameControl.NightTimeRemaining;
    }
}