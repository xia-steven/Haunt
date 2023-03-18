using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for all bullet types
public abstract class Bullet : MonoBehaviour {
    protected int damage = -1;
    protected Shooter shooter;
    protected float firedTime;

    [SerializeField] protected float bulletLife = 1.0f;

    protected virtual void Awake()
    {
        firedTime = Time.time;
    }

    public void SetShooter(Shooter entity)
    {
        shooter = entity;
    }

    protected void OnTriggerEnter(Collider other)
    {
        GameObject collided = other.gameObject;

        // Don't collide with specified items
        if (collided.layer == LayerMask.NameToLayer("Special"))
        {
            return;
        }

        // Make sure player can't shoot themselves
        if (collided.layer == LayerMask.NameToLayer("Player") && shooter == Shooter.Player)
        {
            return;
        }

        // Make sure ememy can't shoot themselves or other enemies
        if (collided.layer == LayerMask.NameToLayer("Enemy") && shooter == Shooter.Enemy)
        {
            return;
        }

        // Alter health if collided has health
        HasHealth health = collided.GetComponent<HasHealth>();
        if (health != null)
        {
            health.AlterHealth(damage);
        }

        // Alter pedestal health if collided is pedestal
        HasPedestalHealth pedHealth = collided.GetComponent<HasPedestalHealth>();
        if (pedHealth != null)
        {
            pedHealth.AlterHealth(-damage);
        }

        Destroy(gameObject);
    }

    protected void Update()
    {
        // Destroy bullet after a certain amount of time
        float passedTime = Time.time - firedTime;
        if (passedTime >= bulletLife)
        {
            Destroy(gameObject);
        }
    }
}