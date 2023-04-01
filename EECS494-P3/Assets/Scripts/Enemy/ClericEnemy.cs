using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    private float prevTime;
    public int pedestalTimeout;

    public override int GetEnemyID() {
        return 9;
    }

    private new void Start() {
        base.Start();
        baseSpeed = 1.5f;
        switchPedestalSub = EventBus.Subscribe<PedestalDestroyedEvent>(pedestalDied);
        addPedestalSub = EventBus.Subscribe<PedestalRepairedEvent>(pedestalRepaired);
        StartCoroutine(WaitAndFindPath());
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pedestal")) {
            var h = other.gameObject.GetComponent<HasPedestalHealth>();
            if (h != null) {
                h.AlterHealth(-5000);
            }
        }
    }

    private int findClosestPedestal() {
        if (PathfindingController.pedestalInfos.All(ped => !ped.Value.destroyed)) {
            return 0;
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

        return closest;
    }

    private IEnumerator WaitAndFindPath() {
        yield return null;
        SetTargetPosition(PathfindingController.pedestalInfos[findClosestPedestal()].position);
    }

    private void SetTargetPosition(Vector3 pos) {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(transform.position, pos);

        if (pathVectorList != null && pathVectorList.Count > 1) {
            pathVectorList.RemoveAt(0);
        }
    }

    private IEnumerator pedetalCoroutine() {
        yield return new WaitForSeconds(pedestalTimeout);
        SetTargetPosition(PathfindingController.pedestalInfos[findClosestPedestal()].position);
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

        StartCoroutine(pedetalCoroutine());
    }

    private void pedestalRepaired(PedestalRepairedEvent event_) {
        PathfindingController.pedestalInfos[event_.pedestalUUID].destroyed = false;
        SetTargetPosition(PathfindingController.pedestalInfos[findClosestPedestal()].position);
    }
}