using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//defunct
public class HealthUpgrade : Upgrade
{
    /*
    protected override void ApplyUpgrade()
    {
        // we do it this way (reaching directly into PlayerHasHealth code)
        // because the UI system which responds to the increase event DEPENDS
        // on PlayerHasHealth having the correct health/maxhealth to display
        
        // if we JUST depended on communicating with both components via event,
        // the ui might try to update the pips before the HasHealth component
        // actually updated the health numbers
        GameObject.Find("Player").GetComponent<PlayerHasHealth>().AddShield();
    }
    */
}
