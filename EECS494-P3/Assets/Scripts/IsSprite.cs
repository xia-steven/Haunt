using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsSprite : MonoBehaviour {
    public float scale = 1.0f;

    void Start() {
        transform.localScale = new Vector3(scale, scale / CameraStretch.instance.height, scale);
    }
}