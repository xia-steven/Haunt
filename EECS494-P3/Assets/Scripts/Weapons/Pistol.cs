using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    private GameObject wielder;
    private GameObject basicBullet;

    protected override void Awake()
    {
        currentClipAmount = 8;
        fullClipAmount = 8;
        
        // Equip pistol on default
        // TODO: change this if necessary
        equipped = true;

        Subscribe();

        wielder = this.transform.parent.gameObject;
        basicBullet = Resources.Load<GameObject>("Prefabs/BasicBullet");
    }

    protected override void _OnFire(FireEvent e)
    {
        // Check if fire event comes from pistol holder
        if (e.shooter != wielder)
        {
            return;
        }

        if (equipped && currentClipAmount > 0)
        {
            // Fires basic bullet in direction pistol is facing

            Vector3 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            direction.z = 0;
            direction = direction.normalized;

            Debug.Log(direction);

            FireProjectile(basicBullet, direction, transform, BasicBullet.bulletSpeed);
            currentClipAmount--;

            Debug.Log("Pistol ammo: " + currentClipAmount);
        }
    }

    protected override void _OnReload(ReloadEvent e)
    {
        // Check if reload event comes from pistol holder
        if (e.reloader != wielder)
        {
            return;
        }

        if (equipped)
        {
            ReloadInfinite();
            Debug.Log("Pistol ammo: " + currentClipAmount);
        }
    }

    private void Update()
    {
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 direction = cursorPosition - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
}
