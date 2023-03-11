using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLightColor : MonoBehaviour
{
    [SerializeField] Color nightColor;
    [SerializeField] Color dayColor;

    Light sunlight;


    bool changingColor = false;
    float duration = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        sunlight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if(changingColor == false)
        {
            changingColor = true;
            StartCoroutine(ChangeColor());
        }
    }

    IEnumerator ChangeColor()
    {
        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / duration;


        // Code from: https://stackoverflow.com/questions/38642587/making-a-gradient-and-change-colors-based-on-that-gradient-in-unity3d-c-sharp
        Gradient g = new Gradient();
        GradientColorKey[] gck = new GradientColorKey[2];
        GradientAlphaKey[] gak = new GradientAlphaKey[2];
        gck[0].color = nightColor;
        gck[0].time = 0F;
        gck[1].color = dayColor;
        gck[1].time = 1.0F;
        gak[0].alpha = 1.0F;
        gak[0].time = 0F;
        gak[1].alpha = 1.0F;
        gak[1].time = 1.0F;
        g.SetKeys(gck, gak);

        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / duration;

            sunlight.color = g.Evaluate(progress);


            yield return null;
        }



        changingColor = false;
    }
}
