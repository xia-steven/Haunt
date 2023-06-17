using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLightColor : MonoBehaviour {
    [SerializeField] Color nightColor;
    [SerializeField] Color dayColor;

    Light sunlight;


    bool roundStopped = false;

    // Set when the night starts
    float duration = 1.0f;
    float resetDuration = 2.0f;
    Gradient colors;

    Subscription<NightBeginEvent> nightStartsub;
    Subscription<NightEndEvent> nightEndsub;

    // Start is called before the first frame update
    void Awake() {
        sunlight = GetComponent<Light>();

        nightStartsub = EventBus.Subscribe<NightBeginEvent>(_OnNightBegin);
        nightEndsub = EventBus.Subscribe<NightEndEvent>(_OnNightEnd);

        // Initialize gradient
        // Code from: https://stackoverflow.com/questions/38642587/making-a-gradient-and-change-colors-based-on-that-gradient-in-unity3d-c-sharp
        colors = new Gradient();
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
        colors.SetKeys(gck, gak);
    }

    void _OnNightBegin(NightBeginEvent nbe) {
        roundStopped = false;
        StartCoroutine(ChangeColor());
    }

    void _OnNightEnd(NightEndEvent nee) {
        roundStopped = true;
    }


    IEnumerator ChangeColor() {
        yield return null;
        duration = GameControl.NightTimeRemaining;


        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / duration;

        while (progress < 1.0f && !roundStopped) {
            progress = (Time.time - initial_time) / duration;

            sunlight.color = colors.Evaluate(progress);


            yield return null;
        }
    }

}