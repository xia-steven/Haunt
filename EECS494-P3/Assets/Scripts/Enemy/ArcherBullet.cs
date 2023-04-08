using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArcherBullet : EnemyBasicBullet {
    private new void Awake() {
        base.Awake();
        bulletLife = 1f;
    }

    public new void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);
    }
}