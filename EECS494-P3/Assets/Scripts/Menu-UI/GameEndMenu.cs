using System.Collections;
using Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu_UI {
    public class GameEndMenu : MonoBehaviour {
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private GameObject gameWinUI;

        private Subscription<GameLossEvent> lossSub;
        private Subscription<GameWinEvent> winSub;

        // Start is called before the first frame update
        private void Start() {
            lossSub = EventBus.Subscribe<GameLossEvent>(_Loss);
            winSub = EventBus.Subscribe<GameWinEvent>(_Win);
        }

        private void _Loss(GameLossEvent e) {
            StartCoroutine(ShowLossMenu(e));
        }

        private void _Win(GameWinEvent e) {
            foreach (Transform child in gameWinUI.transform)
                switch (child.name) {
                    case "Restart":
                        EventSystem.current.SetSelectedGameObject(child.gameObject);
                        break;
                }

            Time.timeScale = 0;
            gameWinUI.SetActive(true);
        }

        private IEnumerator ShowLossMenu(GameLossEvent e) {
            while (!e.finishedDeathAnimation) yield return null;

            foreach (Transform child in gameOverUI.transform)
                switch (child.name) {
                    case "Restart":
                        EventSystem.current.SetSelectedGameObject(child.gameObject);
                        break;
                }

            Time.timeScale = 0;
            gameOverUI.SetActive(true);
        }
    }
}