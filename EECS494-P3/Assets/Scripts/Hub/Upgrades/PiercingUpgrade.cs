using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingUpgrade : Upgrade
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        thisData = typesData.types[(int)UpgradeType.piercingShot];
        base.Start();
    }

    protected override void Apply()
    {
        PlayerModifiers.maxPierce += thisData.rate1;
        base.Apply();
    }
}
