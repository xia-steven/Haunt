using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class EnemyBase : MonoBehaviour {
    private const float speed = 30f;

    private Rigidbody rb;
    private int currentPathIndex;
    private List<Vector3> pathVectorList;


    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        HandleMovement();

        if (Input.GetMouseButtonDown(0)) {
            SetTargetPosition(UtilsClass.GetMouseWorldPosition());
        }
    }

    private void HandleMovement() {
        if (pathVectorList != null) {
            var targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(transform.position, targetPosition) > 1f) {
                var moveDir = (targetPosition - transform.position).normalized;
                transform.position += Time.deltaTime * speed * moveDir;
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

    public void SetTargetPosition(Vector3 targetPosition) {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition);

        if (pathVectorList != null && pathVectorList.Count > 1) {
            pathVectorList.RemoveAt(0);
        }
    }
}