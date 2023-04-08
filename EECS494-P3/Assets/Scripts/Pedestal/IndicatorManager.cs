using System.Collections.Generic;
using Events;
using UnityEngine;

namespace Pedestal {
    public class IndicatorManager : MonoBehaviour {
        [SerializeField] private List<GameObject> Pedestals;

        [SerializeField] private GameObject IndicatorPrefab;

        private List<GameObject> indicatorArrows;

        [SerializeField] private Vector2 canvasSize;

        private float xModifier = 1f;
        private float yModifier = 1f;

        private Subscription<PedestalPartialEvent> healSub;

        // Start is called before the first frame update
        private void Start() {
            indicatorArrows = new List<GameObject>();

            for (var i = 0; i < Pedestals.Count; ++i) {
                var newIndicator = Instantiate(IndicatorPrefab, transform.localPosition, Quaternion.identity);
                var indicatorRect = newIndicator.GetComponent<RectTransform>();
                indicatorRect.pivot = new Vector2(0.5f, 0);
                newIndicator.transform.localScale = Vector3.one;
                newIndicator.transform.SetParent(transform, false);
                newIndicator.SetActive(false);
                indicatorArrows.Add(newIndicator);
            }

            xModifier = canvasSize.x / Screen.width;
            yModifier = canvasSize.y / Screen.height * Mathf.Sqrt(2);

            healSub = EventBus.Subscribe<PedestalPartialEvent>(_onPedestalPartial);
        }

        private void OnDestroy() {
            EventBus.Unsubscribe(healSub);
        }

        private void _onPedestalPartial(PedestalPartialEvent phe) {
            indicatorArrows[phe.pedestalUUID - 1].SetActive(phe.turnOn);
        }


        // Update is called once per frame
        private void LateUpdate() {
            // Code adapted from https://www.youtube.com/watch?v=gAQpR1GN0Os
            for (var a = 0; a < Pedestals.Count; ++a) {
                if (Camera.main == null) continue;
                var screenpos = Camera.main.WorldToScreenPoint(Pedestals[a].transform.position);

                switch (screenpos.z) {
                    // Behind us, invert coords
                    case < 0:
                        screenpos *= -1;
                        break;
                }

                var screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

                // Translate coordinates to make 00 the center of the screen
                screenpos -= screenCenter;

                // angle from player location to pedestal location
                var angle = Mathf.Atan2(screenpos.y, screenpos.x);
                angle -= 90 * Mathf.Deg2Rad;

                var cos = Mathf.Cos(angle);
                var sin = -Mathf.Sin(angle);

                var m = cos / sin;

                // Currently the bounds go to the edge of the screen
                var screenBounds = screenCenter;

                // Check if the pedestal is off the screen
                if (screenpos.x < -screenBounds.x || screenpos.x > screenBounds.x
                                                  || screenpos.y < -screenBounds.y || screenpos.y > screenBounds.y) {
                    screenpos = cos switch {
                        // up
                        > 0 => new Vector3(screenBounds.y / m, screenBounds.y, 0),
                        _ => new Vector3(-screenBounds.y / m, -screenBounds.y, 0)
                    };

                    // out of bounds on the left
                    if (screenpos.x > screenBounds.x)
                        screenpos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
                    // out of bounds on the right
                    else if (screenpos.x < -screenBounds.x) screenpos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
                }

                // Translate coordinates back
                screenpos += screenCenter;

                // Add in canvas modifiers
                screenpos.x *= xModifier;
                screenpos.y *= yModifier;
                indicatorArrows[a].transform.localPosition = screenpos;
                indicatorArrows[a].transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg + 180);
            }
        }
    }
}