using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour {
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

    private void Start() {
        // load weapon purchasables
        shotgunPrefab = Resources.Load<GameObject>("Prefabs/Hub/Shotgun");
        minigunPrefab = Resources.Load<GameObject>("Prefabs/Hub/Minigun");
        sniperPrefab = Resources.Load<GameObject>("Prefabs/Hub/Sniper");
        swordPrefab = Resources.Load<GameObject>("Prefabs/Hub/Sword");
        day = GameControl.Day;
        playerInventory = IsPlayer.instance.gameObject.GetComponent<Inventory>();
        InitShop();
    }

    private void InitShop() {
        switch (day) {
            // Tutorial day is 0
            case 0:
                //healthRestore.SetActive(false);
                //shield.SetActive(false);
                DayZeroShop();
                InitRandomUpgrades();
                break;
            case 1:
                DayOneShop();
                InitRandomUpgrades();
                break;
            case 2:
                DayTwoShop();
                InitRandomUpgrades();
                break;
            case 3:
                DayThreeShop();
                InitRandomUpgrades();
                break;
        }
    }

    private void DayZeroShop() {
        var sword = Instantiate(swordPrefab);
        sword.transform.SetParent(weaponTableRight.transform, false);
    }

    private void DayOneShop() {
        var shotgun = Instantiate(shotgunPrefab);
        var sniper = Instantiate(sniperPrefab);
        shotgun.transform.SetParent(weaponTableLeft.transform, false);
        sniper.transform.SetParent(weaponTableRight.transform, false);
    }

    private void DayTwoShop() {
        var minigun = Instantiate(minigunPrefab);
        // TODO Replace with bazooka
        var sword = Instantiate(swordPrefab);
        minigun.transform.SetParent(weaponTableLeft.transform, false);
        sword.transform.SetParent(weaponTableRight.transform, false);
    }

    private void DayThreeShop() {
        // todo determine which weapons should be available to the player based on what's in their inventory
        var currWeapons = playerInventory.GetCurrentWeapons();

        var possibleWeapons = new List<GameObject>
            { shotgunPrefab, sniperPrefab, minigunPrefab, swordPrefab };

        foreach (var currWeapon in currWeapons) {
            switch (currWeapon) {
                case "Rifle":
                    possibleWeapons.Remove(minigunPrefab);
                    break;
                case "Sword":
                    possibleWeapons.Remove(swordPrefab);
                    break;
                case "Shotgun":
                    possibleWeapons.Remove(shotgunPrefab);
                    break;
                case "Sniper":
                    possibleWeapons.Remove(sniperPrefab);
                    break;
            }
        }

        var firstIndex = Random.Range(0, possibleWeapons.Count);

        if (possibleWeapons.Count > 0) {
            var firstWeapon = Instantiate(possibleWeapons[firstIndex]);
            firstWeapon.transform.SetParent(weaponTableLeft.transform, false);
        }

        if (possibleWeapons.Count > 1) {
            var secondIndex = Random.Range(0, possibleWeapons.Count);
            // Make sure different weapon
            while (secondIndex == firstIndex) {
                secondIndex = Random.Range(0, possibleWeapons.Count);
            }

            var secondWeapon = Instantiate(possibleWeapons[secondIndex]);
            secondWeapon.transform.SetParent(weaponTableRight.transform, false);
        }
    }

    private void InitRandomUpgrades() {
        // don't offer same upgrade
        var range = upgradePool.Length;
        var iter1 = Random.Range(0, range);
        var iter2 = Random.Range(0, range);
        while (iter2 == iter1) {
            iter2 = Random.Range(0, range);
        }

        Instantiate(upgradePool[iter1], topUpgradeTable.transform, false);
        Instantiate(upgradePool[iter2], sideUpgradeTable.transform, false);
    }
}