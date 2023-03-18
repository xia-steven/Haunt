using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Most simple bullet
public class BasicBullet : Bullet {
    public static float bulletSpeed = 10;

    protected override void Awake() {
        base.Awake();
        damage = -1;
    }
}