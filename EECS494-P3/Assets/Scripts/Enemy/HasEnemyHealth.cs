using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasEnemyHealth : HasHealth
{
    public override bool AlterHealth(int healthDelta)
    {
        base.AlterHealth(healthDelta);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        return true;
    }
}
