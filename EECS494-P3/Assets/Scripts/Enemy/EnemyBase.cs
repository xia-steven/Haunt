using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour {
    private const float speed = 2f;
    public int health;

    private Rigidbody rb;
    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    private Transform tf_;

    private Subscription<PlayerPositionEvent> positionSub;

    protected void Start() {
        rb = GetComponent<Rigidbody>();
        tf_ = transform;
        positionSub = EventBus.Subscribe<PlayerPositionEvent>(SetTargetPosition);
    }

    private void Update() {
        HandleMovement();
    }

    private void HandleMovement() {
        if (pathVectorList != null) {
            var targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(transform.position, targetPosition) > 1f) {
                var moveDir = (targetPosition - transform.position).normalized;
                tf_.position += Time.deltaTime * speed * moveDir;
            }
            else {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count) {
                    StopMoving();
                    rb.velocity = Vector3.zero;
                }
            }
        }
        else {
            rb.velocity = Vector3.zero;
        }
    }

    private void StopMoving() {
        pathVectorList = null;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    private void SetTargetPosition(PlayerPositionEvent event_) {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), event_.position);

        if (pathVectorList != null && pathVectorList.Count > 1) {
            pathVectorList.RemoveAt(0);
        }
    }

    private void TakeDamage(int dmg) {
        health -= dmg;
        if (health <= 0) {
            Destroy(gameObject);
        }
    }

    protected void OnTriggerEnter(Collider other) {
        TakeDamage(1);
        Destroy(other.gameObject);
    }
}