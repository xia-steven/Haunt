using Enemy.Weapons;
using UnityEngine;
using Weapons;

public class DashShield : MonoBehaviour {
    // Bullets can only be reversed every X amount of seconds
    // Necessary for big bullets that are comprised of many small colliders
    private const float timeBetweenReversal = 1f;

    private void OnTriggerEnter(Collider other) {
        var collided = other.gameObject;

        // Reverse bullets that are hit
        var bullet = collided.GetComponent<Bullet>();
        if (bullet != null) {
            if (bullet.GetShooter() != Shooter.Enemy) return;
            bullet.SetShooter(Shooter.Player);
            var rb = collided.GetComponent<Rigidbody>();
            rb.velocity = -rb.velocity;
        }
        else if (collided.TryGetComponent(out MagicArcherMiniBullet miniBullet) &&
                 Time.time - miniBullet.GetLastReverse() >= timeBetweenReversal) {
            miniBullet.ChangeParentShooter(Shooter.Player);
            var rb = miniBullet.GetParentRB();
            rb.velocity = -rb.velocity;
            miniBullet.SetLastReverse();
        }
    }
}