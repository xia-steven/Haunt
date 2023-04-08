using ConfigDataTypes;
using Events;
using JSON_Parsing;
using Player;
using UnityEngine;
using Weapons;

// Weapon base class

public abstract class Weapon : MonoBehaviour {
    public static Weapon activeWeapon;

    // Number of bullets in one clip - necessary for all guns
    protected int fullClipAmount = 8;

    public int FullClipAmount {
        get => fullClipAmount;
        private set { }
    }

    // Currently loaded bullets
    protected int currentClipAmount;

    public int CurrentClipAmount {
        get => currentClipAmount;
        private set { }
    }

    //weapon type (used to get sprite)
    protected string type;

    public string Type {
        get => type;
        private set { }
    }

    // Time of last bullet - used to see when last bullet was fired
    protected float lastBullet;

    // Time of last tap fire - used to limit spamming
    protected float lastTap;

    // Determines whether weapon is currently firing or not - used for automatic weapons
    protected bool firing;

    // Direction that gun in facing for overwritten FixedUpdates
    protected Vector3 gunDirection;

    // Time it takes gun to reload
    protected float reloadTime;

    protected SpriteRenderer spriteRenderer;

    // Time between bullets
    [SerializeField] protected float bulletDelay = 0.6f;

    // Time between tap firing
    [SerializeField] protected float tapDelay = 0.2f;


    public float ReloadTime {
        get => reloadTime;
        private set { }
    }

    protected bool shotByPlayer = true;

    public bool ShotByPlayer {
        get => shotByPlayer;
        private set { }
    }

    protected bool playerEnabled = true;

    protected bool isReloading;

    // Length of gun barrel for bullet spawning - will be gun specific due to masking / variability of sprites
    [SerializeField] protected float barrelLength = 0.5f;

    // How much the screen should shake on bullets firing
    [SerializeField] protected float screenShakeStrength = 0.05f;

    // Buffs/nerfs on player speed for specific guns
    [SerializeField] protected float speedMultiplier = 1f;

    protected static WeaponTypesData typesData;
    protected WeaponData thisData;

    private Subscription<FireEvent> fireEventSubscription;
    private Subscription<ReloadEvent> reloadEventSubscription;
    protected Subscription<EnablePlayerEvent> enablePlayerSubscription;
    protected Subscription<DisablePlayerEvent> disablePlayerSubscription;


    protected virtual void Awake() {
        typesData = typesData switch {
            null => ConfigManager.GetData<WeaponTypesData>("WeaponTypes"),
            _ => typesData
        };

        lastBullet = 0;
        lastTap = 0;
        PlayerModifiers.moveSpeed *= speedMultiplier;
    }

    protected void Subscribe() {
        fireEventSubscription = EventBus.Subscribe<FireEvent>(_OnFire);
        reloadEventSubscription = EventBus.Subscribe<ReloadEvent>(_OnReload);
        enablePlayerSubscription = EventBus.Subscribe<EnablePlayerEvent>(_OnEnablePlayer);
        disablePlayerSubscription = EventBus.Subscribe<DisablePlayerEvent>(_OnDisablePlayer);
    }

    protected void UnSubscribe() {
        EventBus.Unsubscribe(fireEventSubscription);
        EventBus.Unsubscribe(reloadEventSubscription);
    }

    protected virtual void _OnEnablePlayer(EnablePlayerEvent e) {
        Debug.Log("Player enabled in weapon");
        playerEnabled = true;
    }

    protected virtual void _OnDisablePlayer(DisablePlayerEvent e) {
        Debug.Log("Player disabled in weapon");
        playerEnabled = false;
    }

    protected virtual void _OnFire(FireEvent e) {
        firing = e.state;
        switch (firing) {
            case true when currentClipAmount <= 0 && !isReloading:
                GunReload();
                break;
        }

        lastBullet = firing switch {
            // Allows for click spamming but not hold spamming
            false => 0,
            _ => lastBullet
        };
    }

    protected virtual void _OnReload(ReloadEvent e) {
        Debug.Log("Base reload called");
    }

    protected virtual void GunReload() {
        Debug.Log("Base gun-specific reload called");
    }

