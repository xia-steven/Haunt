using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingUpgrade : Upgrade
{
    protected override string GetName() {
        return "BulletPierce";
    }

    protected override void Start()
    {
        thisData = typesData.types[(int)PurchaseableType.piercingShot];
        base.Start();
    }

    protected override void Apply()
    {
        PlayerModifiers.maxPierce += thisData.rate1;
        base.Apply();
    }
}
