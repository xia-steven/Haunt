using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RageUpgrade : Upgrade
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        thisData = typesData.types[(int)UpgradeType.damageRage];
        base.Start();
    }

    protected override void Apply()
    {
        HasRageUpgrade newInstance = IsPlayer.instance.gameObject.AddComponent<HasRageUpgrade>() as HasRageUpgrade;
        newInstance.duration = thisData.duration;
        newInstance.dmgMod = thisData.rate1;
        newInstance.moveMod = thisData.rate2;

        base.Apply();
    }
}
