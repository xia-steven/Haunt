using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipToBoss : MonoBehaviour
{


    public void onSkipToBoss()
    {
        SceneManager.LoadScene("LAB_BossTesting");
        TimeManager.ResetTimeScale();
        // Enable player in case disabled
        EventBus.Publish(new EnablePlayerEvent());
        GameObject player = GameObject.Find("Player");
        PlayerHasHealth health = player.GetComponent<PlayerHasHealth>();
        player.transform.position = new Vector3(0, 0.5f, 0);
        health.ResetHealth();
    }
}
