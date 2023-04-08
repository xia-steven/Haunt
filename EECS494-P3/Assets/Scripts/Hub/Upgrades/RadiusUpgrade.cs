using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusUpgrade : Upgrade
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        thisData = typesData.types[(int)PurchaseableType.radius];
        base.Start();
    }

    protected override void Apply()
    {
        PlayerModifiers.explosiveRadius = thisData.rate1;

        base.Apply();
    }
}
