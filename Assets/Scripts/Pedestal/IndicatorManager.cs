using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorManager : MonoBehaviour {
    [SerializeField] List<GameObject> Pedestals;

    [SerializeField] GameObject IndicatorPrefab;

    List<GameObject> indicatorArrows;

    [SerializeField] Vector2 canvasSize;

    float xModifier = 1f;
    float yModifier = 1f;

    Subscription<PedestalPartialEvent> healSub;

    // Start is called before the first frame update
    void Start() {
        indicatorArrows = new List<GameObject>();

        for (int i = 0; i < Pedestals.Count; ++i) {
            GameObject newIndicator = Instantiate(IndicatorPrefab, transform.localPosition, Quaternion.identity);
            RectTransform indicatorRect = newIndicator.GetComponent<RectTransform>();
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

    void _onPedestalPartial(PedestalPartialEvent phe) {
        indicatorArrows[phe.pedestalUUID - 1].SetActive(phe.turnOn);
    }


    // Update is called once per frame
    void LateUpdate() {
        // Code adapted from https://www.youtube.com/watch?v=gAQpR1GN0Os
        for (int a = 0; a < Pedestals.Count; ++a) {
            Vector3 screenpos = Camera.main.WorldToScreenPoint(Pedestals[a].transform.position);

            // Behind us, invert coords
            if (screenpos.z < 0) {
                screenpos *= -1;
            }

            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

            // Translate coordinates to make 00 the center of the screen
            screenpos -= screenCenter;

            // angle from player location to pedestal location
            float angle = Mathf.Atan2(screenpos.y, screenpos.x);
            angle -= 90 * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = -Mathf.Sin(angle);

            float m = cos / sin;

            // Currently the bounds go to the edge of the screen
            Vector3 screenBounds = screenCenter;

            // Check if the pedestal is off the screen
            if (screenpos.x < -screenBounds.x || screenpos.x > screenBounds.x
                                              || screenpos.y < -screenBounds.y || screenpos.y > screenBounds.y) {
                // up
                if (cos > 0) {
                    screenpos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
                }
                // down
                else {
                    screenpos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);
                }

                // out of bounds on the left
                if (screenpos.x > screenBounds.x) {
                    screenpos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
                }
                // out of bounds on the right
                else if (screenpos.x < -screenBounds.x) {
                    screenpos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
                }
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