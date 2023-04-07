using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordUpgrade : Upgrade
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        thisData = typesData.types[(int)PurchaseableType.sword];
        base.Start();
    }

    protected override void Apply()
    {
        Debug.Log("Applying SwordUpgrade");
        HasSwordUpgrade upgrade = IsPlayer.instance.gameObject.AddComponent<HasSwordUpgrade>();
        upgrade.swingArc = thisData.rate1;
        upgrade.swingTime = thisData.rate2;

        base.Apply();
    }
}
