using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipTutorialBtn : MonoBehaviour
{


    public void onSkipTutorial()
    {
        SceneManager.LoadScene("GameScene");
        TimeManager.ResetTimeScale();
        GameObject player = GameObject.Find("Player");
        player.transform.position = new Vector3(0f, 0.5f, 0f);
    }
}
