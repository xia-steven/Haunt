using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullTrailer : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(Revive());
    }

    private IEnumerator Revive()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < sprites.Length; i++)
        {
            yield return new WaitForSeconds(0.2f);
            sr.sprite = sprites[i];
        }

        int active = sprites.Length - 2;
        bool plus = true;
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            sr.sprite = sprites[active];
            if (plus)
            {
                active++;
                plus = false;
            }
            else
            {
                active--;
                plus = true;
            }
        }
    }
}
