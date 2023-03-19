using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunBullet : Bullet
{
    public static float bulletSpeed = 6;

    protected override void Awake()
    {
        base.Awake();
        damage = -1;
        bulletLife = 1.5f;
    }
}
