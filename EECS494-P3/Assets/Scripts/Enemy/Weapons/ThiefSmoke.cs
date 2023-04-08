using UnityEngine;

namespace Enemy.Weapons {
    [RequireComponent(typeof(SpriteRenderer))]
    public class ThiefSmoke : MonoBehaviour {
        private float fadeDuration = 1.0f;

        private SpriteRenderer sprite;
        private Color color;

        private void Start() {
            sprite = GetComponent<SpriteRenderer>();
            color = sprite.color;
        }

        // Update is called once per frame
        private void Update() {
            switch (fadeDuration) {
                case <= 0:
                    Destroy(gameObject);
                    break;
            }

            color.a = fadeDuration;
            sprite.color = color;

            fadeDuration -= Time.deltaTime;
        }
    }
}