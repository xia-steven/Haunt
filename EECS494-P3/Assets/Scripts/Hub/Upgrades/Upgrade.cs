using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Upgrade : IsBuyable {
    // Use start in child classes to deal with thisData
    protected virtual void Start() {
        if (thisData != null) {
            descriptionText.text = thisData.description;
            cost = thisData.cost;
        }

        base.Start();
    }

    protected virtual string GetName() {
        return "";
    }

    protected override void Apply() {
        base.Apply();
        ShopController.upgradePool.Remove(GetName());
    }
}