    // Fires a projectile of type Bullet in specified direction
    protected void FireProjectile(GameObject bullet, Vector3 direction, Transform start, float bulletSpeed,
        Shooter shooter) {
        // Set spawn position based on barrel length
        var barrelOffset = direction * barrelLength;
        var barrelSpawn = start.position + barrelOffset;

        // Spawn bullet at barrel of gun
        var projectile = Instantiate(bullet, barrelSpawn, Quaternion.identity);

        // Set shooter to holder of gun (enemy or player)
        projectile.GetComponent<Bullet>().SetShooter(shooter);

        // Give bullet its velocity
        var rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = direction * bulletSpeed;
    }


    // Reloads gun with specified number of bullets
    // Returns number of bullets returned to inventory
    public int Reload(int bulletsLoaded) {
        var returned = 0;
        var loaded = currentClipAmount + bulletsLoaded;

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

    public void OnEnable() {
        switch (shotByPlayer) {
            case true:
                EventBus.Publish(new WeaponSwapEvent(this));
                PlayerModifiers.moveSpeed = speedMultiplier;
                break;
        }

        firing = false;
    }

    protected virtual void FixedUpdate() {
        switch (shotByPlayer) {
            case false:
                return;
        }

        // Get the screen position of the cursor
        var screenPos = Input.mousePosition;
        var direction = Vector3.zero;

        // Create a ray from the camera through the cursor position
        if (Camera.main != null) {
            var ray = Camera.main.ScreenPointToRay(screenPos);

            // Find the point where the ray intersects the plane that contains the player
            var groundPlane = new Plane(Vector3.up, transform.position);
            if (groundPlane.Raycast(ray, out var distanceToGround) && playerEnabled) {
                // Calculate the direction vector from the player to the intersection point
                var hitPoint = ray.GetPoint(distanceToGround);
                direction = hitPoint - transform.position;

                gunDirection = direction;

                spriteRenderer.flipY = direction.x switch {
                    // Check if gun sprite needs to be flipped
                    < 0 => true,
                    _ => false
                };

                // Calculate the rotation that points in the direction of the intersection point
                var rotation = Quaternion.LookRotation(direction, Vector3.up);

                // Set the rotation of the gun object
                transform.rotation = rotation;
            }
        }

        switch (currentClipAmount) {
            // Fire bullet if ammo in clip, trigger is down, last bullet was not fired recently, last tap was not recent, not reloading
            case > 0 when firing && (Time.time - lastBullet >= bulletDelay) &&
                          (Time.time - lastTap >= tapDelay) && !isReloading:
                WeaponFire(direction);
                break;
        }
    }

    protected virtual void WeaponFire(Vector3 direction) {
        Debug.Log("Base WeaponFire");
    }

    protected bool CanReload() {
        return isReloading switch {
            false when currentClipAmount != fullClipAmount => true,
            _ => false
        };
    }

    protected void SetData() {
        if (thisData != null) {
            type = thisData.name;
            fullClipAmount = thisData.fullClip;
            bulletDelay = thisData.bulletDelay;
            tapDelay = thisData.tapDelay;
            screenShakeStrength = thisData.screenShakeStrength;
            speedMultiplier = thisData.speedMultiplier;
            reloadTime = thisData.reloadTime;
        }
        else {
            Debug.Log("thisData for weapon set to null");
        }
    }

    protected virtual void OnDisable() {
        switch (isReloading) {
            case true:
                StopAllCoroutines();
                ReloadInfinite();
                isReloading = false;
                break;
        }
    }
}

public class FireEvent {
    public readonly GameObject shooter;

    // True for firing false to stop firing
    public readonly bool state;

    public FireEvent(GameObject _shooter, bool _state) {
        shooter = _shooter;
        state = _state;
    }
}

public class ReloadEvent {
    public readonly GameObject reloader;

    public ReloadEvent(GameObject _reloader) {
        reloader = _reloader;
    }
}

public class ReloadStartedEvent {
    public readonly float reloadTime;

    public ReloadStartedEvent(float _reloadTime) {
        reloadTime = _reloadTime;
    }
}

public class WeaponSwapEvent {
    public readonly Weapon newWeapon;

    public WeaponSwapEvent(Weapon _newWeapon) {
        newWeapon = _newWeapon;
    }
}

public enum Shooter {
    Player,
    Enemy
}

public enum WeaponType {
    pistol = 0,
    rifle = 1,
    shotgun = 2,
    sniper = 3,
    launcher = 4
}