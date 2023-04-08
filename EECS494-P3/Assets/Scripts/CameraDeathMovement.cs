using System.Collections;
using Events;
using Player;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(CameraMovement))]
public class CameraDeathMovement : MonoBehaviour {
    private Vector3 center;
    private const float moveTime = 1.0f;
    private const float sinkIntoGroundTime = 3.0f;
    private const float fallOntoGroundTime = 3.0f;
    private CameraMovement moveScript;
    private PixelPerfectCamera pixelCam;
    private const float sizeModifier = 2.0f;
    private int initialXRef;
    private int initialYRef;

    private Subscription<GameLossEvent> gameLossSub;

    private void Start() {
        center = transform.position;
        moveScript = GetComponent<CameraMovement>();
        if (Camera.main != null) pixelCam = Camera.main.GetComponent<PixelPerfectCamera>();
        initialXRef = pixelCam.refResolutionX;
        initialYRef = pixelCam.refResolutionY;

        gameLossSub = EventBus.Subscribe<GameLossEvent>(OnGameLost);
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(gameLossSub);
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            EventBus.Publish(new GameLossEvent(IsPlayer.instance.LastDamaged()));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) {
            moveScript.enabled = true;
            pixelCam.refResolutionX = initialXRef;
            pixelCam.refResolutionY = initialYRef;
        }
    }


    private IEnumerator MoveCameraToCenter(GameLossEvent gle) {
        // disable movement script
        moveScript.enabled = false;

        // Disable the player
        EventBus.Publish(new DisablePlayerEvent());
        //TimeManager.SetTimeScale(0);

        // Get the player
        var player = IsPlayer.instance.gameObject;

        var initial_time = Time.time;
        var progress = (Time.time - initial_time) / moveTime;

        var initial = transform.position;

        switch (gle.cause) {
            case DeathCauses.Pedestal: {
                // Move the camera to the center
                while (progress < 1.0f) {
                    progress = (Time.time - initial_time) / moveTime;

                    // Update pixelPerfectCamera
                    pixelCam.refResolutionX = (int)(initialXRef * (1.0f + sizeModifier * progress));
                    pixelCam.refResolutionY = (int)(initialYRef * (1.0f + sizeModifier * progress));

                    transform.position = Vector3.Lerp(initial, center, progress);

                    yield return null;
                }

                initial_time = Time.time;
                progress = (Time.time - initial_time) / sinkIntoGroundTime;

                var playerPos = player.transform.position;

                // Fade into the ground
                while (progress < 1.0f) {
                    progress = (Time.time - initial_time) / sinkIntoGroundTime;


                    player.transform.position = new Vector3(playerPos.x, playerPos.y - progress * 2f, playerPos.z);

                    yield return null;
                }

                break;
            }
            //if (gle.cause == DeathCauses.Enemy)
            default: {
                initial_time = Time.time;
                progress = (Time.time - initial_time) / fallOntoGroundTime;

                var playerSprite = player.GetComponentInChildren<SpriteRenderer>();

                var spriteRot = playerSprite.transform.rotation;
                var xScale = playerSprite.transform.localScale.x;
                var yScale = playerSprite.transform.localScale.y;
                Vector3 allScale;

                // Fall on the ground
                while (progress < 1.0f) {
                    progress = (Time.time - initial_time) / fallOntoGroundTime;

                    allScale = new Vector3(xScale + (yScale - xScale) * progress, yScale + (xScale - yScale) * progress,
                        playerSprite.transform.localScale.z);

                    playerSprite.transform.localScale = allScale;
                    playerSprite.transform.rotation = spriteRot * Quaternion.Euler(0, 0, 90f * progress);

                    yield return null;
                }

                break;
            }
        }
        //else
        //{
        // Died to pit???
        //}


        // Enable the player?
        //EventBus.Publish(new EnablePlayerEvent());
        //TimeManager.ResetTimeScale();

        // Note that we finished the death animation
        gle.finishedDeathAnimation = true;
    }

    private void OnGameLost(GameLossEvent gle) {
        StartCoroutine(MoveCameraToCenter(gle));
    }
}