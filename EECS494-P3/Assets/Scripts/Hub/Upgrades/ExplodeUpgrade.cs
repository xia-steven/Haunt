using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeUpgrade : Upgrade
{
    protected override string GetName() {
        return "ExplodingDash";
    }

    protected override void Start()
    {
        thisData = typesData.types[(int)PurchaseableType.dashExplode];
        base.Start();
    }

    protected override void Apply()
    {
        HasExplodeUpgrade newInstance = IsPlayer.instance.gameObject.AddComponent<HasExplodeUpgrade>() as HasExplodeUpgrade;
        newInstance.explosiveRadius = thisData.rate1;

        base.Apply();
    }
}
