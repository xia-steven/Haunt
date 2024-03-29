using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashDamageUpgrade : Upgrade {
    protected override void Start() {
        thisData = typesData.types[(int)PurchaseableType.doubleDashDamage];
        base.Start();
    }

    protected override void Apply() {
        HasDashDamageUpgrade newInstance =
            IsPlayer.instance.gameObject.AddComponent<HasDashDamageUpgrade>();
        newInstance.cooldown = thisData.duration;
        newInstance.dmgMod = thisData.rate1;


        base.Apply();
    }
}