using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectUpgrade : Upgrade {
    protected override string GetName() {
        return "DeflectDodge";
    }

    protected override void Start() {
        thisData = typesData.types[(int)PurchaseableType.dashReflect];
        base.Start();
    }

    protected override void Apply() {
        Debug.Log("Applying ReflectUpgrade");
        IsPlayer.instance.gameObject.AddComponent<HasReflectUpgrade>();

        base.Apply();
    }
}