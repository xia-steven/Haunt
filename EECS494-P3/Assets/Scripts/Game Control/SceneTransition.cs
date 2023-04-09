using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {
    public static SceneTransition currentScene;
    private Animator animator;
    private static readonly int FadeOut = Animator.StringToHash("FadeOut");

    private void Awake() {
        currentScene = this;
    }

    private void Start() {
        animator = GetComponent<Animator>();
    }

    public void FadeToScene() {
        animator.SetTrigger(FadeOut);
    }

    public void OnFadeComplete() {
        if (SceneManager.GetActiveScene().name != "Tutorial Hub World") {
            SceneManager.LoadScene("TutorialGameScene");
        }
        else if (GameControl.Day != 3 || SceneManager.GetActiveScene().name != "HubWorld") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name == "HubWorld" ? "GameScene" : "HubWorld");
        }
        else if (SceneManager.GetActiveScene().name == "HubWorld") {
            SceneManager.LoadScene("LAB_BossTesting");
        }
    }
}