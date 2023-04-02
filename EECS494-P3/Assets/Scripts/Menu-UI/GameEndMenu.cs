using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameEndMenu : MonoBehaviour {
    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject gameWinUI;

    private Subscription<GameLossEvent> lossSub;
    private Subscription<GameWinEvent> winSub;

    // Start is called before the first frame update
    void Start() {
        lossSub = EventBus.Subscribe<GameLossEvent>(_Loss);
        winSub = EventBus.Subscribe<GameWinEvent>(_Win);
    }

    void _Loss(GameLossEvent e) {
        StartCoroutine(ShowLossMenu(e));
    }

    void _Win(GameWinEvent e) {
        foreach (Transform child in gameWinUI.transform) {
            if (child.name == "Restart") {
                EventSystem.current.SetSelectedGameObject(child.gameObject);
            }
        }

        Time.timeScale = 0;
        gameWinUI.SetActive(true);
    }

    IEnumerator ShowLossMenu(GameLossEvent e)
    {
        while(!e.finishedDeathAnimation)
        {
            yield return null;
        }

        foreach (Transform child in gameOverUI.transform)
        {
            if (child.name == "Restart")
            {
                EventSystem.current.SetSelectedGameObject(child.gameObject);
            }
        }

        Time.timeScale = 0;
        gameOverUI.SetActive(true);
    }
}