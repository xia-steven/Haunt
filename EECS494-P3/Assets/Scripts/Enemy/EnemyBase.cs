using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour {
    protected float speed;
    public int health;

    private Rigidbody rb;
    protected int currentPathIndex;
    protected List<Vector3> pathVectorList;
    private Transform tf_;
    private Vector3 origin;

    protected void Start() {
        rb = GetComponent<Rigidbody>();
        tf_ = transform;
        // origin = new Vector3(-10.5f, -0.5f, -4.5f);
        origin = new Vector3(-17.5f, 0, -10.5f);
    }

    private void Update() {
        HandleMovement();
    }

    private void HandleMovement() {
        if (pathVectorList != null) {
            var targetPosition = pathVectorList[currentPathIndex] + origin;
            if (Vector3.Distance(transform.position, targetPosition) > 1f) {
                var moveDir = (targetPosition - transform.position).normalized;
                tf_.position += Time.deltaTime * speed * moveDir;
            }
            else {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count) {
                    pathVectorList = null;
                    rb.velocity = Vector3.zero;
                }
            }
        }
        else {
            rb.velocity = Vector3.zero;
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