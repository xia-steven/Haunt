using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingSword : MonoBehaviour
{
    private bool rotating = false;
    // Rotation speed in degrees per second
    private float rotationSpeed = 10f;

    public void SetUp(float speed)
    {
        rotationSpeed = speed;
        rotating = true;
    }

    private void FixedUpdate()
    {
        if (rotating)
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
