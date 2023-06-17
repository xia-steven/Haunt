using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingSword : MonoBehaviour
{
    private bool rotating = false;
    // Rotation speed in degrees per second
    private float rotationSpeed = 10f;
    [SerializeField] private int damage = -1;

    public void SetUp(float speed)
    {
        rotationSpeed = speed;
        rotating = true;
    }

    private void FixedUpdate()
    {
        if (rotating)
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject collided = other.gameObject;

        // Don't collide with specified items
        if (collided.layer == LayerMask.NameToLayer("Special"))
        {
            return;
        }

        // Don't collide with player
        if (collided.layer == LayerMask.NameToLayer("Player"))
        {
            return;
        }

        // Reverse bullets that are hit
        if (collided.layer == LayerMask.NameToLayer("PlayerUtility") && collided.GetComponent<Bullet>().GetShooter() != Shooter.Player)
        {
            collided.GetComponent<Rigidbody>().velocity = -collided.GetComponent<Rigidbody>().velocity;
        }

        // Alter pedestal health if collided is pedestal and shot by player
        HasPedestalHealth pedHealth = collided.GetComponent<HasPedestalHealth>();
        if (pedHealth != null)
        {
            pedHealth.AlterHealth(-damage);
        }

        // Alter health if collided has health
        HasHealth health = collided.GetComponent<HasHealth>();
        if (health != null && pedHealth == null)
        {
            Debug.Log("Health altered");
            health.AlterHealth(damage);
        }
    }
}
