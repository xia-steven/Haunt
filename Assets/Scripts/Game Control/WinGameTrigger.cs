using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinGameTrigger : MonoBehaviour
{
    Vector3 frontOfDoorsLocation;
    Vector3 insideChurchLocation;

    [SerializeField] GameObject WinArrow;

    bool triggered = false;

    // Start is called before the first frame update
    void Start()
    {
        frontOfDoorsLocation = transform.position + new Vector3(0, 0, -1);
        insideChurchLocation = transform.position + new Vector3(0, 0, 3);

        WinArrow.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {
        // If the player triggered the end
        if(other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            StartCoroutine(WinGameCoroutine());
        }
    }

    IEnumerator WinGameCoroutine()
    {
        DisablePlayerEvent eve = new DisablePlayerEvent();
        eve.keepAnimatorEnabled = true;

        // Disable player (still invincible from boss death)
        EventBus.Publish(eve);

        // Disable camera movement
        EventBus.Publish(new TutorialLockCameraEvent(Camera.main.transform.position));

        // Disable indicator arrow
        WinArrow.SetActive(false);

        // Lerp player to front of doors
        Vector3 initialPos = IsPlayer.instance.transform.position;

        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / 1.0f;


        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / 2.0f;

            IsPlayer.instance.transform.position = Vector3.Lerp(initialPos, frontOfDoorsLocation, progress);


            yield return null;
        }

        // Lerp player inside church
        initialPos = IsPlayer.instance.transform.position;

        initial_time = Time.time;
        progress = (Time.time - initial_time) / 1.0f;


        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / 2.0f;

            IsPlayer.instance.transform.position = Vector3.Lerp(initialPos, insideChurchLocation, progress);


            yield return null;
        }

        // Wait 2 seconds, then win
        yield return new WaitForSeconds(2.0f);

        GameControl.WinGame();
    }
}
