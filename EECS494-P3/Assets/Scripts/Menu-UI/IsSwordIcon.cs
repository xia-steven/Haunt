using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IsSwordIcon : MonoBehaviour
{
    [SerializeField] private Image sword;
    [SerializeField] private Color disabledColor;
    private bool equipped;

    private Subscription<SwordVisualEvent> swingEvent;
    private Subscription<EquipSwordEvent> equipEvent;

    void Start()
    {
        equipEvent = EventBus.Subscribe<EquipSwordEvent>(_OnEquip);
        swingEvent = EventBus.Subscribe<SwordVisualEvent>(_OnSwing);
    }

    private void _OnEquip(EquipSwordEvent e)
    {
        Debug.Log("Equip sword detected by sword icon");
        equipped = true;
        sword.enabled = true;
    }

    private void _OnSwing(SwordVisualEvent e)
    {
        if (equipped && e.started)
        {
            DisableSword();
        } else if (equipped && !e.started)
        {
            // Ensure sword visual returns to white before clicking again
            StartCoroutine(EnableSword(e.delay - 0.1f));
        }
    }

    private void DisableSword()
    {
        sword.color = disabledColor;
    }

    private IEnumerator EnableSword(float delay)
    {
        yield return new WaitForSeconds(delay);
        sword.color = Color.white;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(equipEvent);
        EventBus.Unsubscribe(swingEvent);
    }
}
