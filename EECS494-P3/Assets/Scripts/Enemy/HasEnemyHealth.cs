using System.Collections;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy {
    public class HasEnemyHealth : HasHealth {
        private GameObject coinPrefab;
        private GameObject healthPrefab;
        private SpriteRenderer sr;
        private Color normalColor;
        private bool isCleric;

        private void Start() {
            coinPrefab = Resources.Load<GameObject>("Prefabs/Coin");
            healthPrefab = Resources.Load<GameObject>("Prefabs/Health");
            foreach (Transform child in transform)
                switch (child.gameObject.name) {
                    case "Sprite":
                        sr = child.gameObject.GetComponent<SpriteRenderer>();
                        normalColor = sr.color;
                        break;
                }
        }

        public override void AlterHealth(int healthDelta) {
            base.AlterHealth(healthDelta);
            StartCoroutine(FlashRed());
            switch (health) {
                case <= 0: {
                    var roulletteBall = Random.Range(0, 100);
                    // Only drop collectibles if not the tutorial day
                    if (Game_Control.GameControl.Day != 0 && isCleric &&
                        IsPlayer.instance.GetHealth() < IsPlayer.instance.GetMaxHealth())
                        Instantiate(healthPrefab, transform.position, Quaternion.identity);
                    else
                        switch (roulletteBall) {
                            // Only drop collectibles if not the tutorial day
                            case >= 40 and < 80 when Game_Control.GameControl.Day != 0:
                                Instantiate(coinPrefab, transform.position, Quaternion.identity);
                                break;
                        }

                    Destroy(gameObject);
                    break;
                }
            }
        }

        /// <summary>
        /// Called by enemybase to initialize the enemy's max health
        /// </summary>
        /// <param name="newMaxHealth"> New max health for the enemy</param>
        public void setMaxHealth(int newMaxHealth) {
            maxHealth = newMaxHealth;
            health = maxHealth;
        }

        private IEnumerator FlashRed() {
            sr.color = Color.red;
            yield return new WaitForSeconds(.1f);
            sr.color = normalColor;
        }

        public void setClericStatus(bool cleric) {
            isCleric = cleric;
        }
    }
}