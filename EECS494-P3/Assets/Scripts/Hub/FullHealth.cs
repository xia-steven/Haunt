using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullHealth : IsBuyable
{
    protected override void Apply()
    {
        // overflowing the player health will ensure they get to max
        IsPlayer.instance.GetComponent<PlayerHasHealth>().AlterHealth(6);
    }
}
