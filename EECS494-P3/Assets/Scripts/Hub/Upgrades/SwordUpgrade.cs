using Events;
using Player;
using UnityEngine;

namespace Hub.Upgrades {
    public class SwordUpgrade : Upgrade {
        protected override void Start() {
            thisData = typesData.types[(int)PurchaseableType.sword];
            base.Start();
        }

        protected override void Apply() {
            Debug.Log("Applying SwordUpgrade");
            var upgrade = IsPlayer.instance.gameObject.AddComponent<HasSwordUpgrade>();
            upgrade.swingArc = thisData.rate1;
            upgrade.swingTime = thisData.rate2;

            EventBus.Publish(new ActivateTeleporterEvent());

            base.Apply();
        }
    }
}