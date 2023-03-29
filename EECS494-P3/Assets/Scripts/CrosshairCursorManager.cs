using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CrosshairCursorManager: MonoBehaviour
{
    public static CrosshairCursorManager instance;
    [SerializeField] Texture2D defaultCursor;
    [SerializeField] Texture2D[] reloadCursors;
    [SerializeField] Texture2D[] flashRedCursors;
    private Subscription<ReloadEvent> reload_sub;
    private Subscription<PlayerDamagedEvent> playerdamage_sub;
    private float reloadDuration = 1f;
    private float flashDuration = .25f;
    private Vector2 clickPoint = new Vector2(16, 16);

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Cursor.SetCursor(defaultCursor, clickPoint, CursorMode.Auto);
        reload_sub = EventBus.Subscribe<ReloadEvent>(_OnReload);
        playerdamage_sub = EventBus.Subscribe<PlayerDamagedEvent>(_OnDamage);
    }

    private void _OnReload(ReloadEvent e)
    {
        StartCoroutine(AnimateKeyframes(reloadCursors,reloadDuration));
        
    }

    private void _OnDamage(PlayerDamagedEvent e)
    {
        StartCoroutine(AnimateKeyframes(flashRedCursors, flashDuration));
    }

    private IEnumerator AnimateKeyframes(Texture2D[] keyframes, float duration)
    {
        float secs_per_frame = duration/(float)keyframes.Length;
        for (int i = 1; i < keyframes.Length; i++)
        {
            Cursor.SetCursor(keyframes[i], clickPoint, CursorMode.Auto);
            yield return new WaitForSeconds(secs_per_frame);
        }
        Cursor.SetCursor(defaultCursor, clickPoint, CursorMode.Auto);
    }
    
    private void OnDestroy()
    {
        EventBus.Unsubscribe(reload_sub);
        EventBus.Unsubscribe(playerdamage_sub);

    }
}
