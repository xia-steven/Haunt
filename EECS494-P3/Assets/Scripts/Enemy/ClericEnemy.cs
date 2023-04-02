using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PedestalInfo {
    public Vector3 position;
    public bool destroyed = true;

    public PedestalInfo(Vector3 pos) {
        position = pos;
    }
}

public class ClericEnemy : EnemyBase {
    private Subscription<PedestalDestroyedEvent> switchPedestalSub;
    private Subscription<PedestalRepairedEvent> addPedestalSub;
    private Vector3 currentTargetPedestal;

    private float prevTime;
    public int pedestalTimeout;

    public override int GetEnemyID() {
        return 9;
    }

    public override Vector3 GetTarget() {
        return findClosestPedestal();
    }

    private new void Start() {
        base.Start();
        // TODO: REMEMBER TO CHANGE ME BACK!
        baseSpeed = attributes.moveSpeed * 5;
        switchPedestalSub = EventBus.Subscribe<PedestalDestroyedEvent>(pedestalDied);
        addPedestalSub = EventBus.Subscribe<PedestalRepairedEvent>(pedestalRepaired);
        SetTargetPosition(findClosestPedestal());
    }

    private new void FixedUpdate() {
        if (pathVectorList != null) {
            var targetPosition = pathVectorList[currentPathIndex] + PathfindingController.map.origin;
            if (Vector3.Distance(transform.position, targetPosition) > 0.5f) {
                var moveDir = (targetPosition - transform.position).normalized;
                tf_.position += Time.deltaTime * baseSpeed * moveDir;
            }
            else {
                if (++currentPathIndex >= pathVectorList.Count) {
                    pathVectorList = null;
                    rb.velocity = Vector3.zero;
                }
            }
        }
        else {
            rb.velocity = Vector3.zero;
        }
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(switchPedestalSub);
        EventBus.Unsubscribe(addPedestalSub);
    }

    private static IEnumerator AttackPedestal(HasHealth h) {
        while (h.GetHealth() < HasPedestalHealth.PedestalMaxHealth) {
            h.AlterHealth(-1);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pedestal")) {
            var h = other.gameObject.GetComponent<HasPedestalHealth>();
            if (h != null) {
                StartCoroutine(AttackPedestal(h));
            }
        }
        else if (other.gameObject.CompareTag("Player")) {
            playerHealth.AlterHealth(-1, DeathCauses.Enemy);
        }
    }

    private Vector3 findClosestPedestal() {
        if (PathfindingController.pedestalInfos.All(ped => !ped.Value.destroyed)) {
            return Vector3.zero;
        }

        var closestDist = float.MaxValue;
        var closest = (int)Random.Range(1f, 3.99f);
        while (!PathfindingController.pedestalInfos[closest].destroyed) {
            closest = (int)Random.Range(1f, 3.99f);
        }

        foreach (var ped in PathfindingController.pedestalInfos) {
            var distance = Vector3.Distance(transform.position, ped.Value.position);
            if (distance < closestDist && ped.Value.destroyed) {
                closestDist = distance;
                closest = ped.Key;
            }
        }

        return PathfindingController.pedestalInfos[closest].position;
    }

    private void SetTargetPosition(Vector3 pos) {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(transform.position, pos);

        if (pathVectorList != null && pathVectorList.Count > 1) {
            pathVectorList.RemoveAt(0);
        }
    }

    private IEnumerator pedestalCoroutine() {
        yield return new WaitForSeconds(pedestalTimeout);
        SetTargetPosition(findClosestPedestal());
    }

    private void pedestalDied(PedestalDestroyedEvent event_) {
        switch (event_.pedestalUUID) {
            case 1:
                PathfindingController.pedestalInfos[1].destroyed = true;
                break;
            case 2:
                PathfindingController.pedestalInfos[2].destroyed = true;
                break;
            case 3:
                PathfindingController.pedestalInfos[3].destroyed = true;
                break;
        }

        StartCoroutine(pedestalCoroutine());
    }

    private void pedestalRepaired(PedestalRepairedEvent event_) {
        PathfindingController.pedestalInfos[event_.pedestalUUID].destroyed = false;
        SetTargetPosition(findClosestPedestal());
    }
}