using System.Collections;
using TMPro;
using UnityEngine;

namespace Menu_UI {
    public class GameTimerUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color flashColor;
        [SerializeField] private float flashDuration;

        private bool flashing;

        // Update is called once per frame
        private void Update() {
            var remaining = (int)Game_Control.GameControl.NightTimeRemaining;
            switch (remaining) {
                case -1 when !flashing: {
                    switch (Game_Control.GameControl.IsNight) {
                        case true:
                            text.text = "Night Ending!\nGet to the teleporter!";
                            StartCoroutine(Flash());
                            break;
                    }

                    break;
                }
                default: {
                    text.text = Game_Control.GameControl.IsNight switch {
                        true when !flashing => "Sunrise: " + remaining,
                        _ => text.text
                    };

                    break;
                }
            }
        }

        private IEnumerator Flash() {
            flashing = true;
            while (true) {
                text.color = normalColor;
                yield return new WaitForSecondsRealtime(flashDuration);

                text.color = flashColor;
                yield return new WaitForSecondsRealtime(flashDuration);
            }
        }
    }
}