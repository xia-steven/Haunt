using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpgrade : Upgrade {
    protected override void Start() {
        thisData = typesData.types[(int)PurchaseableType.speed];
        base.Start();
    }

    protected override void Apply() {
        PlayerModifiers.moveSpeed *= thisData.rate1;

        base.Apply();
    }
}