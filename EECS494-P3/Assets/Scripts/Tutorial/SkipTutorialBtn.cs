using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipTutorialBtn : MonoBehaviour {
    public void onSkipTutorial() {
        SceneManager.LoadScene("GameScene");
        TimeManager.ResetTimeScale();
        // Enable player in case disabled
        EventBus.Publish(new EnablePlayerEvent());
        // Increment day if not already
        GameControl.Day = 0;
        GameObject player = GameObject.Find("Player");
        PlayerHasHealth health = player.GetComponent<PlayerHasHealth>();
        health.ResetHealth();
    }
}