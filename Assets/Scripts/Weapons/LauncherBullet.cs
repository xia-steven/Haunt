using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherBullet : Bullet
{
    Rigidbody rb;
    float slowdownSpeed = 0.985f;
    float slowdownDelay = 0.6f;
    float flashDelay = 0.1f;
    [SerializeField] GameObject bulletSprite;
    private SpriteRenderer sr;
    private bool flashing = false;
    private bool red = false;
    private Color defaultColor;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sr = bulletSprite.GetComponent<SpriteRenderer>();
        defaultColor = sr.color;
        damage = -4;
    }

    public void setLifetime(float lifetime)
    {
        bulletLife = lifetime;
    }

    private new void Update()
    {
        base.Update();

        // Slowdown over time, after a delay
        if (Time.time - firedTime >= slowdownDelay)
        {
            rb.velocity *= slowdownSpeed;
            if (!flashing)
                StartCoroutine(FlashSprite());
        }
    }

    private IEnumerator FlashSprite()
    {
        flashing = true;

        // Loop until bullet explodes
        while (true)
        {
            yield return new WaitForSeconds(flashDelay);
            if (red)
            {
                sr.color = defaultColor;
                red = false;
            }
            else
            {
                sr.color = Color.red;
                red = true;
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        var collided = other.gameObject;

        // Don't collide with specified items
        if (collided.layer == LayerMask.NameToLayer("Special"))
        {
            return;
        }

        // Make sure player can't shoot themselves
        if (collided.layer == LayerMask.NameToLayer("Player"))
        {
            return;
        }

        if (collided.layer == LayerMask.NameToLayer("PlayerUtility"))
        {
            if (collided.GetComponent<Bullet>().GetShooter() == Shooter.Player)
                return;
        }

        // Alter pedestal health if collided is pedestal and shot by player
        HasPedestalHealth pedHealth = collided.GetComponent<HasPedestalHealth>();
        if (pedHealth != null)
        {
            pedHealth.AlterHealth(-damage * PlayerModifiers.damage);
        }

        // Alter health if collided has health
        HasHealth health = collided.GetComponent<HasHealth>();
        if (health != null && pedHealth == null)
        {
            Debug.Log("Health altered");
            health.AlterHealth(damage * PlayerModifiers.damage);
            pierced++;
        }

        // Don't destroy upon melee collision
        /*
        if (collided.layer == LayerMask.NameToLayer("Melee"))
        {
            return;
        }
        */

        Destroy(gameObject);
    }
}
