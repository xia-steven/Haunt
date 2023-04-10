using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour
{
    private int numWeapons = 0;
    private int currentWeapon = 0;
    private GameObject[] weapons = new GameObject[10];
    private List<string> ownedWeapons;
    private GameObject pistol;
    private GameObject rifle;
    private GameObject shotgun;
    private GameObject sword;
    private GameObject sniper;
    private GameObject launcher;
    private int coins = 0;
    private AudioClip weaponSwapSound;

    private Subscription<SwapEvent> swapEventSubscription;
    private Subscription<SwapSpecificEvent> swapSpecificSubscription;
    private Subscription<CoinEvent> coinEventSubscription;
    private Subscription<ResetInventoryEvent> resetInventorySubscription;
    private Subscription<WeaponPurchasedEvent> weaponPurchasedSubscription;
    private Subscription<ReloadAllEvent> reloadAllSubscription;

    private void Awake()
    {
        if (IsPlayer.instance.gameObject != gameObject) return;

        swapEventSubscription = EventBus.Subscribe<SwapEvent>(_OnSwitchWeapon);
        coinEventSubscription = EventBus.Subscribe<CoinEvent>(_OnCoinChange);
        swapSpecificSubscription = EventBus.Subscribe<SwapSpecificEvent>(_OnSpecificSwap);
        resetInventorySubscription = EventBus.Subscribe<ResetInventoryEvent>(_OnResetInventory);
        weaponPurchasedSubscription = EventBus.Subscribe<WeaponPurchasedEvent>(_OnWeaponPurchase);
        reloadAllSubscription = EventBus.Subscribe<ReloadAllEvent>(_OnReloadAll);

        weaponSwapSound = Resources.Load<AudioClip>("Audio/Weapons/weapswitch");

        ownedWeapons = new List<string>();

        // Equip pistol on load
        pistol = Resources.Load<GameObject>("Prefabs/Weapons/Pistol");
        InitialEquip(pistol);

        // Equip all weapons (will be removed) TODO
        // rifle = Resources.Load<GameObject>("Prefabs/Weapons/Rifle");
        // shotgun = Resources.Load<GameObject>("Prefabs/Weapons/Shotgun");
        // sword = Resources.Load<GameObject>("Prefabs/Weapons/Sword");
        // sniper = Resources.Load<GameObject>("Prefabs/Weapons/Sniper");
        // launcher = Resources.Load<GameObject>("Prefabs/Weapons/Launcher");
        // Equip(rifle);
        // Equip(shotgun);
        // Equip(sword);
        // Equip(sniper);
        // Equip(launcher);

        // Swap to pistol on load (will be removed) TODO
        //EventBus.Publish<SwapSpecificEvent>(new SwapSpecificEvent(1));

        SceneManager.sceneLoaded += OnSceneLoad;
    }

    void OnSceneLoad(Scene s, LoadSceneMode m)
    {
        SwapSpecificEvent swapEvent = new SwapSpecificEvent(currentWeapon+1);
        swapEvent.newScene = true;
        EventBus.Publish(swapEvent);
    }

    public void Equip(GameObject weapon)
    {
        bool wasMessage = false;

        weapons[numWeapons] = Instantiate(weapon, transform);

        // Weapon was already equipped
        if (numWeapons > 0)
            wasMessage = weapons[currentWeapon].GetComponent<Weapon>().messageVisible;

        // Unequip current weapon and equip new weapon
        if (currentWeapon != numWeapons) weapons[currentWeapon].SetActive(false);
        weapons[numWeapons].SetActive(true);

        // Set new weapon to appropriate message existance
        if (wasMessage)
            weapons[numWeapons].GetComponent<Weapon>().messageVisible = true;

        // Play swap sound
        AudioSource.PlayClipAtPoint(weaponSwapSound, transform.position);

        currentWeapon = numWeapons;
        numWeapons++;
        // Add weapon name to owned weapons
        ownedWeapons.Add(weapon.name);
    }

    /// <summary>
    /// Only to be called from within Inventory start/awake
    /// Different as it will not play equipping sound
    /// </summary>
    /// <param name="weapon"></param>
    private void InitialEquip(GameObject weapon)
    {
        weapons[numWeapons] = Instantiate(weapon, transform);

        // Unequip current weapon and equip new weapon
        if (currentWeapon != numWeapons) weapons[currentWeapon].SetActive(false);
        weapons[numWeapons].SetActive(true);

        currentWeapon = numWeapons;
        numWeapons++;
        // Add weapon name to owned weapons
        ownedWeapons.Add(weapon.name);
    }

    public void _OnSwitchWeapon(SwapEvent e)
    {
        // Can't swap with only 1 or 0 weapons
        if (numWeapons <= 1)
            return;

        bool wasMessage = weapons[currentWeapon].GetComponent<Weapon>().messageVisible;

        // Play swap sound
        AudioSource.PlayClipAtPoint(weaponSwapSound, transform.position);
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

        weapons[currentWeapon].GetComponent<Weapon>().messageVisible = wasMessage;
    }

    public void _OnSpecificSwap(SwapSpecificEvent e)
    {
        // Can't swap with only 1 or 0 weapons
        if (e.newScene)
        {
            weapons[currentWeapon].GetComponent<Weapon>().messageVisible = false;
        }

        bool wasMessage = weapons[currentWeapon].GetComponent<Weapon>().messageVisible;
        Debug.Log(wasMessage + " - " + e.newScene);
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

            if (e.newScene)
            {
                weapons[currentWeapon].GetComponent<Weapon>().messageVisible = false;
            }
            else
            {
                weapons[currentWeapon].GetComponent<Weapon>().messageVisible = wasMessage;
                // Play swap sound - placed here so not played on scene load
                if(numWeapons > 1)
                {
                    AudioSource.PlayClipAtPoint(weaponSwapSound, transform.position);
                }
            }
        } else
        {
            Debug.Log("Attempted equip of empty weapon slot.");
        }
    }

    public int GetCoins()
    {
        return coins;
    }

    public List<string> GetCurrentWeapons()
    {
        return ownedWeapons;
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

    private void _OnReloadAll(ReloadAllEvent e)
    {
        for (int i = 0; i < numWeapons; i++)
        {
            weapons[i].GetComponent<Weapon>().ReloadInfinite();
        }
    }

    private void _OnWeaponPurchase(WeaponPurchasedEvent e)
    {
        Equip(e.weapon);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(swapEventSubscription);
        EventBus.Unsubscribe(coinEventSubscription);
        EventBus.Unsubscribe(swapSpecificSubscription);
        EventBus.Unsubscribe(resetInventorySubscription);
        EventBus.Unsubscribe(weaponPurchasedSubscription);
        EventBus.Unsubscribe(reloadAllSubscription);
        
        SceneManager.sceneLoaded -= OnSceneLoad;
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
    public bool newScene = false;

    public SwapSpecificEvent(int _newEquipped)
    {
        newEquipped = _newEquipped;
    }
}