using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class CrosshairCursorManager : MonoBehaviour {
    public static CrosshairCursorManager instance;
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D[] reloadCursors;
    [SerializeField] private Texture2D[] flashRedCursors;
    private Subscription<ReloadStartedEvent> reload_sub;
    private Subscription<PlayerDamagedEvent> playerdamage_sub;
    [SerializeField] private float reloadDuration = 1f;
    private const float flashDuration = .25f;
    private readonly Vector2 clickPoint = new(16, 16);

    // Start is called before the first frame update
    private void Start() {
        instance = this;
        Cursor.SetCursor(defaultCursor, clickPoint, CursorMode.Auto);
        reload_sub = EventBus.Subscribe<ReloadStartedEvent>(_OnReload);
        playerdamage_sub = EventBus.Subscribe<PlayerDamagedEvent>(_OnDamage);
    }

    private void _OnReload(ReloadStartedEvent e) {
        StartCoroutine(AnimateKeyframes(reloadCursors, e.reloadTime));
    }

    private void _OnDamage(PlayerDamagedEvent e) {
        StartCoroutine(AnimateKeyframes(flashRedCursors, flashDuration));
    }

    private IEnumerator AnimateKeyframes(IReadOnlyList<Texture2D> keyframes, float duration) {
        var secs_per_frame = duration / (float)keyframes.Count;
        for (var i = 1; i < keyframes.Count; i++) {
            Cursor.SetCursor(keyframes[i], clickPoint, CursorMode.Auto);
            yield return new WaitForSeconds(secs_per_frame);
        }

        Cursor.SetCursor(defaultCursor, clickPoint, CursorMode.Auto);
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(reload_sub);
        EventBus.Unsubscribe(playerdamage_sub);
    }
}