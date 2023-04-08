using Events;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tutorial {
    public class SkipTutorialBtn : MonoBehaviour {
        public void onSkipTutorial() {
            SceneManager.LoadScene("GameScene");
            TimeManager.ResetTimeScale();
            // Enable player in case disabled
            EventBus.Publish(new EnablePlayerEvent());
            // Increment day if not already
            Game_Control.GameControl.Day = 0;
            var player = GameObject.Find("Player");
            var health = player.GetComponent<PlayerHasHealth>();
            health.ResetHealth();
        }
    }
}