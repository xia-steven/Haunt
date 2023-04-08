using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPistol : Pistol {
    Vector3 playerPos;

    protected override void Awake() {
        shotByPlayer = false;
        playerPos = new Vector3();

        basicBullet = Resources.Load<GameObject>("Prefabs/Weapons/EnemyBasicBullet");

        lastBullet = Time.time;
    }

    private void Update() {
        playerPos = IsPlayer.instance.transform.position;

        // Fire bullets based on delays - tap is between bursts and bullet is between individual bullets
        // Enemy pistol fires one bullet every 3 seconds
        if (Time.time - lastBullet >= tapDelay) {
            Vector3 direction = playerPos - transform.position;
            direction.y = 0;
            direction = direction.normalized;

            FireProjectile(basicBullet, direction, transform, EnemyBasicBullet.bulletSpeed, Shooter.Enemy);

            lastBullet = Time.time;
        }
    }
}