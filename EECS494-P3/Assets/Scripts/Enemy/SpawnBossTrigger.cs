using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBossTrigger : MonoBehaviour
{
    bool triggered = false;
    float moveToChurchTime = 2.0f;
    Vector3 cameraChurchLocation;
    Transform cameraContainer;
    Vector3 explosionLocation;
    GameObject explosion;
    [SerializeField] GameObject finalBoss;
    Vector3 bossSpawnLocation;
    Vector3 cameraGroundPoundLocation;
    float moveWithPoundTime = 0.75f;

    float walkBossTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        cameraChurchLocation = new Vector3(0, 12, -2);
        cameraContainer = Camera.main.transform.parent;

        explosionLocation = new Vector3(0, 2, 13);

        explosion = new GameObject();
        IsExplosive exxp = explosion.AddComponent<IsExplosive>();
        exxp.setExplosiveRadius(4.0f);

        bossSpawnLocation = new Vector3(0, 0.5f, 13);

        cameraGroundPoundLocation = new Vector3(0, 12, -9);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!triggered)
        {
            triggered = true;
            StartCoroutine(SpawnBossCutscene());
        }
    }

    IEnumerator SpawnBossCutscene()
    {
        // Disable player
        EventBus.Publish(new DisablePlayerEvent());
        // Pause time
        TimeManager.SetTimeScale(0);

        // Delay 1 second
        yield return new WaitForSecondsRealtime(1.0f);


        float initial_time = Time.realtimeSinceStartup;
        float progress = (Time.realtimeSinceStartup - initial_time) / moveToChurchTime;

        Vector3 initialPos = cameraContainer.position;

        // Move camera to in front of the church doors
        while (progress < 1.0f)
        {
            progress = (Time.realtimeSinceStartup - initial_time) / moveToChurchTime;

            cameraContainer.position = Vector3.Lerp(initialPos, cameraChurchLocation, progress);

            yield return null;
        }
         
        // Spawn giant explosion
        GameObject churchExplosion = Instantiate(explosion, explosionLocation, Quaternion.identity);
        StartCoroutine(churchExplosion.GetComponent<IsExplosive>().cutsceneExplosion());

        // Swap church sprite out (TODO)



        // Walk boss in
        initial_time = Time.realtimeSinceStartup;
        progress = (Time.realtimeSinceStartup - initial_time) / walkBossTime;

        finalBoss.SetActive(true);

        Vector3 bossInitialPos = finalBoss.transform.position;

        while (progress < 1.0f)
        {
            progress = (Time.realtimeSinceStartup - initial_time) / walkBossTime;

            finalBoss.transform.position = Vector3.Lerp(bossInitialPos, bossSpawnLocation, progress);

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1.0f);

        // Ground pound
        IsBoss bossController = finalBoss.GetComponent<IsBoss>();
        StartCoroutine(bossController.GroundPound(finalBoss.transform.position, true));

        // Wait for ground pound to start
        yield return new WaitForSecondsRealtime(1.0f);

        // Move camera with ground pound wave
        initial_time = Time.realtimeSinceStartup;
        progress = (Time.realtimeSinceStartup - initial_time) / moveWithPoundTime;

        finalBoss.SetActive(true);

        initialPos = cameraContainer.position;

        // Move camera with ground pound wave
        while (progress < 1.0f)
        {
            progress = (Time.realtimeSinceStartup - initial_time) / moveWithPoundTime;

            cameraContainer.position = Vector3.Lerp(initialPos, cameraGroundPoundLocation, progress);

            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        // Reset time before dodge
        TimeManager.ResetTimeScale();

        // Force dodge through ground pound
        EventBus.Publish(new TutorialDodgeStartEvent(Vector3.forward));

        yield return new WaitForSecondsRealtime(0.15f);

        EventBus.Publish(new TutorialDodgeEndEvent());

        // wait 1 more second
        yield return new WaitForSecondsRealtime(1.0f);

        // Set boss active
        bossController.enabled = true;

        // Enable player
        EventBus.Publish(new EnablePlayerEvent());
    }

}
