using UnityEngine;

public class IsSprite : MonoBehaviour {
    public float scale = 1.0f;

    private void Start() {
        transform.localScale = new Vector3(scale, scale / CameraStretch.instance.height, scale);
    }
}