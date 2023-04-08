using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MoonDialController : MonoBehaviour
{
    [SerializeField] private Sprite[] moonSprites;
    private Image moonUI;
    private float nightDuration;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "GameScene")
        {
            gameObject.SetActive(false);
            return;
        };
        nightDuration = GameControl.NightTimeRemaining;
        moonUI = GetComponent<Image>();
        Debug.Log("Duration: " + nightDuration);
        StartCoroutine(FetchNightDuration());
    }

    IEnumerator FetchNightDuration()
    {
        yield return new WaitForSeconds(1f);
        nightDuration = GameControl.NightTimeRemaining;
    }

    // Update is called once per frame
    void Update()
    {
        float pctComplete = (nightDuration - GameControl.NightTimeRemaining) / nightDuration;
        int dialTime = Mathf.Clamp((int)(pctComplete * (float)moonSprites.Length), 0, moonSprites.Length-1);
        Debug.Log("remaining: " + GameControl.NightTimeRemaining);
        moonUI.sprite = moonSprites[dialTime];
    }
}
