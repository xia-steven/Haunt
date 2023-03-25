using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsSprite : MonoBehaviour
{
    void Start()
    {
        Vector3 l = transform.localScale;
        transform.localScale = new Vector3(l.x, l.y / CameraStretch.instance.height, l.z);
        Debug.Log(transform.localScale);
    }
}
