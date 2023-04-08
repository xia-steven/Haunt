using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {
    [SerializeField] GameObject healthPipPrefab;
    [SerializeField] private GameObject shieldPipPrefab;

    [SerializeField] Sprite fullHeartImage;
    [SerializeField] Sprite halfHeartImage;
    [SerializeField] Sprite emptyHeartImage;
    [SerializeField] Sprite lockedHeartImage;
    [SerializeField] Sprite fullShieldImage;
    [SerializeField] Sprite brokenShieldImage;

    List<Image> healthPips = new List<Image>();
    private List<Image> shieldPips = new List<Image>();
    private int maxShields = 6;

    Subscription<HealthUIUpdate> ui_update_event;

    // Start is called before the first frame update
    void Start() {
        ui_update_event = EventBus.Subscribe<HealthUIUpdate>(_OnUpdate);

        InitializeHealth();
        InitializeShields();
    }

    void InitializeHealth() {
        for (int i = 0; i < IsPlayer.instance.GetMaxHealth() / 2; ++i) {
            GameObject newPip = Instantiate(healthPipPrefab, transform.localPosition, Quaternion.identity);
            newPip.transform.localScale = Vector3.one;
            newPip.transform.SetParent(transform, false);
            healthPips.Add(newPip.GetComponent<Image>());
        }
    }

    void InitializeShields() {
        for (int i = 0; i < maxShields / 2; ++i) {
            GameObject newPip = Instantiate(shieldPipPrefab, transform.localPosition, Quaternion.identity);
            newPip.transform.localScale = Vector3.one;
            newPip.transform.SetParent(transform, false);
            shieldPips.Add(newPip.GetComponent<Image>());
            shieldPips[i].color = new Color(0, 0, 0, 0);
        }
    }

    void _OnUpdate(HealthUIUpdate e) {
        Debug.Log(e);
        // iterate backwards from the end snuffing out locked hearts
        int lockedHearts = e.updated_locked_health / 2;
        for (int i = 0; i < lockedHearts; i++) {
            healthPips[healthPips.Count - 1 - i].sprite = lockedHeartImage;
        }

        // then update the remaining full, half, and empty;
        int fullHearts = e.updated_health / 2;
        int halfHeart = e.updated_health % 2;
        for (int i = 0; i < fullHearts; i++) {
            healthPips[i].sprite = fullHeartImage;
        }

        if (halfHeart == 1) {
            healthPips[fullHearts].sprite = halfHeartImage;
        }

        // fill out rest of hearts with empties
        for (int i = fullHearts + halfHeart; i < 3 - lockedHearts; i++) {
            healthPips[i].sprite = emptyHeartImage;
        }

        // shield UI update
        int fullShields = e.updated_shield_health / 2;
        int halfShields = e.updated_shield_health % 2;
        for (int i = 0; i < fullShields; ++i) {
            shieldPips[i].sprite = fullShieldImage;
            shieldPips[i].color = new Color(1, 1, 1, 1);
        }

        for (int i = fullShields; i < shieldPips.Count; i++) {
            shieldPips[i].color = new Color(0, 0, 0, 0);
        }

        if (halfShields == 1) {
            shieldPips[fullShields].sprite = brokenShieldImage;
            shieldPips[fullShields].color = new Color(1, 1, 1, 1);
        }
    }


    private void OnDestroy() {
        EventBus.Unsubscribe(ui_update_event);
    }
}