using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {
    [SerializeField] GameObject healthPipPrefab;

    [SerializeField] Sprite fullHeartImage;
    [SerializeField] Sprite halfHeartImage;
    [SerializeField] Sprite emptyHeartImage;
    [SerializeField] Sprite lockedHeartImage;

    [SerializeField] float loadWaitTime = 0.2f;
    
    List<Image> healthPips = new List<Image>();

    Subscription<HealthUIUpdate> ui_update_event;

    // Start is called before the first frame update
    void Start() {
        ui_update_event = EventBus.Subscribe<HealthUIUpdate>(_OnUpdate);

        InitializeHealth();
        //StartCoroutine(loadHealth());
    }

    private IEnumerator loadHealth() {
        int maxHealth = IsPlayer.instance.GetMaxHealth();
        for (int i = 0; i < healthPips.Count; ++i) {
            yield return new WaitForSeconds(loadWaitTime);
            healthPips[i].enabled = true;
            healthPips[i].sprite = halfHeartImage;
            if (maxHealth / 2 > i) {
                yield return new WaitForSeconds(loadWaitTime);
                healthPips[i].sprite = fullHeartImage;
            }
        }
    }

    void InitializeHealth()
    {
        for (int i = 0; i < IsPlayer.instance.GetMaxHealth()/2; ++i)
        {
            GameObject newPip = Instantiate(healthPipPrefab, transform.localPosition, Quaternion.identity);
            newPip.transform.localScale = Vector3.one;
            newPip.transform.SetParent(transform, false);
            healthPips.Add(newPip.GetComponent<Image>());
        }
    }

    void _OnUpdate(HealthUIUpdate e)
    {
        Debug.Log(e);
        // iterate backwards from the end snuffing out locked hearts
        int lockedHearts = e.updated_locked_health / 2;
        for (int i = 0; i < lockedHearts; i++)
        {
            healthPips[healthPips.Count - 1 - i].sprite = lockedHeartImage;
        }
        
        // then update the remaining full, half, and empty;
        int fullHearts = e.updated_health / 2;
        int halfHeart = e.updated_health % 2;
        for (int i = 0; i < fullHearts; i++)
        {
            healthPips[i].sprite = fullHeartImage;
        }
        if (halfHeart == 1)
        {
            healthPips[fullHearts].sprite = halfHeartImage;
        }
        // fill out rest of hearts with empties
        for (int i = fullHearts+halfHeart; i < 3-lockedHearts; i++)
        {
            healthPips[i].sprite = emptyHeartImage;
        }
        
        // add any shields to UI
        
    }
    

    



    // combines the effects of a pedestal being destroyed with a heal event
    void _OnMaxHealthIncrease(IncreaseMaxHealthEvent e)
    {
        GameObject newPip = Instantiate(healthPipPrefab, transform.localPosition, Quaternion.identity);
        newPip.transform.localScale = Vector3.one;
        newPip.transform.SetParent(transform, false);
        healthPips.Add(newPip.GetComponentsInChildren<Image>()[1]);
        healthPips[healthPips.Count - 1].enabled = false;
        
        var newHealth = IsPlayer.instance.GetHealth();
    }
    

    private void OnDestroy() {
        EventBus.Unsubscribe(ui_update_event);
    }
}