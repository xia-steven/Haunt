using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class IsAnimatedIcon : MonoBehaviour
{
    [SerializeField] Sprite standardSprite;
    [SerializeField] Sprite pressedSprite;

    SpriteRenderer render;

    bool animate = true;

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();

        StartCoroutine(animateSprite());
    }

    private void OnDisable()
    {
        animate = false;
    }

    private void OnEnable()
    {
        animate = true;
        render = GetComponent<SpriteRenderer>();
        StartCoroutine(animateSprite());
    }

    IEnumerator animateSprite()
    {
        while (animate)
        {
            render.sprite = standardSprite;

            yield return new WaitForSeconds(0.5f);

            render.sprite = pressedSprite;

            yield return new WaitForSeconds(0.5f);
        }
    }
}
