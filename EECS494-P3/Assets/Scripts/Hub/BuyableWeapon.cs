using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuyableWeapon : IsBuyable
{
    public enum WeaponType
    {
        Shotgun,
        Rifle,
        Sword,
        Sniper
    }

    [SerializeField] private WeaponType weapon;

    private int initialCost = 0;
    [SerializeField] int actualCost = 10;
    private Subscription<WeaponPurchasedEvent> weaponPurchasedSubscription;

    protected override void Awake()
    {
        // todo: figure out a way to set this to actual cost for the third night
        // could just send a weapon purchase event potentially from shopcontrol?
        cost = initialCost;
        EventBus.Subscribe<WeaponPurchasedEvent>(_OnOtherWeaponPurchase);

        base.Awake();
    }
    
    protected virtual void Start()
    {
        descriptionText.text = gameObject.name;
        base.Start();
    }

    protected override void Apply()
    {
        GameObject shotgun;
        GameObject weaponToEquip = Resources.Load<GameObject>("Prefabs/Weapons/" + weapon.ToString());
        EventBus.Publish(new WeaponPurchasedEvent(weaponToEquip));
    }

    private void _OnOtherWeaponPurchase(WeaponPurchasedEvent e)
    {
        cost = actualCost;
    }

    protected override void OnDestroy()
    {
        EventBus.Unsubscribe<WeaponPurchasedEvent>(weaponPurchasedSubscription);

        base.OnDestroy();
    }

}
