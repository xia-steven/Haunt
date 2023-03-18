using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPistol : Pistol
{
    protected override void _OnFire(FireEvent e)
    {
        Debug.Log("Enemy fire");
    }

    protected override void BulletSettings()
    {
        basicBullet.GetComponent<Bullet>().SetShooter(Shooter.Enemy);
    }
}
