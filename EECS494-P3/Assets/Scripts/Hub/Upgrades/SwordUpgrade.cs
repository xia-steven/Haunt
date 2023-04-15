using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordUpgrade : Upgrade {
    protected override void Start() {
        thisData = typesData.types[(int)PurchaseableType.sword];
        base.Start();
    }

    protected override void Apply() {
        var upgrade = IsPlayer.instance.gameObject.AddComponent<HasSwordUpgrade>();
        upgrade.swingArc = thisData.rate1;
        upgrade.swingTime = thisData.rate2;

        EventBus.Publish(new EquipSwordEvent());
        EventBus.Publish(new ActivateTeleporterEvent());

        base.Apply();
    }
}