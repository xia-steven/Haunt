using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon {
    protected GameObject wielder;
    protected GameObject basicBullet;

    protected override void Awake() {
        currentClipAmount = 8;
        fullClipAmount = 8;

        // Equip pistol on default
        // TODO: change this if necessary
        equipped = true;

        Subscribe();

        wielder = this.transform.parent.gameObject;
        basicBullet = Resources.Load<GameObject>("Prefabs/BasicBullet");
        BulletSettings();
    }

    protected virtual void BulletSettings()
    {
        basicBullet.GetComponent<Bullet>().SetShooter(Shooter.Player);
    }

    protected override void _OnFire(FireEvent e) {
        // Check if fire event comes from pistol holder
        if (e.shooter != wielder) {
            return;
        }

        if (equipped && currentClipAmount > 0) {
            // Fires basic bullet in direction pistol is facing


            /*
            Vector3 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            direction.y = 0;
            direction = direction.normalized;
            */

            // Get the screen position of the cursor
            Vector3 screenPos = Input.mousePosition;

            // Create a ray from the camera through the cursor position
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            // Find the point where the ray intersects the plane that contains the player
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            if (groundPlane.Raycast(ray, out float distanceToGround)) {
                // Calculate the direction vector from the player to the intersection point
                Vector3 hitPoint = ray.GetPoint(distanceToGround);
                Vector3 direction = (hitPoint - transform.position).normalized;

                Debug.Log(direction);

                FireProjectile(basicBullet, direction, transform, BasicBullet.bulletSpeed);
                // Give the player unlimited ammo for now
                //currentClipAmount--;
            }

            Debug.Log("Pistol ammo: " + currentClipAmount);
        }
    }

    protected override void _OnReload(ReloadEvent e) {
        // Check if reload event comes from pistol holder
        if (e.reloader != wielder) {
            return;
        }

        if (equipped) {
            ReloadInfinite();
            Debug.Log("Pistol ammo: " + currentClipAmount);
        }
    }

    private void Update() {
        // Get the screen position of the cursor
        Vector3 screenPos = Input.mousePosition;

        // Create a ray from the camera through the cursor position
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // Find the point where the ray intersects the plane that contains the player
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        if (groundPlane.Raycast(ray, out float distanceToGround)) {
            // Calculate the direction vector from the player to the intersection point
            Vector3 hitPoint = ray.GetPoint(distanceToGround);
            Vector3 direction = hitPoint - transform.position;

            // Calculate the rotation that points in the direction of the intersection point
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Set the rotation of the gun object
            transform.rotation = rotation;
        }
    }

}