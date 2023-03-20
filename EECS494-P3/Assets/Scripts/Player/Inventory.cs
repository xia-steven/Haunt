using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private int numWeapons = 0;
    private int currentWeapon = 0;
    private GameObject[] weapons = new GameObject[10];
    private GameObject pistol;
    private GameObject rifle;
    private GameObject shotgun;
    private int coins = 0;

    private Subscription<SwapEvent> swapEventSubscription;
    private Subscription<CoinEvent> coinEventSubscription;

    private void Awake()
    {
        swapEventSubscription = EventBus.Subscribe<SwapEvent>(_OnSwitchWeapon);
        coinEventSubscription = EventBus.Subscribe<CoinEvent>(_OnCoinChange);

                                // Equip all weapons (will be removed) TODO
                                rifle = Resources.Load<GameObject>("Prefabs/Weapons/Rifle");
        shotgun = Resources.Load<GameObject>("Prefabs/Weapons/Shotgun");
        Equip(rifle);
        Equip(shotgun);

        // Equip pistol on load
        pistol = Resources.Load<GameObject>("Prefabs/Weapons/Pistol");
        Equip(pistol);
    }

    public void Equip(GameObject weapon)
    {
        weapons[numWeapons] = Instantiate(weapon, transform);

        // Unequip current weapon and equip new weapon
        weapons[currentWeapon].gameObject.SetActive(false);
        weapons[numWeapons].gameObject.SetActive(true);

        currentWeapon = numWeapons;
        numWeapons++;
    }

    public void _OnSwitchWeapon(SwapEvent e)
    {
        Debug.Log("Swapping weapons");
        if (e.swapDirection > 0)
        {
            weapons[currentWeapon].gameObject.SetActive(false);
            currentWeapon++;
            if (currentWeapon >= numWeapons)
            {
                currentWeapon = 0;
            }
            weapons[currentWeapon].gameObject.SetActive(true);
        } else if (e.swapDirection < 1)
        {
            weapons[currentWeapon].gameObject.SetActive(false);
            currentWeapon--;
            if (currentWeapon < 0)
            {
                currentWeapon = numWeapons - 1;
            }
            weapons[currentWeapon].gameObject.SetActive(true);
        }
    }

    public int GetCoins()
    {
        return coins;
    }

    private void _OnCoinChange(CoinEvent e)
    {
        coins += e.coinValue;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<SwapEvent>(swapEventSubscription);
        EventBus.Unsubscribe<CoinEvent>(coinEventSubscription);
    }
}

public class SwapEvent {
    public int swapDirection;

    public SwapEvent(int _swapDirection)
    {
        swapDirection = _swapDirection;
    }
}
