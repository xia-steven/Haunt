using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveSkeleton : MonoBehaviour
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
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < sprites.Length; i++)
        {
            yield return new WaitForSeconds(0.5f);
            sr.sprite = sprites[i];
        }
    }
}
