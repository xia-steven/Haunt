using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsSprite : MonoBehaviour
{
    void Start()
    {
        transform.localScale = new Vector3(1, 1 / CameraStretch.instance.height, 1);
    }
}
