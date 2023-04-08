using System.Collections;
using Events;
using UnityEngine;

public class ScreenShakeManager : MonoBehaviour {
    private Subscription<ScreenShakeEvent> shakeSub;
    private Subscription<ScreenShakeToggleEvent> shakeToggleSub;

    private bool shaking;

    // Start is called before the first frame update
    private void Start() {
        shakeSub = EventBus.Subscribe<ScreenShakeEvent>(_OnScreenShake);
        shakeToggleSub = EventBus.Subscribe<ScreenShakeToggleEvent>(_OnToggleScreenShake);
    }

    private void _OnScreenShake(ScreenShakeEvent sse) {
        transform.localPosition = Random.onUnitSphere * sse.amplitude;
    }

    private void _OnToggleScreenShake(ScreenShakeToggleEvent sste) {
        switch (shaking) {
            case true:
                shaking = false;
                break;
            default:
                shaking = true;
                StartCoroutine(shakeScreen(sste.amplitude, sste.shakeFrequency));
                break;
        }
    }

    public float k = 0.1f;
    public float dampening_factor = 0.95f;
    private Vector3 velocity = Vector3.zero;

    // Update is called once per frame
    private void Update() {
        var displacement = Vector3.zero - transform.localPosition;
        var acceleration = k * displacement;
        velocity += acceleration;
        velocity *= dampening_factor;

        transform.localPosition += velocity;

        //Debug();
    }

    private void Debug() {
        if (Input.GetKeyDown(KeyCode.T)) EventBus.Publish(new ScreenShakeEvent());

        if (Input.GetKeyDown(KeyCode.Y)) EventBus.Publish(new ScreenShakeToggleEvent());
    }

    private IEnumerator shakeScreen(float amplitude, float frequency) {
        while (shaking) {
            transform.localPosition = Random.onUnitSphere * amplitude;

            yield return new WaitForSeconds(frequency);
        }
    }
}