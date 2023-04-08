using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player {
    public class Inventory : MonoBehaviour {
        private int numWeapons;
        private int currentWeapon;
        private GameObject[] weapons = new GameObject[10];
        private List<string> ownedWeapons;
        private GameObject pistol;
        private GameObject rifle;
        private GameObject shotgun;
        private GameObject sword;
        private GameObject sniper;
        private GameObject launcher;
        private int coins;

        private Subscription<SwapEvent> swapEventSubscription;
        private Subscription<SwapSpecificEvent> swapSpecificSubscription;
        private Subscription<CoinEvent> coinEventSubscription;
        private Subscription<ResetInventoryEvent> resetInventorySubscription;
        private Subscription<WeaponPurchasedEvent> weaponPurchasedSubscription;
        private Subscription<ReloadAllEvent> reloadAllSubscription;

        private void Awake() {
            if (IsPlayer.instance.gameObject != gameObject) return;

            swapEventSubscription = EventBus.Subscribe<SwapEvent>(_OnSwitchWeapon);
            coinEventSubscription = EventBus.Subscribe<CoinEvent>(_OnCoinChange);
            swapSpecificSubscription = EventBus.Subscribe<SwapSpecificEvent>(_OnSpecificSwap);
            resetInventorySubscription = EventBus.Subscribe<ResetInventoryEvent>(_OnResetInventory);
            weaponPurchasedSubscription = EventBus.Subscribe<WeaponPurchasedEvent>(_OnWeaponPurchase);
            reloadAllSubscription = EventBus.Subscribe<ReloadAllEvent>(_OnReloadAll);

            ownedWeapons = new List<string>();

            // Equip pistol on load
            pistol = Resources.Load<GameObject>("Prefabs/Weapons/Pistol");
            Equip(pistol);

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

        private void OnSceneLoad(Scene s, LoadSceneMode m) {
            EventBus.Publish(new SwapSpecificEvent(currentWeapon + 1));
        }

        private void Equip(GameObject weapon) {
            weapons[numWeapons] = Instantiate(weapon, transform);

            // Unequip current weapon and equip new weapon
            if (currentWeapon != numWeapons) weapons[currentWeapon].SetActive(false);
            weapons[numWeapons].SetActive(true);

            currentWeapon = numWeapons;
            numWeapons++;
            // Add weapon name to owned weapons
            ownedWeapons.Add(weapon.name);
        }

        private void _OnSwitchWeapon(SwapEvent e) {
            if (e.swapDirection > 0) {
                weapons[currentWeapon].SetActive(false);
                currentWeapon++;
                if (currentWeapon >= numWeapons)
                    currentWeapon = 0;
                weapons[currentWeapon].SetActive(true);
            }
            else if (e.swapDirection < 1) {
                weapons[currentWeapon].SetActive(false);
                currentWeapon--;
                currentWeapon = currentWeapon switch {
                    < 0 => numWeapons - 1,
                    _ => currentWeapon
                };

                weapons[currentWeapon].SetActive(true);
            }
        }

        private void _OnSpecificSwap(SwapSpecificEvent e) {
            var newEquipped = e.newEquipped;
            var actualSlot = newEquipped - 1;

            // Check if weapon exists in slot
            if (weapons[actualSlot] != null) {
                // "Remove" currently equipped weapon
                weapons[currentWeapon].SetActive(false);

                // "Equip" new weapon based on input
                currentWeapon = actualSlot;
                weapons[currentWeapon].SetActive(true);
            }
            else {
                Debug.Log("Attempted equip of empty weapon slot.");
            }
        }

        public int GetCoins() {
            return coins;
        }

        public List<string> GetCurrentWeapons() {
            return ownedWeapons;
        }

        private void _OnCoinChange(CoinEvent e) {
            coins += e.coinValue;
        }

        private void _OnResetInventory(ResetInventoryEvent e) {
            // Set all values back to zero
            numWeapons = 0;
            currentWeapon = 0;
            coins = 0;

            // Remove all weapons
            for (var i = 0; i < numWeapons; i++) weapons[i] = null;

            // Re-equip pistol
            Equip(pistol);
        }

        private void _OnReloadAll(ReloadAllEvent e) {
            for (var i = 0; i < numWeapons; i++) weapons[i].GetComponent<Weapon>().ReloadInfinite();
        }

        private void _OnWeaponPurchase(WeaponPurchasedEvent e) {
            Equip(e.weapon);
        }

        private void OnDestroy() {
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

        public SwapEvent(int _swapDirection) {
            swapDirection = _swapDirection;
        }
    }

    public class SwapSpecificEvent {
        public int newEquipped;

        public SwapSpecificEvent(int _newEquipped) {
            newEquipped = _newEquipped;
        }
    }
}