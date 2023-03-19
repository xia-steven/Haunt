using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPistol : Pistol
{
    GameObject player;

    protected override void Awake()
    {
        lastBullet = Time.time;

        tapDelay = 3.0f;

        player = GameObject.Find("Player");

        // Don't subscribe to firing or reloading in enemy weapon awake
        basicBullet = Resources.Load<GameObject>("Prefabs/Weapons/BasicBullet");
        BulletSettings();
    }

    protected override void BulletSettings()
    {
        basicBullet.GetComponent<Bullet>().SetShooter(Shooter.Enemy);
    }

    private void Update()
    {
        // Fire bullets based on delays - tap is between bursts and bullet is between individual bullets
        // Enemy pistol fires one bullet every 3 seconds
        if (Time.time - lastBullet >= tapDelay)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0;
            direction = direction.normalized;
            Debug.Log(direction);

            FireProjectile(basicBullet, direction, transform, BasicBullet.bulletSpeed);

            lastBullet = Time.time;
        }
    }
}
