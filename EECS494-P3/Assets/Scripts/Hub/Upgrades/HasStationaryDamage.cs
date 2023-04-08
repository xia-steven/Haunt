using System.Collections;
using Player;
using UnityEngine;

namespace Hub.Upgrades {
    public class HasStationaryDamage : MonoBehaviour {
        public float holdTime;
        public float dmgMod;

        private Vector3 knownPos;
        private float lastUpdate;

        private bool holding;
        private bool dmgIncreased;

        private void Start() {
            StartCoroutine(PollForHold());
        }

        // Update is called once per frame
        private void Update() {
            switch (holding) {
                case true when !dmgIncreased:
                    PlayerModifiers.damage *= dmgMod;
                    dmgIncreased = true;
                    break;
                case false when dmgIncreased:
                    PlayerModifiers.damage /= dmgMod;
                    dmgIncreased = false;
                    break;
            }
        }

        private IEnumerator PollForHold() {
            lastUpdate = Time.time;
            knownPos = IsPlayer.instance.transform.position;

            while (true) {
                // if player has moved, reset count
                if (knownPos != IsPlayer.instance.transform.position) {
                    lastUpdate = Time.time;
                    knownPos = IsPlayer.instance.transform.position;
                    holding = false;
                }

                // if held for long enough, set holding true
                if (Time.time - lastUpdate > holdTime) holding = true;

                yield return new WaitForSeconds(.1f);
            }
        }
    }
}