using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EliteArcherEnemy : ArcherEnemy {
    protected override void Start() {
        base.Start();

        Bullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/EliteArcherBullet");
    }

    public override int GetEnemyID() {
        return 3;
    }
}