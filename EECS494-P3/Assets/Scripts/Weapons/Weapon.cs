using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Weapon base class

public abstract class Weapon : MonoBehaviour {

    public static Weapon activeWeapon;
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

    // Direction that gun in facing for overwritten FixedUpdates
    protected Vector3 gunDirection;

    // Time it takes gun to reload
    protected float reloadTime;
    protected SpriteRenderer spriteRenderer;
    // Time between bullets
    [SerializeField] protected float bulletDelay = 0.6f;
    // Time between tap firing
    [SerializeField] protected float tapDelay = 0.2f;

    // Reload prompt variables
    Sprite rSprite;
    Sprite rPressedSprite;
    SpritePromptEvent rPrompt;

    public float ReloadTime {
        get { return reloadTime; }
        private set{ }
    }

    protected bool shotByPlayer = true;
    public bool ShotByPlayer {
        get { return shotByPlayer; }
        private set{ }
    }

    protected bool playerEnabled = true;
    protected bool isReloading = false;
    public bool messageVisible = false;
    protected int lastMessageSender;
    // Length of gun barrel for bullet spawning - will be gun specific due to masking / variability of sprites
    [SerializeField] protected float barrelLength = 0.5f;
    // How much the screen should shake on bullets firing
    [SerializeField] protected float screenShakeStrength = 0.05f;
    // Buffs/nerfs on player speed for specific guns
    [SerializeField] protected float speedMultiplier = 1f;

    // Gun sounds
    [SerializeField] protected AudioClip firingSound;
    [SerializeField] protected AudioClip reloadSound;

    protected static WeaponTypesData typesData;
    protected WeaponData thisData;

    protected Subscription<FireEvent> fireEventSubscription;
    protected Subscription<ReloadEvent> reloadEventSubscription;
    protected Subscription<EnablePlayerEvent> enablePlayerSubscription;
    protected Subscription<DisablePlayerEvent> disablePlayerSubscription;
    protected Subscription<MessageEvent> messageSubscription;
    protected Subscription<MessageFinishedEvent> messageFinishedSubscription;
    protected Subscription<GameLossEvent> gameLossSubscription;

    protected virtual void Awake()
    {
        if (typesData == null)
            typesData = ConfigManager.GetData<WeaponTypesData>("WeaponTypes");

        lastBullet = 0;
        lastTap = 0;
        PlayerModifiers.moveSpeed *= speedMultiplier;

        Object[] sprites = Resources.LoadAll("tilemap");
        rSprite = (Sprite)sprites[361];
        rPressedSprite = (Sprite)sprites[89];

        rPrompt = new SpritePromptEvent(rSprite, rPressedSprite, KeyCode.R);
        rPrompt.cancelPrompt = true;
    }

    protected void Subscribe() {
        fireEventSubscription = EventBus.Subscribe<FireEvent>(_OnFire);
        reloadEventSubscription = EventBus.Subscribe<ReloadEvent>(_OnReload);
        enablePlayerSubscription = EventBus.Subscribe<EnablePlayerEvent>(_OnEnablePlayer);
        disablePlayerSubscription = EventBus.Subscribe<DisablePlayerEvent>(_OnDisablePlayer);
        messageSubscription = EventBus.Subscribe<MessageEvent>(_OnMessage);
        messageFinishedSubscription = EventBus.Subscribe<MessageFinishedEvent>(_OnMessageFinished);
        gameLossSubscription = EventBus.Subscribe<GameLossEvent>(_OnGameLoss);
    }

    protected void UnSubscribe()
    {
        EventBus.Unsubscribe(fireEventSubscription);
        EventBus.Unsubscribe(reloadEventSubscription);
    }

    protected virtual void _OnEnablePlayer(EnablePlayerEvent e)
    {
        Debug.Log("Player enabled in weapon");
        playerEnabled = true;
    }

    protected virtual void _OnDisablePlayer(DisablePlayerEvent e)
    {
        Debug.Log("Player disabled in weapon");
        playerEnabled = false;
    }

    protected virtual void _OnMessage(MessageEvent e)
    {
        Debug.Log("Message showing received by weapon");
        lastMessageSender = e.senderInstanceID;
        messageVisible = true;
    }

