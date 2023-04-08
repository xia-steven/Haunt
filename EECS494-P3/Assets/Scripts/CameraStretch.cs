using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraStretch : MonoBehaviour {
    public static CameraStretch instance;

    Camera cam;
    public float height = 1f;
    public float width = 1f;

    // Use this for initialization
    void Awake() {
        if (instance == null) instance = this;
        else Destroy(this);

        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update() {
        //stretch view//
        cam.ResetProjectionMatrix();
        var m = cam.projectionMatrix;

        m.m11 *= height;
        m.m00 *= width;
        cam.projectionMatrix = m;
    }
}