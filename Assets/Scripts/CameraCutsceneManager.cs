using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

[RequireComponent(typeof(CameraMovement))]
public class CameraCutsceneManager : MonoBehaviour
{
    [SerializeField] Image pedestalFilter;
    [SerializeField] List<GameObject> pedestals;
    float filterFadeTime = 1.0f;

    Vector3 center;
    float moveTime = 1.0f;
    float sinkIntoGroundTime = 3.0f;
    float pedestalBrightenTime = 2.0f;
    float pedestalBrightenAmount = 30.0f;
    float expandHoleTime = 2.0f;
    float fallOntoGroundTime = 3.0f;
    CameraMovement moveScript;
    PixelPerfectCamera pixelCam;
    float deathZoomSizeModifier = 0.75f;
    float teleporterZoomSizeModifier = -0.75f;
    float pedestalZoomSizeModifier = -0.75f;
    int initialXRef = 0;
    int initialYRef = 0;

    List<Light> pedestalLights;

    Subscription<GameLossEvent> gameLossSub;
    Subscription<NightEndEvent> nightEndSub;
    Subscription<PedestalRepairedEvent> pedRepairSub;

    IsTeleporter teleporter;

    Sprite abyssSprite;

    bool lostPedestalYet = false;

    private void Start()
    {
        center = transform.position;
        moveScript = GetComponent<CameraMovement>();
        pixelCam = Camera.main.GetComponent<PixelPerfectCamera>();
        initialXRef = pixelCam.refResolutionX;
        initialYRef = pixelCam.refResolutionY;

        // Get teleporter from only teleporter in scene
        teleporter = GameObject.FindGameObjectWithTag("Teleporter").GetComponent<IsTeleporter>();

        // Set pedestal filter disabled
        pedestalFilter.gameObject.SetActive(false);

        gameLossSub = EventBus.Subscribe<GameLossEvent>(OnGameLost);
        nightEndSub = EventBus.Subscribe<NightEndEvent>(OnNightEnd);
        pedRepairSub = EventBus.Subscribe<PedestalRepairedEvent>(OnPedestalRepair);

        pedestalLights = new List<Light>();

        abyssSprite = Resources.Load<Sprite>("Textures-Sprites/abyss_shadow");

        // Get pedestal lights
        for (int a = 0; a < pedestals.Count; ++a)
        {
            pedestalLights.Add(pedestals[a].GetComponentInChildren<Light>());
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(gameLossSub);
        EventBus.Unsubscribe(nightEndSub);
        EventBus.Unsubscribe(pedRepairSub);
    }


    void OnGameLost(GameLossEvent gle)
    {
        StartCoroutine(PlayerDeathCutscene(gle));
    }

    void OnNightEnd(NightEndEvent nee)
    {
        // Only play teleporter cutscene on the tutorial night
        if (SceneManager.GetActiveScene().name == "TutorialGameScene")
        {
            StartCoroutine(ZoomToLocationCutscene(true, center, teleporterZoomSizeModifier));
        }
    }

    void OnPedestalRepair(PedestalRepairedEvent pre)
    {
        if (!lostPedestalYet && SceneManager.GetActiveScene().name == "TutorialGameScene")
        {
            lostPedestalYet = true;
            StartCoroutine(ZoomToLocationCutscene(false, pre.pedestalLocation + center + new Vector3(0, 0, 1f), pedestalZoomSizeModifier));
        }
    }

    IEnumerator PlayerDeathCutscene(GameLossEvent gle)
    {
        // disable movement script
        moveScript.enabled = false;

        // Disable the player
        EventBus.Publish(new DisablePlayerEvent());
        TimeManager.SetTimeScale(0);

        // Get the player 
        GameObject player = IsPlayer.instance.gameObject;

        float initial_time = Time.realtimeSinceStartup;
        float progress = (Time.realtimeSinceStartup - initial_time) / moveTime;

        Vector3 initial = transform.position;

        if(gle.cause == DeathCauses.Pedestal)
        {
            // Move the camera to the center
            while (progress < 1.0f)
            {
                progress = (Time.realtimeSinceStartup - initial_time) / moveTime;

                // Update pixelPerfectCamera
                pixelCam.refResolutionX = (int)(initialXRef * (1.0f + deathZoomSizeModifier * progress));
                pixelCam.refResolutionY = (int)(initialYRef * (1.0f + deathZoomSizeModifier * progress));

                transform.position = Vector3.Lerp(initial, center, progress);

                yield return null;
            }

            // Brighten pedestals

            initial_time = Time.realtimeSinceStartup;
            progress = (Time.realtimeSinceStartup - initial_time) / pedestalBrightenTime;

            while (progress < 1.0f)
            {
                progress = (Time.realtimeSinceStartup - initial_time) / pedestalBrightenTime;

                for(int a = 0; a < pedestalLights.Count; ++a )
                {
                    pedestalLights[a].intensity = 5f + progress * pedestalBrightenAmount;
                }

                yield return null;
            }

            // Expand dark hole under player (shadow)

            initial_time = Time.realtimeSinceStartup;
            progress = (Time.realtimeSinceStartup - initial_time) / expandHoleTime;

            GameObject playerShadow = IsPlayer.instance.transform.GetChild(1).gameObject;
            playerShadow.transform.parent = this.transform;
            SpriteRenderer playerSpriteRenderer = playerShadow.GetComponent<SpriteRenderer>();
            playerSpriteRenderer.sprite = abyssSprite;
            

            while (progress < 1.0f)
            {
                progress = (Time.realtimeSinceStartup - initial_time) / expandHoleTime;


                playerShadow.transform.localScale = new Vector3(1.0f + progress * 10.0f, 1.0f + progress * 10.0f, 1.0f + progress * 10.0f);

                yield return null;
            }

            playerSpriteRenderer.sortingOrder = 1000;


            initial_time = Time.realtimeSinceStartup;
            progress = (Time.realtimeSinceStartup - initial_time) / sinkIntoGroundTime;

            Vector3 playerPos = player.transform.position;

            // Fade into the ground
            while(progress < 1.0f)
            {
                progress = (Time.realtimeSinceStartup - initial_time) / sinkIntoGroundTime;


                player.transform.position = new Vector3(playerPos.x, playerPos.y - progress * 2f, playerPos.z);

                yield return null;
            }
        }
        else //if (gle.cause == DeathCauses.Enemy)
        {
            initial_time = Time.realtimeSinceStartup;
            progress = (Time.realtimeSinceStartup - initial_time) / fallOntoGroundTime;

            GameObject playerSprite = IsPlayer.instance.transform.GetChild(0).gameObject;

            Quaternion spriteRot = playerSprite.transform.rotation;
            float xScale = playerSprite.transform.localScale.x;
            float yScale = playerSprite.transform.localScale.y;
            Vector3 allScale;

            // Fall on the ground
            while (progress < 1.0f)
            {
                progress = (Time.realtimeSinceStartup - initial_time) / fallOntoGroundTime;

                allScale = new Vector3(xScale + (yScale - xScale) * progress, yScale + (xScale - yScale) * progress,
                    playerSprite.transform.localScale.z);

                playerSprite.transform.localScale = allScale;
                playerSprite.transform.rotation = spriteRot * Quaternion.Euler(0, 0, 90f * progress);

                yield return null;
            }
        }
        // Note that we finished the death animation
        gle.finishedDeathAnimation = true;

    }

    IEnumerator ZoomToLocationCutscene(bool isTeleporterCutscene, Vector3 zoomLocation, float zoomModifier)
    {
        // disable movement script
        moveScript.enabled = false;

        // Disable the player
        EventBus.Publish(new DisablePlayerEvent());
        EventBus.Publish(new ToggleInvincibilityEvent(true));
        TimeManager.SetTimeScale(0);

        float initial_time = Time.realtimeSinceStartup;
        float progress = (Time.realtimeSinceStartup - initial_time) / moveTime;

        Vector3 initial = transform.position;

        // Move the camera to the zoom location
        while (progress < 1.0f)
        {
            progress = (Time.realtimeSinceStartup - initial_time) / moveTime;

            // Update pixelPerfectCamera
            pixelCam.refResolutionX = (int)(initialXRef * (1.0f + zoomModifier * progress));
            pixelCam.refResolutionY = (int)(initialYRef * (1.0f + zoomModifier * progress));

            transform.position = Vector3.Lerp(initial, zoomLocation, progress);

            yield return null;
        }

        // Wait a few seconds
        if(isTeleporterCutscene)
        {
            // Show teleporter waking up
            teleporter.Active = true;
            yield return new WaitForSecondsRealtime(3.0f);
        }
        else
        {
            // Highlight hearts and pedestal
            pedestalFilter.color = new Color(0, 0, 0, 0);
            pedestalFilter.gameObject.SetActive(true);

            // Fade in highlight
            initial_time = Time.realtimeSinceStartup;
            progress = (Time.realtimeSinceStartup - initial_time) / filterFadeTime;

            while (progress < 1.0f)
            {
                progress = (Time.realtimeSinceStartup - initial_time) / filterFadeTime;

                pedestalFilter.color = new Color(0, 0, 0, progress / 2f);

                yield return null;
            }

            yield return new WaitForSecondsRealtime(1.5f);

            // Fade out highlight
            initial_time = Time.realtimeSinceStartup;
            progress = (Time.realtimeSinceStartup - initial_time) / filterFadeTime;

            while (progress < 1.0f)
            {
                progress = (Time.realtimeSinceStartup - initial_time) / filterFadeTime;

                pedestalFilter.color = new Color(0, 0, 0, (1 - progress) / 2f);

                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.5f);

            pedestalFilter.gameObject.SetActive(false);
        }


        initial_time = Time.realtimeSinceStartup;
        progress = (Time.realtimeSinceStartup - initial_time) / moveTime;

        // Move the camera back to the player
        while (progress < 1.0f)
        {
            progress = (Time.realtimeSinceStartup - initial_time) / moveTime;

            // Update pixelPerfectCamera
            pixelCam.refResolutionX = (int)(initialXRef * (1.0f + zoomModifier * (1 -progress)));
            pixelCam.refResolutionY = (int)(initialYRef * (1.0f + zoomModifier * (1 - progress)));

            transform.position = Vector3.Lerp(initial, zoomLocation, (1 - progress));

            yield return null;
        }

        // Reenable player
        EventBus.Publish(new EnablePlayerEvent());
        EventBus.Publish(new ToggleInvincibilityEvent(false));

        // Reenable movement script
        moveScript.enabled = true;

        // Reenable time
        TimeManager.ResetTimeScale();
    }
}
