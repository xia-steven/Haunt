using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Weapon
{
    protected GameObject wielder;
    protected GameObject launcherShot;
    [SerializeField] protected float bulletSpread = 5f;

    // Sprite of launcher - necessary so it can be flipped
    [SerializeField] protected GameObject launcherSprite;

    protected override void Awake()
    {
        base.Awake();
        thisData = typesData.types[(int)WeaponType.launcher];

        SetData();
        currentClipAmount = fullClipAmount;

        Subscribe();

        spriteRenderer = launcherSprite.GetComponent<SpriteRenderer>();
        wielder = this.transform.parent.gameObject;
        launcherShot = Resources.Load<GameObject>("Prefabs/Weapons/LauncherShot");
    }

    protected override void _OnFire(FireEvent e)
    {
        if (!gameObject.activeInHierarchy) return;

        // Check if fire event comes from launcher holder
        if (e.shooter != wielder)
        {
            return;
        }

        base._OnFire(e);
    }

    protected override void _OnReload(ReloadEvent e)
    {
        if (!gameObject.activeInHierarchy) return;

        // Check if reload event comes from launcher holder
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

        EventBus.Publish(new ReloadStartedEvent(reloadTime * PlayerModifiers.reloadSpeed));
        yield return new WaitForSeconds(reloadTime * PlayerModifiers.reloadSpeed);

        // TODO: change to line up with inventory ammo
        ReloadInfinite();
        Debug.Log("Launcher ammo: " + currentClipAmount);
        isReloading = false;
    }

    protected override void WeaponFire(Vector3 direction)
    {
        direction.y = 0;
        direction = direction.normalized;

        Quaternion spread = Quaternion.AngleAxis(Random.Range(-bulletSpread, bulletSpread) / 2f, Vector3.up);

        Vector3 randomDirection = spread * direction;
        randomDirection = randomDirection.normalized;

        FireProjectile(launcherShot, randomDirection, transform, DemolitionProjectile.bulletSpeed, Shooter.Player);
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