    protected virtual void _OnMessageFinished(MessageFinishedEvent e)
    {
        Debug.Log("Message finished received by weapon");
        if(isActiveAndEnabled)
        {
            StartCoroutine(MessageDelay(e));
        }
    }
    protected IEnumerator MessageDelay(MessageFinishedEvent e)
    {
        yield return new WaitForSeconds(1.0f);
        if (e.senderInstanceID == lastMessageSender)
            messageVisible = false;
    }

    protected virtual void _OnGameLoss(GameLossEvent e)
    {
        // Stop all firing and set all guns inactive
        firing = false;
        gameObject.SetActive(false);
    }

    protected virtual void _OnFire(FireEvent e) {
        firing = e.state;
        if (firing && currentClipAmount <= 0 && !isReloading)
        {
            GunReload();
        }
        if (!firing)
        {
            // Allows for click spamming but not hold spamming
            lastBullet = 0;
        }
    }

    protected virtual void _OnReload(ReloadEvent e) {
        Debug.Log("Base reload called");
    }

    protected virtual void GunReload()
    {
        Debug.Log("Base gun-specific reload called");
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


        // Calculate the rotation of gun barrel
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

        // Set the rotation of the bullet
        projectile.transform.rotation = rotation;

        // Give bullet its velocity
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = direction * bulletSpeed;

        // Play firing sound
        AudioSource.PlayClipAtPoint(firingSound, transform.position);
    }


    // Reloads gun with specified number of bullets
    // Returns number of bullets returned to inventory
    public int Reload(int bulletsLoaded) {
        int returned = 0;
        int loaded = currentClipAmount + bulletsLoaded;

        // Cancel reload prompt
        rPrompt.cancelPrompt = true;

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

        // Cancel reload prompt
        rPrompt.cancelPrompt = true;
    }

    public void OnEnable()
    {
        if (shotByPlayer)
        {
            EventBus.Publish(new WeaponSwapEvent(this));
            PlayerModifiers.moveSpeed = speedMultiplier;
        }
        firing = false;
        messageVisible = false;

        if (currentClipAmount == 0 && thisData.name != "sniper")
        {
            rPrompt.cancelPrompt = false;
            EventBus.Publish(rPrompt);
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!shotByPlayer) return;

        // Get the screen position of the cursor
        Vector3 screenPos = Input.mousePosition;
        Vector3 direction = Vector3.zero;

        // Create a ray from the camera through the cursor position
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // Find the point where the ray intersects the plane that contains the player
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        if (groundPlane.Raycast(ray, out float distanceToGround) && playerEnabled)
        {
            // Calculate the direction vector from the player to the intersection point
            Vector3 hitPoint = ray.GetPoint(distanceToGround);
            direction = hitPoint - transform.position;

            gunDirection = direction;

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
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Set the rotation of the gun object
            transform.rotation = rotation;
        }

        // Fire bullet if ammo in clip, trigger is down, last bullet was not fired recently, last tap was not recent, not reloading
        if (currentClipAmount > 0 && firing && (Time.time - lastBullet >= bulletDelay) && (Time.time - lastTap >= tapDelay) && !isReloading && !messageVisible)
        {
            WeaponFire(direction);
            if (currentClipAmount == 0 && thisData.name != "sniper")
            {
                rPrompt.cancelPrompt = false;
                EventBus.Publish(rPrompt);
            }
        }
    }

    protected virtual void WeaponFire(Vector3 direction)
    {
        Debug.Log("Base WeaponFire");
    }

    protected bool CanReload()
    {
        if (!isReloading && currentClipAmount != fullClipAmount)
        {
            return true;
        }

        return false; 
    }

    protected void SetData()
    {
        if (thisData != null)
        {
            type = thisData.name;
            fullClipAmount = thisData.fullClip;
            bulletDelay = thisData.bulletDelay;
            tapDelay = thisData.tapDelay;
            screenShakeStrength = thisData.screenShakeStrength;
            speedMultiplier = thisData.speedMultiplier;
            reloadTime = thisData.reloadTime;
        } else
        {
            Debug.Log("thisData for weapon set to null");
        }
    }

    protected virtual void OnDisable()
    {
        if (isReloading)
        {
            StopAllCoroutines();
            ReloadInfinite();
            isReloading = false;
        }

        rPrompt.cancelPrompt = true;
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

public class ReloadStartedEvent
{
    public float reloadTime;

    public ReloadStartedEvent(float _reloadTime)
    {
        reloadTime = _reloadTime;
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

public enum WeaponType
{
    pistol = 0,
    rifle = 1,
    shotgun = 2,
    sniper = 3,
    launcher = 4
}