using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    private PlayerControls playerControls;
    private InputAction menu;

    [SerializeField] GameObject pauseUI;
    bool isPaused;

    private Subscription<GamePauseEvent> pauseSub;
    private Subscription<GamePlayEvent> playSub;

    // Start is called before the first frame update
    void Awake() {
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

    void Pause(InputAction.CallbackContext context) {
        if (GameObject.Find("Player").GetComponent<HasHealth>().GetHealth() == 0) {
            return;
        }

        if (isPaused) {
            EventBus.Publish(new GamePlayEvent());
        }
        else {
            EventBus.Publish(new GamePauseEvent());
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

    public void RestartLevel() {
        isPaused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void LoadMainMenu() {
        isPaused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenu"));
    }
}