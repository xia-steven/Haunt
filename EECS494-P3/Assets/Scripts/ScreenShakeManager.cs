using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeManager : MonoBehaviour {
    Subscription<ScreenShakeEvent> shakeSub;
    Subscription<ScreenShakeToggleEvent> shakeToggleSub;

    bool shaking = false;

    // Start is called before the first frame update
    void Start() {
        shakeSub = EventBus.Subscribe<ScreenShakeEvent>(_OnScreenShake);
        shakeToggleSub = EventBus.Subscribe<ScreenShakeToggleEvent>(_OnToggleScreenShake);
    }

    void _OnScreenShake(ScreenShakeEvent sse) {
        transform.localPosition = UnityEngine.Random.onUnitSphere * sse.amplitude;
    }

    void _OnToggleScreenShake(ScreenShakeToggleEvent sste) {
        if (shaking) {
            shaking = false;
        }
        else {
            shaking = true;
            StartCoroutine(shakeScreen(sste.amplitude, sste.shakeFrequency));
        }
    }

    public float k = 0.1f;
    public float dampening_factor = 0.95f;
    Vector3 velocity = Vector3.zero;

    // Update is called once per frame
    void Update() {
        Vector3 displacement = Vector3.zero - transform.localPosition;
        Vector3 acceleration = k * displacement;
        velocity += acceleration;
        velocity *= dampening_factor;

        transform.localPosition += velocity;

        //Debug();
    }

    void Debug() {
        if (Input.GetKeyDown(KeyCode.T)) {
            EventBus.Publish(new ScreenShakeEvent());
        }

        if (Input.GetKeyDown(KeyCode.Y)) {
            EventBus.Publish(new ScreenShakeToggleEvent());
        }
    }

    IEnumerator shakeScreen(float amplitude, float frequency) {
        while (shaking) {
            transform.localPosition = UnityEngine.Random.onUnitSphere * amplitude;

            yield return new WaitForSeconds(frequency);
        }
    }
}