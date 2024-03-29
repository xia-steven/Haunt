using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectUpgrade : Upgrade {
    protected override void Start() {
        thisData = typesData.types[(int)PurchaseableType.dashReflect];
        base.Start();
    }

    protected override void Apply() {
        IsPlayer.instance.gameObject.AddComponent<HasReflectUpgrade>();

        base.Apply();
    }
}