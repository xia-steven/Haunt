using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    protected GameObject wielder;
    protected GameObject basicBullet;

    // Time between bullets
    [SerializeField] protected float bulletDelay = 0.2f;
    // Time between tap firing
    [SerializeField] protected float tapDelay = 0.1f;

    protected override void Awake()
    {
        base.Awake();

        currentClipAmount = 30;
        fullClipAmount = 30;

        Subscribe();

        wielder = this.transform.parent.gameObject;
        basicBullet = Resources.Load<GameObject>("Prefabs/Weapons/BasicBullet");
        BulletSettings();
    }

    protected virtual void BulletSettings()
    {
        basicBullet.GetComponent<Bullet>().SetShooter(Shooter.Player);
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

        // TODO: change to line up with inventory ammo
        ReloadInfinite();
        Debug.Log("Rifle ammo: " + currentClipAmount);
    }

    private void Update()
    {
        // Get the screen position of the cursor
        Vector3 screenPos = Input.mousePosition;
        Vector3 direction = Vector3.zero;

        // Create a ray from the camera through the cursor position
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // Find the point where the ray intersects the plane that contains the player
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        if (groundPlane.Raycast(ray, out float distanceToGround))
        {
            // Calculate the direction vector from the player to the intersection point
            Vector3 hitPoint = ray.GetPoint(distanceToGround);
            direction = hitPoint - transform.position;

            // Calculate the rotation that points in the direction of the intersection point
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Set the rotation of the gun object
            transform.rotation = rotation;
        }

        // Fire bullet if ammo in clip, trigger is down, last bullet was not fired recently, last tap was not recent

        if (currentClipAmount > 0 && firing && (Time.time - lastBullet >= bulletDelay) && (Time.time - lastTap >= tapDelay))
        {
            // Fires basic bullet in direction pistol is facing

            /*
            Vector3 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            direction.y = 0;
            direction = direction.normalized;
            */

            direction.y = 0;
            direction = direction.normalized;

            FireProjectile(basicBullet, direction, transform, BasicBullet.bulletSpeed);
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