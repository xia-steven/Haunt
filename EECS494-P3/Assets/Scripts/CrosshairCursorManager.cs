using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CrosshairCursorManager: MonoBehaviour
{
    [SerializeField] Texture2D defaultCursor;
    [SerializeField] Texture2D[] reloadCursors;
    [SerializeField] Texture2D[] flashRedCursors;
    private Subscription<ReloadEvent> reload_sub;
    private Subscription<PlayerDamagedEvent> playerdamage_sub;
    private float reloadDuration = 1f;
    private float flashDuration = .25f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(defaultCursor, new Vector2(16,16), CursorMode.Auto);
        reload_sub = EventBus.Subscribe<ReloadEvent>(_OnReload);
        playerdamage_sub = EventBus.Subscribe<PlayerDamagedEvent>(_OnDamage);
    }

    // Update is called once per frame
    void Update()
    {

    }

    
    private void _OnReload(ReloadEvent e)
    {
        StartCoroutine(ReloadAnimation(reloadDuration));
        
    }

    private void _OnDamage(PlayerDamagedEvent e)
    {
        StartCoroutine(FlashRedAnimation(flashDuration));
    }
    private IEnumerator ReloadAnimation(float duration)
    {
        float frames = reloadDuration/(float)reloadCursors.Length;
        for (int i = 1; i < reloadCursors.Length; i++)
        {
            Cursor.SetCursor(reloadCursors[i], new Vector2(16,16), CursorMode.Auto);
            yield return new WaitForSeconds(frames);
        }
        Cursor.SetCursor(defaultCursor, new Vector2(16,16), CursorMode.Auto);
    }
    
    private IEnumerator FlashRedAnimation(float duration)
    {
        float frames = reloadDuration/(float)reloadCursors.Length;
        for (int i = 1; i < flashRedCursors.Length; i++)
        {
            Cursor.SetCursor(flashRedCursors[i], new Vector2(16,16), CursorMode.Auto);
            yield return new WaitForSeconds(frames);
        }
        Cursor.SetCursor(defaultCursor, new Vector2(16,16), CursorMode.Auto);
    }
    

    private void OnDestroy()
    {
        EventBus.Unsubscribe(reload_sub);
        EventBus.Unsubscribe(playerdamage_sub);

    }
}
