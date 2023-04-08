using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour {
    void Awake() {
        var objs = GameObject.FindGameObjectsWithTag("Player");

        if (objs.Length > 1) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}