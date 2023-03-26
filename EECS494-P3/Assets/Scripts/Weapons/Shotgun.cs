using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    protected GameObject wielder;
    protected GameObject shotgunBullet;
    protected SpriteRenderer spriteRenderer;

    [SerializeField] protected GameObject shotgunSprite;
    // Time between bullets
    [SerializeField] protected float bulletDelay = 0.85f;
    // Time between tap firing
    [SerializeField] protected float tapDelay = 0.8f;
    // Bullet spread in degrees
    [SerializeField] protected float spread = 30f;


    protected override void Awake()
    {
        base.Awake();

        currentClipAmount = 5;
        fullClipAmount = 5;
        reloadTime = 2.0f;

        Subscribe();

        spriteRenderer = shotgunSprite.GetComponent<SpriteRenderer>();
        wielder = this.transform.parent.gameObject;
        shotgunBullet = Resources.Load<GameObject>("Prefabs/Weapons/ShotgunBullet");
    }

    protected override void _OnFire(FireEvent e)
    {
        // Check if fire event comes from pistol holder
        if (e.shooter != wielder)
        {
            return;
        }

        base._OnFire(e);
    }

    protected override void _OnReload(ReloadEvent e)
    {
        // Check if reload event comes from pistol holder
        if (e.reloader != wielder)
        {
            return;
        }

        StartCoroutine(ReloadDelay());
    }

    private IEnumerator ReloadDelay()
    {
        isReloading = true;
        Debug.Log("Reloading");
        yield return new WaitForSeconds(reloadTime);

        // TODO: change to line up with inventory ammo
        ReloadInfinite();
        Debug.Log("Shotgun ammo: " + currentClipAmount);
        isReloading = false;
    }

    private void Update()
    {
        // Get the screen position of the cursor
        Vector3 screenPos = Input.mousePosition;
        Vector3 direction = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        // Create a ray from the camera through the cursor position
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // Find the point where the ray intersects the plane that contains the player
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        if (groundPlane.Raycast(ray, out float distanceToGround))
        {
            // Calculate the direction vector from the player to the intersection point
            Vector3 hitPoint = ray.GetPoint(distanceToGround);
            direction = hitPoint - transform.position;

            // Check if gun sprite needs to be flipped
            if (direction.x < 0)
            {
                spriteRenderer.flipY = true;
            }
            else
            {
                spriteRenderer.flipY = false;
            }

            // Calculate the rotation that points in the direction of the intersection point
            rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Set the rotation of the gun object
            transform.rotation = rotation;
        }

        // Fire bullet if ammo in clip, trigger is down, last bullet was not fired recently, last tap was not recent

        if (currentClipAmount > 0 && firing && (Time.time - lastBullet >= bulletDelay) && (Time.time - lastTap >= tapDelay) && !isReloading)
        {
            // Fires basic bullet in direction pistol is facing

            /*
            Vector3 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            direction.y = 0;
            direction = direction.normalized;
            */


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
            //currentClipAmount--;

            lastBullet = Time.time;
            lastTap = Time.time;
        }
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }
}
