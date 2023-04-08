using Events;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipToBoss : MonoBehaviour {
    public void onSkipToBoss() {
        SceneManager.LoadScene("LAB_BossTesting");
        TimeManager.ResetTimeScale();
        // Enable player in case disabled
        EventBus.Publish(new EnablePlayerEvent());
        var player = GameObject.Find("Player");
        var health = player.GetComponent<PlayerHasHealth>();
        player.transform.position = new Vector3(0, 0.5f, 0);
        health.ResetHealth();
    }
}