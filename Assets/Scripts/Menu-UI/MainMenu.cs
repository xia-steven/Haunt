using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    private PlayerControls playerControls;

    // Start is called before the first frame update
    private void Awake() {
        playerControls = new PlayerControls();
    }

    public void LoadTutorial() {
        SceneManager.LoadScene("TutorialHubWorld");
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadCredits() {
        SceneManager.LoadScene("Credits");
    }

    public void Quit() {
        Application.Quit();
    }
}