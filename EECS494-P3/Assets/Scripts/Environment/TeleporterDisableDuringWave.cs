using UnityEngine;

namespace Environment {
    [RequireComponent(typeof(IsTeleporter))]
    public class TeleporterDisableDuringWave : MonoBehaviour {
        private IsTeleporter tp;
        private bool activated;

        private void Start() {
            tp = GetComponent<IsTeleporter>();

            switch (Game_Control.GameControl.NightEnding) {
                case false:
                    tp.Active = false;
                    activated = false;
                    break;
            }
        }

        // Update is called once per frame
        private void Update() {
            switch (activated) {
                case false when Game_Control.GameControl.NightEnding:
                    activated = true;
                    tp.Active = true;
                    break;
                default: {
                    switch (Game_Control.GameControl.NightEnding) {
                        case false:
                            tp.Active = false;
                            activated = false;
                            break;
                    }

                    break;
                }
            }
        }
    }
}