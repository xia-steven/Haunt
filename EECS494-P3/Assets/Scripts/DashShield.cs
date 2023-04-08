using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashShield : MonoBehaviour {
    // Bullets can only be reversed every X amount of seconds
    // Necessary for big bullets that are comprised of many small colliders
    private float timeBetweenReversal = 1.0f;

    private void OnTriggerEnter(Collider other) {
        GameObject collided = other.gameObject;
        MagicArcherMiniBullet miniBullet;

        // Reverse bullets that are hit
        Bullet bullet = collided.GetComponent<Bullet>();
        if (bullet != null) {
            if (bullet.GetShooter() == Shooter.Enemy) {
                bullet.SetShooter(Shooter.Player);
                Rigidbody rb = collided.GetComponent<Rigidbody>();
                rb.velocity = -rb.velocity;
            }
        }
        else if (collided.TryGetComponent<MagicArcherMiniBullet>(out miniBullet)) {
            if (Time.time - miniBullet.GetLastReverse() >= timeBetweenReversal) {
                miniBullet.ChangeParentShooter(Shooter.Player);
                Rigidbody rb = miniBullet.GetParentRB();
                rb.velocity = -rb.velocity;
                miniBullet.SetLastReverse();
            }
        }
    }
}