using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Upgrade : IsBuyable
{
    // fields that may be deleted later

    protected static UpgradeTypesHolder typesData;
    protected UpgradeData thisData;
    
    
    // Use awake in child classes to deal with subscriptions
    protected override void Awake()
    {
        if (typesData == null)
            typesData = ConfigManager.GetData<UpgradeTypesHolder>("UpgradeTypes");

        base.Awake();
    }

    // Use start in child classes to deal with thisData
    protected virtual void Start()
    {
        if (thisData != null)
        {
            descriptionText.text = thisData.description;
            cost = thisData.cost;
        }
            
        base.Start();
    }

}

public enum UpgradeType
{
    dashReflect = 0,        //dashing through incoming shots reflects them
    dashExplode = 1,        //dashes leave behind explosives
    damageRage = 2,         //extra damage and move speed after taking damage
    doubleDashDamage = 3,   //first shot after dash deals extra damage
    piercingShot = 4,       //shots pierce one more enemy
    speed = 5,              //extra speed
    stationaryDamage = 6,   //extra damage while standing still
    fastReload = 7          //faster reload time
}

