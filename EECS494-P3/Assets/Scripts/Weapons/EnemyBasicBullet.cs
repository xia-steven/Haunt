using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Most simple enemy bullet
public class EnemyBasicBullet : Bullet
{
    public static float bulletSpeed = 4;

    protected override void Awake()
    {
        base.Awake();
        damage = -1;
        bulletLife = 3.0f;
    }
}