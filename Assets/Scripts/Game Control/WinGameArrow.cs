using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinGameArrow : MonoBehaviour
{
    [SerializeField] Transform triggerTransform;

    [SerializeField] GameObject IndicatorPrefab;
    [SerializeField] Vector2 canvasSize;

    GameObject arrow;

    float xModifier = 1f;
    float yModifier = 1f;


    // Start is called before the first frame update
    void Start()
    {
        arrow = Instantiate(IndicatorPrefab, transform.localPosition, Quaternion.identity);
        RectTransform indicatorRect = arrow.GetComponent<RectTransform>();
        indicatorRect.pivot = new Vector2(0.5f, 0);
        arrow.transform.localScale = Vector3.one;
        arrow.transform.SetParent(transform, false);
        arrow.GetComponentInChildren<Image>().color = new Color32(255, 247, 69, 255);


        xModifier = canvasSize.x / Screen.width;
        yModifier = canvasSize.y / Screen.height * Mathf.Sqrt(2);
    }

    private void LateUpdate()
    {
        // Implement arrow update towards doors
        Vector3 screenpos = Camera.main.WorldToScreenPoint(triggerTransform.position);

        // Behind us, invert coords
        if (screenpos.z < 0)
        {
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
            || screenpos.y < -screenBounds.y || screenpos.y > screenBounds.y)
        {
            // up
            if (cos > 0)
            {
                screenpos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
            }
            // down
            else
            {
                screenpos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);
            }

            // out of bounds on the left
            if (screenpos.x > screenBounds.x)
            {
                screenpos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
            }
            // out of bounds on the right
            else if (screenpos.x < -screenBounds.x)
            {
                screenpos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
            }

        }

        // Translate coordinates back
        screenpos += screenCenter;

        // Add in canvas modifiers
        screenpos.x *= xModifier;
        screenpos.y *= yModifier;
        arrow.transform.localPosition = screenpos;
        arrow.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg + 180);
    }
}
