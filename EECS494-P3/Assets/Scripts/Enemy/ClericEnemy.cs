using System.Collections;
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
    private HasPedestalHealth currAttackingPedestal;

    private float prevTime;
    private const int pedestalTimeout = 6;

    protected override int GetEnemyID() {
        return 9;
    }

    protected override Vector3 GetTarget() {
        return currentTargetPedestal;
    }

    protected override bool needAStar() {
        // First, check if the enemy cannot pathfind directly to the player/target
        var playerDirection = (GetTarget() - transform.position).normalized;
        // Ignore hits on other enemies
        var layerMask = ~LayerMask.GetMask("Enemy");
        if (Physics.Raycast(transform.position, playerDirection, out var hit,
                Vector3.Distance(GetTarget(), transform.position), layerMask)) {
            if (hit.transform.gameObject.CompareTag("Pit") && attributes.isRanged &&
                Vector3.Distance(GetTarget(), transform.position) <= attributes.targetDistance) {
                return false;
            }

            return hit.transform.gameObject.layer != LayerMask.NameToLayer("Pedestal");
        }

        return true;
    }

    protected override bool canAttack(Vector3 targetPosition) {
        return currAttackingPedestal is not null;
    }

    protected new void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);
        if (other.gameObject.layer == LayerMask.NameToLayer("Pedestal")) {
            currAttackingPedestal = other.gameObject.GetComponent<HasPedestalHealth>();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pedestal")) {
            currAttackingPedestal = null;
        }
    }

    private new void Start() {
        base.Start();
        enemyHealth.setClericStatus(true);
        switchPedestalSub = EventBus.Subscribe<PedestalDestroyedEvent>(pedestalDied);
        addPedestalSub = EventBus.Subscribe<PedestalRepairedEvent>(pedestalRepaired);
        currentTargetPedestal = findClosestPedestal();
    }

    // Override attack function
    protected override IEnumerator EnemyAttack() {
        // While attacking
        while (state == EnemyState.Attacking) {
            currAttackingPedestal.AlterHealth(-1);
            if (currAttackingPedestal is null) {
                break;
            }

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
        if (PathfindingController.pedestalInfos.All(static ped => !ped.Value.destroyed)) {
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

    private IEnumerator pedestalCoroutine() {
        yield return new WaitForSeconds(pedestalTimeout);
        if (currAttackingPedestal is null) {
            currentTargetPedestal = findClosestPedestal();
        }
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
        if (currentTargetPedestal == PathfindingController.pedestalInfos[event_.pedestalUUID].position) {
            currentTargetPedestal = findClosestPedestal();
            currAttackingPedestal = null;
        }
    }
}