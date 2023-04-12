using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RageUpgrade : Upgrade {
    protected override string GetName() {
        return "RageUpgrade";
    }

    protected override void Start() {
        thisData = typesData.types[(int)PurchaseableType.damageRage];
        base.Start();
    }

    protected override void Apply() {
        HasRageUpgrade newInstance = IsPlayer.instance.gameObject.AddComponent<HasRageUpgrade>() as HasRageUpgrade;
        newInstance.duration = thisData.duration;
        newInstance.dmgMod = thisData.rate1;
        newInstance.moveMod = thisData.rate2;

        base.Apply();
    }
}