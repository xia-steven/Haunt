using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsTabIcon : MonoBehaviour
{
    Subscription<WeaponPurchasedEvent> boughtWeaponSub;


    // Start is called before the first frame update
    void Start()
    {
        boughtWeaponSub = EventBus.Subscribe<WeaponPurchasedEvent>(onWeaponPurchase);


        if (PlayerModifiers.hasMultipleWeapons)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    private void OnDestroy()
    {
        EventBus.Unsubscribe(boughtWeaponSub);
    }

    void onWeaponPurchase(WeaponPurchasedEvent wpe)
    {
        PlayerModifiers.hasMultipleWeapons = true;
        gameObject.SetActive(true);
    }
}
