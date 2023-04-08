using System.Collections.Generic;
using Events;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace Menu_UI {
    public class HealthUI : MonoBehaviour {
        [SerializeField] private GameObject healthPipPrefab;
        [SerializeField] private GameObject shieldPipPrefab;

        [SerializeField] private Sprite fullHeartImage;
        [SerializeField] private Sprite halfHeartImage;
        [SerializeField] private Sprite emptyHeartImage;
        [SerializeField] private Sprite lockedHeartImage;
        [SerializeField] private Sprite fullShieldImage;
        [SerializeField] private Sprite brokenShieldImage;

        private readonly List<Image> healthPips = new();
        private readonly List<Image> shieldPips = new();
        private const int maxShields = 6;

        private Subscription<HealthUIUpdate> ui_update_event;

        // Start is called before the first frame update
        private void Start() {
            ui_update_event = EventBus.Subscribe<HealthUIUpdate>(_OnUpdate);

            InitializeHealth();
            InitializeShields();
        }

        private void InitializeHealth() {
            for (var i = 0; i < IsPlayer.instance.GetMaxHealth() / 2; ++i) {
                var newPip = Instantiate(healthPipPrefab, transform.localPosition, Quaternion.identity);
                newPip.transform.localScale = Vector3.one;
                newPip.transform.SetParent(transform, false);
                healthPips.Add(newPip.GetComponent<Image>());
            }
        }

        private void InitializeShields() {
            for (var i = 0; i < maxShields / 2; ++i) {
                var newPip = Instantiate(shieldPipPrefab, transform.localPosition, Quaternion.identity);
                newPip.transform.localScale = Vector3.one;
                newPip.transform.SetParent(transform, false);
                shieldPips.Add(newPip.GetComponent<Image>());
                shieldPips[i].color = new Color(0, 0, 0, 0);
            }
        }

        private void _OnUpdate(HealthUIUpdate e) {
            Debug.Log(e);
            // iterate backwards from the end snuffing out locked hearts
            var lockedHearts = e.updated_locked_health / 2;
            for (var i = 0; i < lockedHearts; i++) healthPips[healthPips.Count - 1 - i].sprite = lockedHeartImage;

            // then update the remaining full, half, and empty;
            var fullHearts = e.updated_health / 2;
            var halfHeart = e.updated_health % 2;
            for (var i = 0; i < fullHearts; i++) healthPips[i].sprite = fullHeartImage;

            healthPips[fullHearts].sprite = halfHeart switch {
                1 => halfHeartImage,
                _ => healthPips[fullHearts].sprite
            };

            // fill out rest of hearts with empties
            for (var i = fullHearts + halfHeart; i < 3 - lockedHearts; i++) healthPips[i].sprite = emptyHeartImage;

            // shield UI update
            var fullShields = e.updated_shield_health / 2;
            var halfShields = e.updated_shield_health % 2;
            for (var i = 0; i < fullShields; ++i) {
                shieldPips[i].sprite = fullShieldImage;
                shieldPips[i].color = new Color(1, 1, 1, 1);
            }

            for (var i = fullShields; i < shieldPips.Count; i++) shieldPips[i].color = new Color(0, 0, 0, 0);

            switch (halfShields) {
                case 1:
                    shieldPips[fullShields].sprite = brokenShieldImage;
                    shieldPips[fullShields].color = new Color(1, 1, 1, 1);
                    break;
            }
        }


        private void OnDestroy() {
            EventBus.Unsubscribe(ui_update_event);
        }
    }
}