using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour {
    // All Available Shop Upgrades, set up in Unity Editor
    public static List<string> upgradePool = new() {
        "DashDamageUpgrade", "BulletPierce", "DeflectDodge", "ExplodingDash", "RageUpgrade", "ReloadUpgrade",
        "SpeedUpgrade", "StationaryUpgrade"
    };

    [SerializeField] private GameObject weaponTableRight;
    [SerializeField] private GameObject weaponTableLeft;
    [SerializeField] private GameObject topUpgradeTable;
    [SerializeField] private GameObject sideUpgradeTable;
    [SerializeField] private GameObject healthRestore;
    [SerializeField] private GameObject shield;

    private GameObject shotgunPrefab;
    private GameObject minigunPrefab;
    private GameObject sniperPrefab;
    private GameObject swordPrefab;
    private GameObject launcherPrefab;


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
        launcherPrefab = Resources.Load<GameObject>("Prefabs/Hub/Launcher");
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
        var sword = Instantiate(swordPrefab, weaponTableRight.transform, false);
    }

    private void DayOneShop() {
        var shotgun = Instantiate(shotgunPrefab, weaponTableLeft.transform, false);
        var sniper = Instantiate(sniperPrefab, weaponTableRight.transform, false);
    }

    private void DayTwoShop() {
        var minigun = Instantiate(minigunPrefab, weaponTableLeft.transform, false);
        var launcher = Instantiate(launcherPrefab, weaponTableRight.transform, false);
    }

    private void DayThreeShop() {
        var currWeapons = playerInventory.GetCurrentWeapons();

        var possibleWeapons = new List<GameObject>
            { shotgunPrefab, sniperPrefab, minigunPrefab, launcherPrefab };

        foreach (var t in currWeapons) {
            switch (t) {
                case "Rifle":
                    possibleWeapons.Remove(minigunPrefab);
                    break;
                case "Launcher":
                    possibleWeapons.Remove(launcherPrefab);
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
            var firstWeapon = Instantiate(possibleWeapons[firstIndex], weaponTableLeft.transform, false);
        }

        if (possibleWeapons.Count > 1) {
            var secondIndex = Random.Range(0, possibleWeapons.Count);
            // Make sure different weapon
            while (secondIndex == firstIndex) {
                secondIndex = Random.Range(0, possibleWeapons.Count);
            }

            var secondWeapon = Instantiate(possibleWeapons[secondIndex], weaponTableRight.transform, false);
        }
    }

    private void InitRandomUpgrades() {
        if (upgradePool.Count == 0) {
            return;
        }

        // don't offer same upgrade
        var range = upgradePool.Count;
        var iter1 = Random.Range(0, range);
        var iter2 = Random.Range(0, range);
        while (iter2 == iter1) {
            iter2 = Random.Range(0, range);
        }

        var upgrade1 = Instantiate(Resources.Load<GameObject>("Prefabs/Hub/" + upgradePool[iter1]),
            topUpgradeTable.transform, false);
        var upgrade2 = Instantiate(Resources.Load<GameObject>("Prefabs/Hub/" + upgradePool[iter2]),
            sideUpgradeTable.transform, false);
        upgradePool.Remove(upgradePool[iter1]);
        upgradePool.Remove(upgradePool[iter2]);
    }
}