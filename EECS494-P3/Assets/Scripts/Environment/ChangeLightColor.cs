using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLightColor : MonoBehaviour {
    [SerializeField] private Color nightColor;
    [SerializeField] private Color dayColor;

    private Light sunlight;


    private bool roundStopped;

    // Set when the night starts
    private float duration = 1.0f;
    private float resetDuration = 2.0f;
    private Gradient colors;

    private Subscription<NightBeginEvent> nightStartsub;
    private Subscription<NightEndEvent> nightEndsub;

    // Start is called before the first frame update
    private void Awake() {
        sunlight = GetComponent<Light>();

        nightStartsub = EventBus.Subscribe<NightBeginEvent>(_OnNightBegin);
        nightEndsub = EventBus.Subscribe<NightEndEvent>(_OnNightEnd);

        // Initialize gradient
        // Code from: https://stackoverflow.com/questions/38642587/making-a-gradient-and-change-colors-based-on-that-gradient-in-unity3d-c-sharp
        colors = new Gradient();
        var gck = new GradientColorKey[2];
        var gak = new GradientAlphaKey[2];
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

    private void _OnNightBegin(NightBeginEvent nbe) {
        roundStopped = false;
        StartCoroutine(ChangeColor());
    }

    private void _OnNightEnd(NightEndEvent nee) {
        roundStopped = true;
        //StartCoroutine(ResetColor());
    }


    private IEnumerator ChangeColor() {
        //TODO: Make less horrible
        yield return null;
        if (GameControl.Day == 0) {
            duration = 15f;
        }
        else {
            duration = GameControl.NightTimeRemaining;
        }


        var initial_time = Time.time;
        var progress = (Time.time - initial_time) / duration;

        while (progress < 1.0f && !roundStopped) {
            progress = (Time.time - initial_time) / duration;

            sunlight.color = colors.Evaluate(progress);


            yield return null;
        }
    }

    private IEnumerator ResetColor() {
        var initial_time = Time.time;
        var progress = (Time.time - initial_time) / resetDuration;

        while (progress < 1.0f) {
            progress = (Time.time - initial_time) / resetDuration;

            sunlight.color = colors.Evaluate(1.0f - progress);


            yield return null;
        }
    }
}