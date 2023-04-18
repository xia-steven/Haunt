using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameWinMenuReveal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject roseCanvas;
    [SerializeField] private GameObject buttons;

    [SerializeField] private GameObject playerUI;
    // Start is called before the first frame update
    void Start()
    {
        buttons.SetActive(false);
        playerUI.SetActive(false);
        roseCanvas.SetActive(true);
        StartCoroutine(DropRosesCutscene());
        StartCoroutine(TypeThankYou());

    }
    
    IEnumerator DropRosesCutscene()
    {
        RectTransform menuRect = roseCanvas.GetComponent<RectTransform>();
        Vector2 menuSize = menuRect.rect.size;
        
        GameObject rosePrefab = Resources.Load<GameObject>("Prefabs/UI/MenuWhiteRose");
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - start < 180f)
        {
            GameObject rose  = Instantiate(rosePrefab, menuRect);
            // Generate random x and y positions within the bounds of the menu object
            float randomX = Random.Range(-menuSize.x / 2, menuSize.x / 2);
            float randomY = Random.Range(-menuSize.y / 2, menuSize.y / 2);

            // Set the position of the animated GameObject to the random position
            RectTransform roseRect = rose.GetComponent<RectTransform>();
            roseRect.anchoredPosition = new Vector2(randomX, randomY);
            roseRect.localPosition = new Vector3(roseRect.localPosition.x, roseRect.localPosition.y,
                menuRect.localPosition.z - 1);
            yield return new WaitForSecondsRealtime(0.1f);

        }
    }

    IEnumerator TypeThankYou()
    {
        yield return new WaitForSecondsRealtime(2f);
        string thankYou = "Thank You!";
        int i = 0;
        while (i <= thankYou.Length)
        {
            Debug.Log(text.text);
            text.text = thankYou.Substring(0, i);
            ++i;
            yield return new WaitForSecondsRealtime(0.5f);
        }
        buttons.SetActive(true);
        foreach (Transform child in buttons.transform) {
            if (child.name == "Restart") {
                EventSystem.current.SetSelectedGameObject(child.gameObject);
            }
        }
        
    }

    
}
