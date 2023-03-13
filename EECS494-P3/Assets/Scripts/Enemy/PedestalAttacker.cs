using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestalAttacker : EnemyBase {
    private Dictionary<int, Vector3> pedestalPositions;
    private Subscription<PedestalDestroyedEvent> switchPedestalSub;

    private new void Start() {
        base.Start();
        speed = 4f;
        pedestalPositions = new Dictionary<int, Vector3>
            { { 1, new Vector3(10, 0, 0) }, { 2, new Vector3(1000, 0, 2) }, { 3, new Vector3(1000, 0, 3) } };
        switchPedestalSub = EventBus.Subscribe<PedestalDestroyedEvent>(SetTargetPosition);
        StartCoroutine(WaitAndFindPath());
    }

    private new void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pedestal")) {
            Debug.Log("Hit the pedestal");
            var h = other.gameObject.GetComponent<HasPedestalHealth>();
            if (h != null) {
                h.AlterHealth(-3);
            }
        }
        else if (!other.CompareTag("Player")) {
            base.OnTriggerEnter(other);
        }
    }

    private int findClosestPedestal() {
        var closestDist = float.MaxValue;
        var closest = -1;
        foreach (var ped in pedestalPositions) {
            var distance = Vector3.Distance(transform.position, ped.Value);
            if (distance < closestDist) {
                closestDist = distance;
                closest = ped.Key;
            }
        }

        return closest;
    }

    private IEnumerator WaitAndFindPath() {
        yield return null;
        SetTargetPosition(pedestalPositions[findClosestPedestal()]);
    }

    private void SetTargetPosition(Vector3 pos) {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(transform.position, pos);

        if (pathVectorList != null && pathVectorList.Count > 1) {
            pathVectorList.RemoveAt(0);
        }
    }

    private void SetTargetPosition(PedestalDestroyedEvent event_) {
        pedestalPositions.Remove(event_.pedestalUUID);
        SetTargetPosition(pedestalPositions[findClosestPedestal()]);
    }
}