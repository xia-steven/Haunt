using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherBullet : Bullet
{
    Rigidbody rb;
    float slowdownSpeed = 0.985f;
    float slowdownDelay = 0.6f;
    float flashDelay = 0.1f;
    [SerializeField] GameObject bulletSprite;
    private SpriteRenderer sr;
    private bool flashing = false;
    private bool red = false;
    private Color defaultColor;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sr = bulletSprite.GetComponent<SpriteRenderer>();
        defaultColor = sr.color;
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
            if (!flashing)
                StartCoroutine(FlashSprite());
        }
    }

    private IEnumerator FlashSprite()
    {
        flashing = true;

        // Loop until bullet explodes
        while (true)
        {
            yield return new WaitForSeconds(flashDelay);
            if (red)
            {
                sr.color = defaultColor;
                red = false;
            }
            else
            {
                sr.color = Color.red;
                red = true;
            }
        }
    }
}
