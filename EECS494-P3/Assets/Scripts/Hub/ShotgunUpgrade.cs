using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShotgunUpgrade : Upgrade
{
    private int initialCost = 0;
    private int actualCost = 10;
    private Subscription<WeaponPurchasedEvent> weaponPurchasedSubscription;

    void Start()
    {
        // todo: figure out a way to set this to actual cost for the third night
        // could just send a weapon purchase event potentially from shopcontrol?
        cost = initialCost;
        EventBus.Subscribe<WeaponPurchasedEvent>(_OnOtherWeaponPurchase);
    }
    protected override void ApplyUpgrade()
    {
        GameObject shotgun;
        shotgun = Resources.Load<GameObject>("Prefabs/Weapons/Shotgun");
        EventBus.Publish(new WeaponPurchasedEvent(shotgun));
        Destroy(gameObject);
    }

    private void _OnOtherWeaponPurchase(WeaponPurchasedEvent e)
    {
        cost = actualCost;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<WeaponPurchasedEvent>(weaponPurchasedSubscription);

    }

}
