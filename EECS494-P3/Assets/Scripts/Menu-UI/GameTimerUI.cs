using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimerUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text;

    // Update is called once per frame
    void Update() {
        int remaining = (int)GameControl.NightTimeRemaining;
        if (remaining == -1)
        {
            if (GameControl.IsNight)
                text.text = "Night Ending!\nGet to the teleporter!";
            else
                text.text = "Use the teleporter to\nstart the next night";
        }
            
        else
            text.text = "Remaining: " + remaining;
    }
}