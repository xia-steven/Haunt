using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon {
    protected GameObject wielder;
    protected GameObject basicBullet;

    // Sprite of pistol - necessary so it can be flipped
    [SerializeField] protected GameObject pistolSprite;

    protected override void Awake() {
        base.Awake();

        currentClipAmount = 8;
        fullClipAmount = 8;
        reloadTime = 0.5f;
        type = "pistol";

        Subscribe();

        spriteRenderer = pistolSprite.GetComponent<SpriteRenderer>();
        wielder = this.transform.parent.gameObject;
        basicBullet = Resources.Load<GameObject>("Prefabs/Weapons/BasicBullet");
    }

    protected override void _OnFire(FireEvent e) {
        if (!gameObject.activeInHierarchy) return;

        // Check if fire event comes from pistol holder
        if (e.shooter != wielder) {
            return;
        }

        base._OnFire(e);
    }

    protected override void _OnReload(ReloadEvent e) {
        if (!gameObject.activeInHierarchy) return;

        // Check if reload event comes from pistol holder
        if (e.reloader != wielder) {
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

        EventBus.Publish<ReloadStartedEvent>(new ReloadStartedEvent(reloadTime * PlayerModifiers.reloadSpeed));
        yield return new WaitForSeconds(reloadTime * PlayerModifiers.reloadSpeed);

        // TODO: change to line up with inventory ammo
        ReloadInfinite();
        Debug.Log("Pistol ammo: " + currentClipAmount);
        isReloading = false;
    }

    protected override void WeaponFire(Vector3 direction)
    {
        direction.y = 0;
        direction = direction.normalized;

        FireProjectile(basicBullet, direction, transform, BasicBullet.bulletSpeed, Shooter.Player);
        // Give the player unlimited ammo for now
        currentClipAmount--;

        lastBullet = Time.time;
        lastTap = Time.time;
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