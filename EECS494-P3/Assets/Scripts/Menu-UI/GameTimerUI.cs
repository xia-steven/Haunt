using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimerUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Color normalColor;
    [SerializeField] Color flashColor;
    [SerializeField] float flashDuration;

    bool flashing = false;

    // Update is called once per frame
    void Update() {
        int remaining = (int)GameControl.NightTimeRemaining;
        if (remaining == -1 && !flashing) {
            if (GameControl.IsNight) {
                text.text = "Night Ending!\nGet to the teleporter!";
                StartCoroutine(Flash());
            }
        }
        else if (GameControl.IsNight && !flashing) {
            text.text = "Sunrise: " + remaining;
        }
    }

    IEnumerator Flash() {
        flashing = true;
        while (true) {
            text.color = normalColor;
            yield return new WaitForSecondsRealtime(flashDuration);

            text.color = flashColor;
            yield return new WaitForSecondsRealtime(flashDuration);
        }
    }
}