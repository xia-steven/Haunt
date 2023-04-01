using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashShield : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject collided = other.gameObject;

        // Reverse bullets that are hit
        Bullet bullet = collided.GetComponent<Bullet>();
        if (collided.layer == LayerMask.NameToLayer("PlayerUtility") && bullet != null)
        {
            if (bullet.GetShooter() == Shooter.Enemy)
            {
                collided.GetComponent<Rigidbody>().velocity = -collided.GetComponent<Rigidbody>().velocity;
            }
        }
    }
}
