using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(CameraMovement))]
public class CameraDeathMovement : MonoBehaviour
{
    Vector3 center;
    float moveTime = 1.0f;
    float sinkIntoGroundTime = 3.0f;
    float fallOntoGroundTime = 3.0f;
    CameraMovement moveScript;
    PixelPerfectCamera pixelCam;
    float sizeModifier = 2.0f;
    int initialXRef = 0;
    int initialYRef = 0;

    Subscription<GameLossEvent> gameLossSub;

    private void Start()
    {
        center = transform.position;
        moveScript = GetComponent<CameraMovement>();
        pixelCam = Camera.main.GetComponent<PixelPerfectCamera>();
        initialXRef = pixelCam.refResolutionX;
        initialYRef = pixelCam.refResolutionY;

        gameLossSub = EventBus.Subscribe<GameLossEvent>(OnGameLost);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(gameLossSub);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            EventBus.Publish(new GameLossEvent(IsPlayer.instance.LastDamaged()));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            moveScript.enabled = true;
            pixelCam.refResolutionX = initialXRef;
            pixelCam.refResolutionY = initialYRef;
        }
    }


    IEnumerator MoveCameraToCenter(GameLossEvent gle)
    {
        // disable movement script
        moveScript.enabled = false;

        // Disable the player
        EventBus.Publish(new DisablePlayerEvent());

        // Get the player 
        GameObject player = IsPlayer.instance.gameObject;

        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / moveTime;

        Vector3 initial = transform.position;

        if(gle.cause == DeathCauses.Pedestal)
        {
            // Move the camera to the center
            while (progress < 1.0f)
            {
                progress = (Time.time - initial_time) / moveTime;

                // Update pixelPerfectCamera
                pixelCam.refResolutionX = (int)(initialXRef * (1.0f + sizeModifier * progress));
                pixelCam.refResolutionY = (int)(initialYRef * (1.0f + sizeModifier * progress));

                transform.position = Vector3.Lerp(initial, center, progress);

                yield return null;
            }

            initial_time = Time.time;
            progress = (Time.time - initial_time) / sinkIntoGroundTime;

            Vector3 playerPos = player.transform.position;

            // Fade into the ground
            while(progress < 1.0f)
            {
                progress = (Time.time - initial_time) / sinkIntoGroundTime;


                player.transform.position = new Vector3(playerPos.x, playerPos.y - progress * 2f, playerPos.z);

                yield return null;
            }
        }
        else if (gle.cause == DeathCauses.Enemy)
        {
            initial_time = Time.time;
            progress = (Time.time - initial_time) / fallOntoGroundTime;

            SpriteRenderer playerSprite = player.GetComponentInChildren<SpriteRenderer>();
            
            Quaternion spriteRot = playerSprite.transform.rotation;
            float xScale = playerSprite.transform.localScale.x;
            float yScale = playerSprite.transform.localScale.y;
            Vector3 allScale;

            // Fall on the ground
            while (progress < 1.0f)
            {
                progress = (Time.time - initial_time) / fallOntoGroundTime;

                allScale = new Vector3(xScale + (yScale - xScale) * progress, yScale + (xScale - yScale) * progress,
                    playerSprite.transform.localScale.z);

                playerSprite.transform.localScale = allScale;
                playerSprite.transform.rotation = spriteRot * Quaternion.Euler(0, 0, 90f * progress);

                yield return null;
            }
        }
        else
        {
            // Died to pit???
        }


        // Enable the player?
        //EventBus.Publish(new EnablePlayerEvent());

        // Note that we finished the death animation
        gle.finishedDeathAnimation = true;

    }

    void OnGameLost(GameLossEvent gle)
    {
        StartCoroutine(MoveCameraToCenter(gle));
    }

}
