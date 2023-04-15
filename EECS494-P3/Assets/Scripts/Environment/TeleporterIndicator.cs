using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleporterIndicator : MonoBehaviour {
    [SerializeField] private GameObject Teleporter;

    [SerializeField] private GameObject IndicatorPrefab;

    private GameObject indicatorArrow;

    [SerializeField] private Vector2 canvasSize;

    private float xModifier = 1f;
    private float yModifier = 1f;

    private Subscription<NightEndEvent> nightEndSub;
    private Subscription<NightBeginEvent> nightBeginSub;
    private bool isNight;

    // Start is called before the first frame update
    private void Start() {
        GameObject newIndicator = Instantiate(IndicatorPrefab, transform.localPosition, Quaternion.identity);
        RectTransform indicatorRect = newIndicator.GetComponent<RectTransform>();
        indicatorRect.pivot = new Vector2(0.5f, 0);
        newIndicator.transform.localScale = Vector3.one;
        newIndicator.transform.SetParent(transform, false);
        newIndicator.SetActive(false);
        indicatorArrow = newIndicator;


        xModifier = canvasSize.x / Screen.width;
        yModifier = canvasSize.y / Screen.height * Mathf.Sqrt(2);

        nightEndSub = EventBus.Subscribe<NightEndEvent>(TurnOn);
        nightBeginSub = EventBus.Subscribe<NightBeginEvent>(TurnOff);
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(nightEndSub);
    }

    private void TurnOn(NightEndEvent e) {
        if (e.valid)
            isNight = false;
    }

    private void TurnOff(NightBeginEvent e) {
        if (e.valid)
            isNight = true;
    }

    private void Update() {
        if (!isNight && Vector3.Distance(Teleporter.transform.position, IsPlayer.instance.transform.position) > 6.5f)
            indicatorArrow.SetActive(true);
        else
            indicatorArrow.SetActive(false);
    }

    // Update is called once per frame
    private void LateUpdate() {
        Vector3 screenpos = Camera.main.WorldToScreenPoint(Teleporter.transform.position);

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
        indicatorArrow.transform.localPosition = screenpos;
        indicatorArrow.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg + 180);
    }
}