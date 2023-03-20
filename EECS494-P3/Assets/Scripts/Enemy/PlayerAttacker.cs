using UnityEngine;

public class PlayerAttacker : EnemyBase {
    private Subscription<PlayerPositionEvent> positionSub;

    private new void Start() {
        base.Start();
        speed = 5f;
        positionSub = EventBus.Subscribe<PlayerPositionEvent>(SetTargetPosition);
    }

    private new void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            EventBus.Publish(new PlayerDamagedEvent());
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("PlayerUtility")) {
            //base.OnTriggerEnter(other);
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