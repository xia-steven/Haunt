using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    protected GameObject wielder;
    protected GameObject basicBullet;
    [SerializeField] protected float bulletSpread = 15f;

    [SerializeField] protected GameObject rifleSprite;

    protected override void Awake()
    {
        base.Awake();
        thisData = typesData.types[(int)WeaponType.rifle];

        SetData();
        currentClipAmount = fullClipAmount;

        Subscribe();

        spriteRenderer = rifleSprite.GetComponent<SpriteRenderer>();
        wielder = this.transform.parent.gameObject;
        basicBullet = Resources.Load<GameObject>("Prefabs/Weapons/MinigunBullet");
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
        Debug.Log("Rifle ammo: " + currentClipAmount);
        isReloading = false;
    }

    protected override void WeaponFire(Vector3 direction)
    {
        // Fires basic bullet in direction rifle is facing

        direction.y = 0;
        direction = direction.normalized;

        Quaternion spread = Quaternion.AngleAxis(Random.Range(-bulletSpread, bulletSpread) / 2f, Vector3.up);

        Vector3 randomDirection = spread * direction;
        randomDirection = randomDirection.normalized;

        FireProjectile(basicBullet, randomDirection, transform, BasicBullet.bulletSpeed, Shooter.Player);
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
