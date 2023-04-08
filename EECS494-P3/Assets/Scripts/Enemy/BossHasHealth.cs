using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Enemy {
    public class BossHasHealth : HasHealth {
        private SpriteRenderer sr;
        private Color normalColor;
        [SerializeField] private GameObject healthBar;
        private Image healthBarImage;
        private TMP_Text healthText;
        private IsBoss boss;

        private float lastClericSpawn;
        private int clericCount = 1;

        private void Start() {
            foreach (Transform child in transform)
                switch (child.gameObject.name) {
                    case "Sprite":
                        sr = child.gameObject.GetComponent<SpriteRenderer>();
                        normalColor = sr.color;
                        break;
                }

            healthBarImage = healthBar.GetComponent<Image>();
            healthText = healthBar.GetComponentInChildren<TMP_Text>();
            healthText.text = health.ToString();

            boss = GetComponent<IsBoss>();

            lastClericSpawn = maxHealth;
        }


        public override void AlterHealth(int healthDelta) {
            base.AlterHealth(healthDelta);
            StartCoroutine(FlashRed());
            switch (health) {
                case <= 0:
                    Destroy(gameObject);
                    // Hide health bar on death
                    healthBarImage.gameObject.SetActive(false);

                    Game_Control.GameControl.WinGame();
                    break;
                default:
                    healthBarImage.fillAmount = health / maxHealth;
                    healthText.text = health.ToString();
                    break;
            }

            if (health / maxHealth < 0.33)
                boss.enabledLaser = true;
            else if (health / maxHealth < 0.66) boss.enabledGroundPound = true;

            switch (lastClericSpawn - health) {
                // If we haven't spawned clerics in 20 health
                case > 20:
                    boss.SpawnClerics(clericCount);
                    ++clericCount;
                    lastClericSpawn = health;
                    break;
            }
        }


        private IEnumerator FlashRed() {
            sr.color = Color.red;
            yield return new WaitForSeconds(.1f);
            sr.color = normalColor;
        }

        public void setMaxHealth(float health_in) {
            maxHealth = health_in;
            health = maxHealth;
            lastClericSpawn = health;
            healthText.text = health.ToString();
        }
    }
}