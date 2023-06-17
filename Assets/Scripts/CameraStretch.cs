using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraStretch : MonoBehaviour {
    public static CameraStretch instance;

    private Camera cam;
    public float height = 1f;
    public float width = 1f;

    // Use this for initialization
    private void Awake() {
        if (instance == null) instance = this;
        else DestroyImmediate(this);

        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    private void Update() {
        //stretch view//
        cam.ResetProjectionMatrix();
        var m = cam.projectionMatrix;

        m.m11 *= height;
        m.m00 *= width;
        cam.projectionMatrix = m;
    }
}