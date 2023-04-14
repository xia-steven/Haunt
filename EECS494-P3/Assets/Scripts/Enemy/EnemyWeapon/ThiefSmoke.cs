using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ThiefSmoke : MonoBehaviour {
    float fadeDuration = 1.0f;

    SpriteRenderer sprite;
    Color color;

    private void Start() {
        sprite = GetComponent<SpriteRenderer>();
        color = sprite.color;
    }

    // Update is called once per frame
    void Update() {
        if (fadeDuration <= 0) {
            Destroy(this.gameObject);
        }

        color.a = fadeDuration;
        sprite.color = color;

        fadeDuration -= Time.deltaTime;
    }
}