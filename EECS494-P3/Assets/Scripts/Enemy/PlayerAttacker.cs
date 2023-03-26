using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttacker : EnemyBase {
    private Subscription<PlayerPositionEvent> positionSub;

    public bool is_ranged;
    public float range = 4;
    private Vector3 curr_player_pos;

    private new void Start() {
        curr_player_pos = new Vector3();
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
        curr_player_pos = event_.position;
        pathVectorList = Pathfinding.Instance.FindPath(transform.position, curr_player_pos);

        if (pathVectorList != null && pathVectorList.Count > 1) {
            pathVectorList.RemoveAt(0);
        }
    }

    private new void FixedUpdate() {
        if (is_ranged && Vector3.Distance(transform.position, curr_player_pos) < range) {
            rb.velocity = Vector3.zero;
            return;
        }

        base.FixedUpdate();
    }
}