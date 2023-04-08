using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTypesData : Savable {
    public List<WeaponData> types;
}

[System.Serializable]
public class WeaponData {
    public WeaponType type;
    public string name;
    public int fullClip;
    public float bulletDelay;
    public float tapDelay;
    public float screenShakeStrength;
    public float speedMultiplier;
    public float reloadTime;
}