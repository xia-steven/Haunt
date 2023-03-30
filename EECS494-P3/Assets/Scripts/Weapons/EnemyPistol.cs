using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPistol : Pistol
{
    GameObject player;


    protected override void Awake()
    {
        isPlayer = false;
    }

    
}
