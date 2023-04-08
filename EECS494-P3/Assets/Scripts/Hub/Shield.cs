using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : IsBuyable {
    protected override void Apply() {
        IsPlayer.instance.GetComponent<PlayerHasHealth>().AddShield();
    }
}