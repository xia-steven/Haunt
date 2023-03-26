using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestalInfo {
    public Vector3 position;
    public bool destroyed = true;

    public PedestalInfo(Vector3 pos) {
        position = pos;
    }
}

public class PedestalAttacker : EnemyBase {

    public static Dictionary<int, PedestalInfo> pedestalInfos = new Dictionary<int, PedestalInfo> {
        { 1, new PedestalInfo(new Vector3(10, 0, 0)) }, { 2, new PedestalInfo(new Vector3(-10, 0, 0)) },
        { 3, new PedestalInfo(new Vector3(0, 0, -9)) }
    };

    private Subscription<PedestalDestroyedEvent> switchPedestalSub;
    private Subscription<PedestalRepairedEvent> addPedestalSub;

    private float prevTime;
    public int pedestalTimeout;

    private new void Start() {
        base.Start();
        speed = 1.5f;
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
        var closestDist = float.MaxValue;
        var closest = (int)Random.Range(1f, 3.99f);
        while (!pedestalInfos[closest].destroyed) {
            closest = (int)Random.Range(1f, 3.99f);
        }
        foreach (var ped in pedestalInfos) {
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
        SetTargetPosition(pedestalInfos[findClosestPedestal()].position);
    }

    private void SetTargetPosition(Vector3 pos) {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(transform.position, pos);

        if (pathVectorList != null && pathVectorList.Count > 1) {
            pathVectorList.RemoveAt(0);
        }
    }

    private IEnumerator pedetalCoroutine(int uuid) {
        yield return new WaitForSeconds(pedestalTimeout);
        SetTargetPosition(pedestalInfos[findClosestPedestal()].position);
    }

    private void pedestalDied(PedestalDestroyedEvent event_) {
        switch (event_.pedestalUUID) {
            case 1:
                pedestalInfos[1].destroyed = true;
                break;
            case 2:
                pedestalInfos[2].destroyed = true;
                break;
            case 3:
                pedestalInfos[3].destroyed = true;
                break;
        }

        StartCoroutine(pedetalCoroutine(event_.pedestalUUID));
    }

    private void pedestalRepaired(PedestalRepairedEvent event_) {
        pedestalInfos[event_.pedestalUUID].destroyed = false;
        SetTargetPosition(pedestalInfos[findClosestPedestal()].position);
    }
}