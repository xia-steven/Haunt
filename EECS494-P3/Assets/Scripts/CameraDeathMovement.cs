using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(CameraMovement))]
public class CameraDeathMovement : MonoBehaviour
{
    Vector3 center;
    float moveTime = 1.0f;
    CameraMovement moveScript;
    PixelPerfectCamera pixelCam;
    float sizeModifier = 2.0f;
    int initialXRef = 0;
    int initialYRef = 0;

    private void Start()
    {
        center = transform.position;
        moveScript = GetComponent<CameraMovement>();
        pixelCam = Camera.main.GetComponent<PixelPerfectCamera>();
        initialXRef = pixelCam.refResolutionX;
        initialYRef = pixelCam.refResolutionY;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(MoveCameraToCenter());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            moveScript.enabled = true;
            pixelCam.refResolutionX = initialXRef;
            pixelCam.refResolutionY = initialYRef;
        }
    }


    IEnumerator MoveCameraToCenter()
    {
        // disable movement script
        moveScript.enabled = false;

        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / moveTime;

        Vector3 initial = transform.position;

        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / moveTime;

            // Update pixelPerfectCamera
            pixelCam.refResolutionX = (int)(initialXRef * (1.0f + sizeModifier * progress));
            pixelCam.refResolutionY = (int)(initialYRef * (1.0f + sizeModifier * progress));

            transform.position = Vector3.Lerp(initial, center, progress);

            yield return null;
        }
    }

}
