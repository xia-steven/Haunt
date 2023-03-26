using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    private PlayerControls playerControls;

    // Start is called before the first frame update
    void Awake() {
        playerControls = new PlayerControls();
    }

    public void LoadTutorial() {
        SceneManager.LoadScene("TutorialHubWorld");
    }

    public void Quit() {
        Application.Quit();
    }
}