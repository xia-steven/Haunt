using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseableTypesData : Savable
{
    public List<UpgradeData> types;
}

[System.Serializable]
public class UpgradeData
{
    public PurchaseableType type;
    public int cost;
    public float rate1;
    public float rate2;
    public float duration;
    public string description;
    public int maxOwnable = 1;
}
