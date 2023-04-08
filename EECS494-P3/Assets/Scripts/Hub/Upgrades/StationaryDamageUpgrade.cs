using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryDamageUpgrade : Upgrade
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        thisData = typesData.types[(int)PurchaseableType.stationaryDamage];
        base.Start();
    }

    protected override void Apply()
    {
        HasStationaryDamage newInstance = IsPlayer.instance.gameObject.AddComponent<HasStationaryDamage>() as HasStationaryDamage;
        newInstance.holdTime = thisData.duration;
        newInstance.dmgMod = thisData.rate1;


        base.Apply();
    }
}
