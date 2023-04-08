using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu_UI {
    public class MainMenu : MonoBehaviour {
        private PlayerControls playerControls;

        // Start is called before the first frame update
        private void Awake() {
            playerControls = new PlayerControls();
        }

        public void LoadTutorial() {
            SceneManager.LoadScene("TutorialHubWorld");
        }

        public void Quit() {
            Application.Quit();
        }
    }
}