using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Weapon
{
    protected GameObject wielder;
    private GameObject lastHit;

    // The length of the raycast
    private float raycastLength = 100f;
    private LineRenderer lineRenderer;
    private int damage = -5;

    // Sprite of pistol - necessary so it can be flipped
    [SerializeField] protected GameObject sniperSprite;
    [SerializeField] protected Color laserColor;
    [SerializeField] protected Color flashColor;
    [SerializeField] protected Color reloadColor;

    protected override void Awake()
    {
        base.Awake();

        currentClipAmount = 1;
        fullClipAmount = 1;
        reloadTime = 1.5f;
        type = "pistol";

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

        int layerMask = ~(1 << LayerMask.NameToLayer("Special"));

        // Set the positions of the LineRenderer to draw a line from the current position to the point of intersection
        if (Physics.Raycast(raycastSpawn, transform.forward, out RaycastHit hit, raycastLength, layerMask))
        {
            lineRenderer.SetPositions(new Vector3[] { barrelSpawn, hit.point });
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
        currentClipAmount--;

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
        lineRenderer.material.color = flashColor;
        lineRenderer.endWidth = 0.06f;

        // Alter pedestal health if collided is pedestal and shot by player
        HasPedestalHealth pedHealth = lastHit.GetComponent<HasPedestalHealth>();
        if (pedHealth != null)
        {
            pedHealth.AlterHealth(-damage);
        }

        // Alter health if collided has health
        HasHealth health = lastHit.GetComponent<HasHealth>();
        if (health != null && pedHealth == null)
        {
            Debug.Log("Health altered");
            health.AlterHealth(damage);
        }

        yield return new WaitForSeconds(0.1f);

        lineRenderer.endWidth = 0.04f;
        GunReload();
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }
}