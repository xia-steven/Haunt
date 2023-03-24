using UnityEngine;

public class PlayerAttacker : EnemyBase {
    private Subscription<PlayerPositionEvent> positionSub;

    private new void Start() {
        base.Start();
        positionSub = EventBus.Subscribe<PlayerPositionEvent>(SetTargetPosition);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            EventBus.Publish(new PlayerDamagedEvent());
        }
    }

    private void SetTargetPosition(PlayerPositionEvent event_) {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(transform.position, event_.position);

        if (pathVectorList != null && pathVectorList.Count > 1) {
            pathVectorList.RemoveAt(0);
        }
    }
}