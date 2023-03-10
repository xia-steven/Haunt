using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private PlayerControls playerControls;
    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new PlayerControls();
    }

    public void LoadLevelOne()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevelTwo()
    {
        SceneManager.LoadScene("Level2");
    }
    public void LoadLevelThree()
    {
        SceneManager.LoadScene("Level3");
    }

    public void LoadLevelFour()
    {
        SceneManager.LoadScene("Level4");
    }
}
