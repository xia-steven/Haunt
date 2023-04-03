using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    
    // All Available Shop Upgrades, set up in Unity Editor
    [SerializeField] private GameObject[] upgradePool;
    [SerializeField] private GameObject weaponTableRight;
    [SerializeField] private GameObject weaponTableLeft;
    [SerializeField] private GameObject topUpgradeTable;
    [SerializeField] private GameObject sideUpgradeTable;
    [SerializeField] GameObject healthRestore;
    [SerializeField] GameObject shield;

    private GameObject shotgunPrefab;
    private GameObject minigunPrefab;
    private GameObject sniperPrefab;
    private GameObject swordPrefab;

    
    // pulled from GameControl to determine what's on sale
    private int day; 
    
    // Checked to see what weapons the player can buy on night 3
    private Inventory playerInventory;
 
    void Start()
    {
        // load weapon purchasables
        shotgunPrefab = Resources.Load<GameObject>("Prefabs/Hub/Shotgun");
        minigunPrefab = Resources.Load<GameObject>("Prefabs/Hub/Minigun");
        sniperPrefab = Resources.Load<GameObject>("Prefabs/Hub/Sniper");
        swordPrefab = Resources.Load<GameObject>("Prefabs/Hub/Sword");
        day = GameControl.Day;
        playerInventory = IsPlayer.instance.gameObject.GetComponent<Inventory>();
        InitShop();
    }

    void InitShop()
    {
        switch (day)
        {
            // Tutorial day is 0
            case(0):
                healthRestore.SetActive(false);
                shield.SetActive(false);
                break;
            case(1):
                DayOneShop();
                InitRandomUpgrades();
                break;
            case(2):
                DayTwoShop();
                InitRandomUpgrades();
                break;
            case(3):
                DayThreeShop();
                InitRandomUpgrades();
                break;
        }
    }
    
    
    void DayOneShop()
    {
        GameObject shotgun = Instantiate(shotgunPrefab);
        GameObject sniper = Instantiate(sniperPrefab);
        shotgun.transform.SetParent( weaponTableLeft.transform, false);
        sniper.transform.SetParent( weaponTableRight.transform, false);
    }
    void DayTwoShop()
    {
        GameObject minigun = Instantiate(minigunPrefab);
        GameObject sword = Instantiate(swordPrefab);
        minigun.transform.SetParent( weaponTableLeft.transform, false);
        sword.transform.SetParent( weaponTableRight.transform, false);
    }
    void DayThreeShop()
    {
        // todo determine which weapons should be available to the player based on what's in their inventory
        
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
        GameObject upgrade1 = Instantiate(upgradePool[iter1], topUpgradeTable.transform, false);
        GameObject upgrade2 = Instantiate(upgradePool[iter2], sideUpgradeTable.transform, false);
    }
    
}
