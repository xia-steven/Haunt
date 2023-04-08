using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherBullet : Bullet
{
    Rigidbody rb;
    float slowdownSpeed = 0.985f;
    float slowdownDelay = 0.6f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void setLifetime(float lifetime)
    {
        bulletLife = lifetime;
    }

    private new void Update()
    {
        base.Update();

        // Slowdown over time, after a delay
        if (Time.time - firedTime >= slowdownDelay)
        {
            rb.velocity *= slowdownSpeed;
        }
    }
}
