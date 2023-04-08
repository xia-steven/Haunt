using Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Menu_UI {
    public class PauseMenu : MonoBehaviour {
        private PlayerControls playerControls;
        private InputAction menu;

        [SerializeField] private GameObject pauseUI;
        private bool isPaused;

        private Subscription<GamePauseEvent> pauseSub;
        private Subscription<GamePlayEvent> playSub;

        // Start is called before the first frame update
        private void Awake() {
            playerControls = new PlayerControls();

            pauseSub = EventBus.Subscribe<GamePauseEvent>(_ActivateMenu);
            playSub = EventBus.Subscribe<GamePlayEvent>(_DeactivateMenu);
        }

        private void OnEnable() {
            menu = playerControls.UI.Escape;
            menu.Enable();

            menu.performed += Pause;
        }

        private void OnDisable() {
            menu.Disable();
        }

        private void Pause(InputAction.CallbackContext context) {
            if (GameObject.Find("Player").GetComponent<HasHealth>().GetHealth() == 0) return;

            switch (isPaused) {
                case true:
                    EventBus.Publish(new GamePlayEvent());
                    break;
                default:
                    EventBus.Publish(new GamePauseEvent());
                    break;
            }
        }

        private void _ActivateMenu(GamePauseEvent e) {
            isPaused = true;
            Time.timeScale = 0;
            AudioListener.pause = true;
            pauseUI.SetActive(true);
        }

        private void _DeactivateMenu(GamePlayEvent e) {
            isPaused = false;
            Time.timeScale = 1;
            AudioListener.pause = false;
            pauseUI.SetActive(false);
            isPaused = false;
        }

        // Used for the button callback
        public void DeactivateMenu() {
            EventBus.Publish(new GamePlayEvent());
        }


        public void RestartLevel() {
            Game_Control.GameControl.ResetGame("TutorialHubWorld");
        }

        public void QuitGame() {
            Application.Quit();
        }


        public void LoadMainMenu() {
            isPaused = false;
            Time.timeScale = 1;
            AudioListener.pause = false;
            Game_Control.GameControl.ResetGame("MainMenu");
        }
    }
}