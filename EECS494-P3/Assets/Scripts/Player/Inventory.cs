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
    private Subscription<SwapSpecificEvent> swapSpecificSubscription;
    private Subscription<CoinEvent> coinEventSubscription;
    private Subscription<ResetInventoryEvent> resetInventorySubscription;

    private void Awake()
    {
        swapEventSubscription = EventBus.Subscribe<SwapEvent>(_OnSwitchWeapon);
        coinEventSubscription = EventBus.Subscribe<CoinEvent>(_OnCoinChange);
        swapSpecificSubscription = EventBus.Subscribe<SwapSpecificEvent>(_OnSpecificSwap);
        resetInventorySubscription = EventBus.Subscribe<ResetInventoryEvent>(_OnResetInventory);

        // Equip pistol on load
        pistol = Resources.Load<GameObject>("Prefabs/Weapons/Pistol");
        Equip(pistol);

        // Equip all weapons (will be removed) TODO
        rifle = Resources.Load<GameObject>("Prefabs/Weapons/Rifle");
        shotgun = Resources.Load<GameObject>("Prefabs/Weapons/Shotgun");
        Equip(rifle);
        Equip(shotgun);

        // Swap to pistol on load (will be removed) TODO
        EventBus.Publish<SwapSpecificEvent>(new SwapSpecificEvent(1));
    }

    public void Equip(GameObject weapon)
    {
        weapons[numWeapons] = Instantiate(weapon, transform);

        // Unequip current weapon and equip new weapon
        weapons[currentWeapon].SetActive(false);
        weapons[numWeapons].SetActive(true);

        currentWeapon = numWeapons;
        numWeapons++;
    }

    public void _OnSwitchWeapon(SwapEvent e)
    {
        if (e.swapDirection > 0)
        {
            weapons[currentWeapon].SetActive(false);
            currentWeapon++;
            if (currentWeapon >= numWeapons)
            {
                currentWeapon = 0;
            }
            weapons[currentWeapon].SetActive(true);
        } else if (e.swapDirection < 1)
        {
            weapons[currentWeapon].SetActive(false);
            currentWeapon--;
            if (currentWeapon < 0)
            {
                currentWeapon = numWeapons - 1;
            }
            weapons[currentWeapon].SetActive(true);
        }
    }

    public void _OnSpecificSwap(SwapSpecificEvent e)
    {
        int newEquipped = e.newEquipped;
        int actualSlot = newEquipped - 1;

        // Check if weapon exists in slot
        if (weapons[actualSlot] != null)
        {
            // "Remove" currently equipped weapon
            weapons[currentWeapon].SetActive(false);

            // "Equip" new weapon based on input
            currentWeapon = actualSlot;
            weapons[currentWeapon].SetActive(true);
        } else
        {
            Debug.Log("Attempted equip of empty weapon slot.");
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

    private void _OnResetInventory(ResetInventoryEvent e) 
    {
        // Set all values back to zero
        numWeapons = 0;
        currentWeapon = 0;
        coins = 0;

        // Remove all weapons
        for (int i = 0; i < numWeapons; i++)
        {
            weapons[i] = null;
        }

        // Re-equip pistol
        Equip(pistol);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<SwapEvent>(swapEventSubscription);
        EventBus.Unsubscribe<CoinEvent>(coinEventSubscription);
        EventBus.Unsubscribe<SwapSpecificEvent>(swapSpecificSubscription);
        EventBus.Unsubscribe<ResetInventoryEvent>(resetInventorySubscription);
    }
}

public class SwapEvent {
    public int swapDirection;

    public SwapEvent(int _swapDirection)
    {
        swapDirection = _swapDirection;
    }
}

public class SwapSpecificEvent
{
    public int newEquipped;

    public SwapSpecificEvent(int _newEquipped)
    {
        newEquipped = _newEquipped;
    }
}