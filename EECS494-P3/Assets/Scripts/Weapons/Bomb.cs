using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float explosionTime;
    private float droppedTime;
    float flashDelay = 0.1f;
    [SerializeField] GameObject bulletSprite;
    private SpriteRenderer sr;
    private bool flashing = false;
    private bool red = false;
    private Color defaultColor;

    private void Awake()
    {
        droppedTime = Time.time;
        sr = bulletSprite.GetComponent<SpriteRenderer>();
        defaultColor = sr.color;
    }

    void Update()
    {
        // Destroy (explode) bomb after time has passed
        float passedTime = Time.time - droppedTime;

        // Flash bomb after half explosion time
        if (!flashing && passedTime >= (explosionTime / 2))
            StartCoroutine(FlashSprite());

        if (passedTime >= explosionTime)
        {
            Destroy(gameObject);
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
