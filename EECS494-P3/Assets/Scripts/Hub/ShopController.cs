using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    
    // All Available Shop Upgrades, set up in Unity Editor
    [SerializeField] private GameObject[] dayOneWeapons;
    [SerializeField] private GameObject[] dayTwoWeapons;
    [SerializeField] private GameObject[] dayThreeWeapons;
    [SerializeField] private GameObject restoreHealth;
    [SerializeField] private GameObject[] shieldUpgrades;
    [SerializeField] private GameObject[] unpurchasedWeapons;
    [SerializeField] private GameObject[] upgradePool;
    
    
    
    // pulled from GameControl to determine what's on sale
    private int day; 
    
    // Checked to see what weapons the player can buy on night 3
    private Inventory playerInventory;
 
    void Start()
    {
        day = GameControl.Day;
        playerInventory = IsPlayer.instance.gameObject.GetComponent<Inventory>();
        InitShop();
    }

    void InitShop()
    {
        switch (day)
        {
            case(1):
                DayOneShop();
                break;
            case(2):
                DayTwoShop();
                break;
            case(3):
                DayThreeShop();
                break;
        }
        
        
        
    }
    
    
    void DayOneShop()
    {
        foreach (GameObject weapon in dayOneWeapons)
        {
            weapon.SetActive(true);
        }

    }
    void DayTwoShop()
    {
        foreach (GameObject weapon in dayTwoWeapons)
        {
            weapon.SetActive(true);
        }
    }
    void DayThreeShop()
    {
        // determine which weapons should be available to the player based on what's in their inventory
        
    }

    void InitRandomUpgrades()
    {
        // don't offer same upgrade
        int range = upgradePool.Length;
        int iter1 = Random.Range(0, range);
        int iter2 = Random.Range(0, range);
        while (iter2 == iter1)
        {
            iter2 = Random.Range(0, range);
        }
        GameObject upgrade1 = upgradePool[Random.Range(0, range)];
        GameObject upgrade2 = upgradePool[Random.Range(0, range)];
        upgrade1.SetActive(true);
        upgrade2.SetActive(true);
    }
    
}
