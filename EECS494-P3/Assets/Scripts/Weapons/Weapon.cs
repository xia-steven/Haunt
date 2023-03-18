using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Weapon base class

public abstract class Weapon : MonoBehaviour {
    // Number of bullets in one clip - necessary for all guns
    protected int fullClipAmount = 8;

    // Currently loaded bullets
    protected int currentClipAmount;

    // Bool to keep track if current weapon is equipped - this is useful for knowing when to reload and fire specific weapons
    protected bool equipped = false;

    protected Subscription<FireEvent> fireEventSubscription;
    protected Subscription<ReloadEvent> reloadEventSubscription;


    protected virtual void Awake() {
        currentClipAmount = fullClipAmount;
    }

    protected void Subscribe() {
        fireEventSubscription = EventBus.Subscribe<FireEvent>(_OnFire);
        reloadEventSubscription = EventBus.Subscribe<ReloadEvent>(_OnReload);
    }

    protected virtual void _OnFire(FireEvent e) {
        Debug.Log("Base fire called");
    }

    protected virtual void _OnReload(ReloadEvent e) {
        Debug.Log("Base reload called");
    }

    // Fires a projectile of type Bullet in specified direction
    public void FireProjectile(GameObject bullet, Vector3 direction, Transform start, float bulletSpeed) {
        GameObject projectile = Instantiate(bullet, start.position, Quaternion.identity);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = direction * bulletSpeed;
    }

    // Reloads gun with specified number of bullets
    // Returns number of bullets returned to inventory
    public int Reload(int bulletsLoaded) {
        int returned = 0;
        int loaded = currentClipAmount + bulletsLoaded;

        if (loaded > fullClipAmount) // Reload would exceed clip capacity
        {
            returned = loaded - fullClipAmount; // Set excess bullets to be returned to inventory

            currentClipAmount = fullClipAmount; // Reset clip to full
        }
        else {
            currentClipAmount = loaded; // Set loaded clip to new amount
        }

        return returned; // Return excess (will remain zero if all bullets can be loaded in)
    }

    // Reloads gun without thinking about inventory
    // Used for base pistol and god mode
    public void ReloadInfinite() {
        currentClipAmount = fullClipAmount;
    }

    public void Equip() {
        equipped = true;
    }

    public void UnEquip() {
        equipped = false;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<FireEvent>(fireEventSubscription);
        EventBus.Unsubscribe<ReloadEvent>(reloadEventSubscription);
    }
}

public class FireEvent {
    public GameObject shooter;

    public FireEvent(GameObject _shooter) {
        shooter = _shooter;
    }
}

public class ReloadEvent {
    public GameObject reloader;

    public ReloadEvent(GameObject _reloader) {
        reloader = _reloader;
    }
}

public enum Shooter
{
    Player,
    Enemy
}