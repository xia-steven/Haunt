using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadUpgrade : Upgrade
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        thisData = typesData.types[(int)UpgradeType.fastReload];
        base.Start();
    }

    protected override void Apply()
    {
        PlayerModifiers.reloadSpeed /= thisData.rate1;

        base.Apply();
    }
}
