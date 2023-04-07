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
        return currentTargetPedestal;
    }

    public override bool needAStar(RaycastHit hit) {
        return hit.transform.gameObject.layer != LayerMask.NameToLayer("Pedestal");
    }

    private new void Start() {
        base.Start();
        enemyHealth.setClericStatus(true);
        baseSpeed = attributes.moveSpeed;
        switchPedestalSub = EventBus.Subscribe<PedestalDestroyedEvent>(pedestalDied);
        addPedestalSub = EventBus.Subscribe<PedestalRepairedEvent>(pedestalRepaired);
        currentTargetPedestal = findClosestPedestal();
        SetTargetPosition(currentTargetPedestal);
    }

    // Override attack function
    public override IEnumerator EnemyAttack() {
        // While attacking
        while (state == EnemyState.Attacking) {
            var targetPosition = IsPlayer.instance.transform.position;

            var direction = targetPosition - transform.position;
            direction.y = 0;
            direction = direction.normalized;

            // TODO: finish me

            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        // Let update know that we're done
        runningCoroutine = false;
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(switchPedestalSub);
        EventBus.Unsubscribe(addPedestalSub);
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
        currentTargetPedestal = findClosestPedestal();
        SetTargetPosition(currentTargetPedestal);
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
        currentTargetPedestal = findClosestPedestal();
        SetTargetPosition(currentTargetPedestal);
    }
}