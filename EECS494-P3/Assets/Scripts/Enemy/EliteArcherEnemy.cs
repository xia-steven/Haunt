using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EliteArcherEnemy : ArcherEnemy {
    protected override void Start() {
        base.Start();
        projectileSpeed = 5.5f;
        Bullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/EliteArcherBullet");
    }

    public override int GetEnemyID() {
        return 3;
    }
}