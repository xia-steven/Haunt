using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Weapon base class

public abstract class Weapon : MonoBehaviour {
    // Number of bullets in one clip - necessary for all guns
    protected int fullClipAmount = 8;
    public int FullClipAmount {
        get { return fullClipAmount; }
        private set { }
    }

    // Currently loaded bullets
    protected int currentClipAmount;
    public int CurrentClipAmount {
        get { return currentClipAmount; }
        private set { }
    }

    //weapon type (used to get sprite)
    protected string type;
    public string Type {
        get { return type; }
        private set { }
    }

    // Time of last bullet - used to see when last bullet was fired
    protected float lastBullet;
    // Time of last tap fire - used to limit spamming
    protected float lastTap;

    // Determines whether weapon is currently firing or not - used for automatic weapons
    protected bool firing = false;

    // Time it takes gun to reload (NEED TO SHOW THIS VISUALLY)
    protected float reloadTime;
    public float ReloadTime {
        get { return reloadTime; }
        private set{ }
    }

    protected bool isReloading = false;
    // Length of gun barrel for bullet spawning - will be gun specific due to masking / variability of sprites
    [SerializeField] protected float barrelLength = 0.5f;

    protected Subscription<FireEvent> fireEventSubscription;
    protected Subscription<ReloadEvent> reloadEventSubscription;

    bool hasDoneInitialBroadcast;

    protected virtual void Awake() {
        lastBullet = 0;
        lastTap = 0;
    }

    private void Start()
    {
        EventBus.Publish(new WeaponSwapEvent(this));
    }

    protected void Subscribe() {
        fireEventSubscription = EventBus.Subscribe<FireEvent>(_OnFire);
        reloadEventSubscription = EventBus.Subscribe<ReloadEvent>(_OnReload);
    }

    protected void UnSubscribe()
    {
        EventBus.Unsubscribe<FireEvent>(fireEventSubscription);
        EventBus.Unsubscribe<ReloadEvent>(reloadEventSubscription);
    }

    protected virtual void _OnFire(FireEvent e) {
        firing = e.state;
        if (!firing)
        {
            // Allows for click spamming but not hold spamming
            lastBullet = 0;

            if (currentClipAmount > 0) --currentClipAmount;
        }

    }

    protected virtual void _OnReload(ReloadEvent e) {
        Debug.Log("Base reload called");
    }

    // Fires a projectile of type Bullet in specified direction
    public void FireProjectile(GameObject bullet, Vector3 direction, Transform start, float bulletSpeed, Shooter shooter) {
        // Set spawn position based on barrel length
        Vector3 barrelOffset = direction * barrelLength;
        Vector3 barrelSpawn = start.position + barrelOffset;

        // Spawn bullet at barrel of gun
        GameObject projectile = Instantiate(bullet, barrelSpawn, Quaternion.identity);

        // Set shooter to holder of gun (enemy or player)
        projectile.GetComponent<Bullet>().SetShooter(shooter);

        // Give bullet its velocity
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

    public void OnEnable()
    {
        EventBus.Publish(new WeaponSwapEvent(this));
    }
}

public class FireEvent {
    public GameObject shooter;
    // True for firing false to stop firing
    public bool state;

    public FireEvent(GameObject _shooter, bool _state) {
        shooter = _shooter;
        state = _state;
    }
}

public class ReloadEvent {
    public GameObject reloader;

    public ReloadEvent(GameObject _reloader) {
        reloader = _reloader;
    }
}

public class WeaponSwapEvent
{
    public Weapon newWeapon;

    public WeaponSwapEvent(Weapon _newWeapon)
    {
        newWeapon = _newWeapon;
    }
}

public enum Shooter
{
    Player,
    Enemy
}