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
        Sniper,
        Launcher
    }

    [SerializeField] private WeaponType weapon;

    private int initialCost = 0;
    private Subscription<WeaponPurchasedEvent> weaponPurchasedSubscription;

    protected override void Awake()
    {

        if(GameControl.Day != 3)
        {
            cost = initialCost;
        }

        EventBus.Subscribe<WeaponPurchasedEvent>(_OnOtherWeaponPurchase);

        base.Awake();
    }
    
    protected virtual void Start()
    {
        if(weapon == WeaponType.Shotgun)
        {
            thisData = typesData.types[(int)PurchaseableType.shotgun];
        }
        else if (weapon == WeaponType.Rifle)
        {
            thisData = typesData.types[(int)PurchaseableType.minigun];
        }
        else if (weapon == WeaponType.Sniper)
        {
            thisData = typesData.types[(int)PurchaseableType.sniper];
        }
        else if (weapon == WeaponType.Sword)
        {
            thisData = typesData.types[(int)PurchaseableType.sword];
        } else if (weapon == WeaponType.Launcher)
        {
            thisData = typesData.types[(int)PurchaseableType.launcher];
        }

        if(GameControl.Day == 3)
        {
            cost = thisData.cost;
        }


        descriptionText.text = thisData.description;
        base.Start();
    }

    protected override void Apply()
    {
        GameObject weaponToEquip = Resources.Load<GameObject>("Prefabs/Weapons/" + weapon.ToString());
        EventBus.Publish(new WeaponPurchasedEvent(weaponToEquip));
        EventBus.Publish(new ActivateTeleporterEvent());
    }

    private void _OnOtherWeaponPurchase(WeaponPurchasedEvent e)
    {
        cost = thisData.cost;
    }

    protected override void OnDestroy()
    {
        EventBus.Unsubscribe<WeaponPurchasedEvent>(weaponPurchasedSubscription);

        base.OnDestroy();
    }

}
