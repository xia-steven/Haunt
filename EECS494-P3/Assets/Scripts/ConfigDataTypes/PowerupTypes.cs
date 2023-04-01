using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTypesHolder : Savable
{
    public List<UpgradeData> types;
}

[System.Serializable]
public class UpgradeData
{
    public UpgradeType type;
    public int cost;
    public float rate1;
    public float rate2;
    public float duration;
    public string description;
    public int maxOwnable = 1;
}
