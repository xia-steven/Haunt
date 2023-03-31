using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpgrade : Upgrade
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        thisData = typesData.types[(int)UpgradeType.speed];
        base.Start();
    }

    protected override void Apply()
    {
        PlayerModifiers.moveSpeed *= thisData.rate1;

        base.Apply();
    }
}
