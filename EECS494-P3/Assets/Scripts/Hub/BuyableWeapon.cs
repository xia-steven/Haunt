using System;
using Events;
using UnityEngine;

namespace Hub {
    public class BuyableWeapon : IsBuyable {
        private enum WeaponType {
            Shotgun,
            Minigun,
            Sword,
            Sniper,
            Launcher
        }

        [SerializeField] private WeaponType weapon;

        private const int initialCost = 0;
        private Subscription<WeaponPurchasedEvent> weaponPurchasedSubscription;

        protected override void Awake() {
            if (Game_Control.GameControl.Day != 3) cost = initialCost;

            EventBus.Subscribe<WeaponPurchasedEvent>(_OnOtherWeaponPurchase);

            base.Awake();
        }

        protected new virtual void Start() {
            thisData = weapon switch {
                WeaponType.Shotgun =>
                    typesData.types[(int)PurchaseableType.shotgun],
                WeaponType.Minigun =>
                    typesData.types[(int)PurchaseableType.minigun],
                WeaponType.Sniper =>
                    typesData.types[(int)PurchaseableType.sniper],
                WeaponType.Sword =>
                    typesData.types[(int)PurchaseableType.sword],
                WeaponType.Launcher =>
                    typesData.types[(int)PurchaseableType.launcher],
                _ => throw new ArgumentOutOfRangeException()
            };

            cost = Game_Control.GameControl.Day switch {
                3 => thisData.cost,
                _ => cost
            };

            descriptionText.text = thisData.description;
            base.Start();
        }

        protected override void Apply() {
            var weaponToEquip = Resources.Load<GameObject>("Prefabs/Weapons/" + weapon.ToString());
            EventBus.Publish(new WeaponPurchasedEvent(weaponToEquip));
            EventBus.Publish(new ActivateTeleporterEvent());
        }

        private void _OnOtherWeaponPurchase(WeaponPurchasedEvent e) {
            cost = thisData.cost;
        }

        protected override void OnDestroy() {
            EventBus.Unsubscribe(weaponPurchasedSubscription);

            base.OnDestroy();
        }
    }
}