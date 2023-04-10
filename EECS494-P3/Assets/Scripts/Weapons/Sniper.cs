using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Sniper : Weapon
{
    protected GameObject wielder;
    private GameObject lastHit;

    // The length of the raycast
    private float raycastLength = 100f;
    private LineRenderer lineRenderer;
    private int damage = -5;
    private bool isFiring = false;

    // Sprite of pistol - necessary so it can be flipped
    [SerializeField] protected GameObject sniperSprite;
    [SerializeField] protected Color laserColor;
    [SerializeField] protected Color flashColor;
    [SerializeField] protected Color reloadColor;

    private float pierce = 1;
    private RaycastHit[] hits;

    protected override void Awake()
    {
        base.Awake();
        thisData = typesData.types[(int)WeaponType.sniper];

        SetData();
        currentClipAmount = fullClipAmount;
        pierce += PlayerModifiers.maxPierce;

        Subscribe();

        spriteRenderer = sniperSprite.GetComponent<SpriteRenderer>();
        wielder = this.transform.parent.gameObject;

        lineRenderer = gameObject.GetComponent<LineRenderer>();

        // Set the width and color of the line
        lineRenderer.startWidth = 0.04f;
        lineRenderer.endWidth = 0.04f;
        lineRenderer.material.color = laserColor;
    }

    // Need to add extra elements for the sniper fixed update such as the scope line
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        gunDirection.y = 0;
        gunDirection = gunDirection.normalized;

        // Set spawn position based on barrel length
        Vector3 barrelOffset = gunDirection * barrelLength;
        Vector3 barrelSpawn = transform.position + barrelOffset;

        Vector3 raycastSpawn = barrelSpawn;
        raycastSpawn.y = 0.5f;

        int layerMask = ~LayerMask.GetMask("Special", "BossShockwave");

        // Get all hits of raycast
        hits = Physics.RaycastAll(raycastSpawn, transform.forward, 100, layerMask);

        // Set the positions of the LineRenderer to draw a line from the current position to the point of intersection
        if (Physics.Raycast(raycastSpawn, transform.forward, out RaycastHit hit, raycastLength, layerMask))
        {
            // Set laser
            lineRenderer.SetPositions(new Vector3[] { barrelSpawn, hit.point });

            // Sort hit array (now that we know objects were hit)
            Array.Sort(hits, (x, y) => Vector3.Distance(raycastSpawn, x.point).CompareTo(Vector3.Distance(raycastSpawn, y.point)));
            lastHit = hit.collider.gameObject;
        }
        else
        {
            // If the raycast doesn't hit anything, draw a line for the entire length of the ray
            lineRenderer.SetPositions(new Vector3[] { barrelSpawn, transform.position + transform.forward * raycastLength });
        }
    }

    protected override void _OnFire(FireEvent e)
    {
        if (!gameObject.activeInHierarchy) return;

        // Check if fire event comes from sniper holder
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
        lineRenderer.material.color = reloadColor;
        Debug.Log("Reloading");
        // Play reload sound
        AudioSource.PlayClipAtPoint(reloadSound, transform.position);

        EventBus.Publish<ReloadStartedEvent>(new ReloadStartedEvent(reloadTime));
        yield return new WaitForSeconds(reloadTime);

        // TODO: change to line up with inventory ammo
        ReloadInfinite();
        Debug.Log("Sniper ammo: " + currentClipAmount);
        lineRenderer.material.color = laserColor;
        isReloading = false;
    }

    protected override void WeaponFire(Vector3 direction)
    {
        if (!playerEnabled) return;

        if (messageVisible) return;

        currentClipAmount--;

        // Double check pierce amount (sniper can always pierce one)
        pierce = PlayerModifiers.maxPierce + 1;

        StartCoroutine(SniperFire());

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

    private IEnumerator SniperFire()
    {
        isFiring = true;
        lineRenderer.material.color = flashColor;
        lineRenderer.endWidth = 0.06f;
        // Play firing sound
        AudioSource.PlayClipAtPoint(firingSound, transform.position);

        Debug.Log("Beginning of loop");
        Debug.Log("Length: " + hits.Length);
        for (int i = 0; i < pierce; i++)
        {
            if (i >= hits.Length)
            {
                break;
            }

            Debug.Log(hits[i].collider.gameObject.name);

            // Alter pedestal health if collided is pedestal and shot by player
            HasPedestalHealth pedHealth = hits[i].collider.gameObject.GetComponent<HasPedestalHealth>();
            if (pedHealth != null)
            {
                pedHealth.AlterHealth(-damage);
                // Pedestals count as "buildings" so don't pierce
                break;
            }

            // Alter health if collided has health
            HasHealth health = hits[i].collider.gameObject.GetComponent<HasHealth>();
            if (health != null && pedHealth == null)
            {
                Debug.Log("Health altered");
                health.AlterHealth(damage);
            }

            // Hit non-pedestal and non-enemy (most likely wall) so don't allow pierce
            if (health == null && pedHealth == null)
            {
                break;
            }
        }
        Debug.Log("Got to end of loop");

        yield return new WaitForSeconds(0.1f);

        Debug.Log("Beginning reload and line fix");
        lineRenderer.endWidth = 0.04f;
        isFiring = false;
        GunReload();

        // Shake screen
        EventBus.Publish(new ScreenShakeEvent(screenShakeStrength));
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }

    protected override void OnDisable()
    {
        if (isReloading || isFiring)
        {
            lineRenderer.material.color = laserColor;
            lineRenderer.endWidth = 0.04f;
        }
        base.OnDisable();
    }
}
