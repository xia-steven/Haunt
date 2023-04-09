using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    protected GameObject wielder;
    protected GameObject shotgunBullet;

    [SerializeField] protected GameObject shotgunSprite;
    // Bullet spread in degrees
    [SerializeField] protected float spread = 30f;


    protected override void Awake()
    {
        base.Awake();

        currentClipAmount = fullClipAmount;

        Subscribe();

        spriteRenderer = shotgunSprite.GetComponent<SpriteRenderer>();
        wielder = this.transform.parent.gameObject;
        shotgunBullet = Resources.Load<GameObject>("Prefabs/Weapons/ShotgunBullet");
    }

    protected override void Start()
    {
        base.Start();
        thisData = typesData.types[(int)WeaponType.shotgun];
        SetData();
    }

    protected override void _OnFire(FireEvent e)
    {
        if (!gameObject.activeInHierarchy) return;
        // Check if fire event comes from pistol holder
        if (e.shooter != wielder)
        {
            return;
        }

        base._OnFire(e);
    }

    protected override void _OnReload(ReloadEvent e)
    {
        if (!gameObject.activeInHierarchy) return;
        // Check if reload event comes from pistol holder
        if (e.reloader != wielder)
        {
            return;
        }

        if (CanReload())
        {
            StartCoroutine(ReloadDelay());
        }
    }

    private IEnumerator ReloadDelay()
    {
        isReloading = true;
        Debug.Log("Reloading");
        // Play reload sound
        AudioSource.PlayClipAtPoint(reloadSound, transform.position);

        EventBus.Publish<ReloadStartedEvent>(new ReloadStartedEvent(reloadTime * PlayerModifiers.reloadSpeed));
        yield return new WaitForSeconds(reloadTime * PlayerModifiers.reloadSpeed);

        // TODO: change to line up with inventory ammo
        ReloadInfinite();
        Debug.Log("Shotgun ammo: " + currentClipAmount);
        isReloading = false;
    }

    protected override void WeaponFire(Vector3 direction)
    {
        // Fires shotgun bullets in direction shotgun is facing

        direction.y = 0;
        Vector3 straight = direction.normalized;

        Quaternion rotation1 = Quaternion.AngleAxis(-spread / 2f, Vector3.up);
        Quaternion rotation2 = Quaternion.AngleAxis(-spread / 4f, Vector3.up);
        Quaternion rotation3 = Quaternion.AngleAxis(spread / 4f, Vector3.up);
        Quaternion rotation4 = Quaternion.AngleAxis(spread / 2f, Vector3.up);

        Vector3 bullet1 = rotation1 * direction;
        Vector3 bullet2 = rotation2 * direction;
        Vector3 bullet3 = rotation3 * direction;
        Vector3 bullet4 = rotation4 * direction;

        bullet1.Normalize();
        bullet2.Normalize();
        bullet3.Normalize();
        bullet4.Normalize();

        FireProjectile(shotgunBullet, straight, transform, ShotgunBullet.bulletSpeed, Shooter.Player);
        FireProjectile(shotgunBullet, bullet1, transform, ShotgunBullet.bulletSpeed, Shooter.Player);
        FireProjectile(shotgunBullet, bullet2, transform, ShotgunBullet.bulletSpeed, Shooter.Player);
        FireProjectile(shotgunBullet, bullet3, transform, ShotgunBullet.bulletSpeed, Shooter.Player);
        FireProjectile(shotgunBullet, bullet4, transform, ShotgunBullet.bulletSpeed, Shooter.Player);

        // Give the player unlimited ammo for now
        currentClipAmount--;

        lastBullet = Time.time;
        lastTap = Time.time;

        // Shake screen
        EventBus.Publish(new ScreenShakeEvent(screenShakeStrength));
    }

    protected override void GunReload()
    {
        if (CanReload())
        {
            StartCoroutine(ReloadDelay());
        }
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }
}
