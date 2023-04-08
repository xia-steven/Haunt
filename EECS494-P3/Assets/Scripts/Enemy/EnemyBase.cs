using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HasEnemyHealth))]
public class EnemyBase : MonoBehaviour {
    // Base enemy variables
    protected float baseSpeed = 3.0f;
    protected HasEnemyHealth enemyHealth;

    protected Rigidbody rb;

    // Used to determine if the enemy is currently running a coroutine
    protected bool runningCoroutine;
    protected EnemyState state = EnemyState.Idle;

    // Enemy attributes
    protected EnemyAttributes attributes;

    // Pathfinding variables
    protected int currentPathIndex;
    protected List<Vector3> pathVectorList;
    private Transform tf_;

    // Player variables
    private PlayerHasHealth playerHealth;

    protected virtual void Start() {
        // Initialize components and transform
        rb = GetComponent<Rigidbody>();
        enemyHealth = GetComponent<HasEnemyHealth>();
        tf_ = transform;

        // Get enemy attributes
        attributes = EnemyDataManager.GetEnemyAttributes(GetEnemyID());

        // Set max health
        enemyHealth.setMaxHealth(attributes.health);

        // Get player attributes
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHasHealth>();
    }

    protected void FixedUpdate() {
        if (state != EnemyState.AStarMovement && needAStar()) {
            if (!runningCoroutine) {
                state = EnemyState.AStarMovement;
                runningCoroutine = true;
                PathfindingController.FindClosestWalkable(GetTarget(), out var x, out var y);
                StartCoroutine(MoveWithAStar(x, y));
            }
            else {
                // Set idle to wait for previous state to finish
                state = EnemyState.Idle;
            }

            return;
        }

        // Second, check if the enemy can attack the player from their current distance
        if (state != EnemyState.Attacking && canAttack(GetTarget())) {
            if (!runningCoroutine) {
                state = EnemyState.Attacking;
                runningCoroutine = true;
                StartCoroutine(EnemyAttack());
            }
            else {
                // Set idle to wait for previous state to finish
                state = EnemyState.Idle;
            }

            return;
        }

        // Finally, pathfind directly to the player
        if (state != EnemyState.SimpleMovement &&
            Vector3.Distance(GetTarget(), transform.position) > attributes.targetDistance) {
            if (!runningCoroutine) {
                state = EnemyState.SimpleMovement;
                runningCoroutine = true;
                StartCoroutine(MoveStraightToTarget());
            }
            else {
                // Set idle to wait for previous state to finish
                state = EnemyState.Idle;
            }
        }
    }

    protected virtual bool canAttack(Vector3 targetPosition) {
        return Vector3.Distance(targetPosition, transform.position) <= attributes.targetDistance;
    }

    protected virtual bool needAStar() {
        // First, check if the enemy cannot pathfind directly to the player/target
        var playerDirection = (GetTarget() - transform.position).normalized;
        var originOffset = Quaternion.AngleAxis(90, Vector3.up) * playerDirection;
        var ray1Origin = transform.position + originOffset;
        var ray2Origin = transform.position - originOffset;
        var ray1Dir = (GetTarget() - ray1Origin).normalized;
        var ray2Dir = (GetTarget() - ray2Origin).normalized;
        // Ignore hits on other enemies
        var lm = ~LayerMask.GetMask("Enemy");
        var ray1 = Physics.Raycast(ray1Origin, ray1Dir, out var hit1, Vector3.Distance(GetTarget(), ray1Origin), lm);
        var ray2 = Physics.Raycast(ray2Origin, ray2Dir, out var hit2, Vector3.Distance(GetTarget(), ray2Origin), lm);

        if (ray1 && ray2) {
            if (hit1.transform.gameObject.CompareTag("Pit") && hit2.transform.gameObject.CompareTag("Pit") &&
                attributes.isRanged &&
                Vector3.Distance(GetTarget(), transform.position) <= attributes.targetDistance) {
                return false;
            }
        }

        return !hit1.transform.gameObject.CompareTag("Player") || !hit2.transform.gameObject.CompareTag("Player");
    }

    /// <summary>
    /// This function is used to get the enemy's attributes from the config value.
    /// Each unique enemy must override this to get the correct attributes
    /// </summary>
    /// <returns></returns>
    protected virtual int GetEnemyID() {
        // MUST BE OVERRIDDEN TO RETURN CORRECT ENEMY ID
        return 0;
    }

    /// <summary>
    /// This function is used to get the target of this enemy.  Almost always is the player, except
    /// for cleric enemies.
    /// </summary>
    /// <returns>Returns the target position of the enemy</returns>
    protected virtual Vector3 GetTarget() {
        return IsPlayer.instance.transform.position;
    }

    /// <summary>
    /// This function is used for the enemy to perform their attack.
    /// Each unique enemy must override this to perform an attack.
    /// Each overridden function MUST set runningCoroutine to false before exiting
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator EnemyAttack() {
        // Should ALWAYS be overridden
        while (state == EnemyState.Attacking) {
            yield return null;
        }

        runningCoroutine = false;
    }

    /// <summary>
    /// This coroutine runs whenever the enemy is moving with A*.  The EnemyState
    /// variable is used to track when to stop.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveWithAStar(int targetX, int targetZ) {
        // Start player position updating
        StartCoroutine(updatePathfindingVector(targetX, targetZ));

        while (state == EnemyState.AStarMovement) {
            if (pathVectorList != null) {
                if (currentPathIndex >= pathVectorList.Count) {
                    currentPathIndex = pathVectorList.Count - 1;
                }

                var targetPosition = pathVectorList[currentPathIndex] + PathfindingController.map.origin;
                if (Vector3.Distance(transform.position, targetPosition) > 0.1f) {
                    var moveDir = (targetPosition - transform.position).normalized;
                    tf_.position += Time.deltaTime * baseSpeed * attributes.moveSpeed * moveDir;
                }
                else {
                    if (++currentPathIndex >= pathVectorList.Count) {
                        pathVectorList = null;
                        currentPathIndex = 0;
                        rb.velocity = Vector3.zero;
                    }
                }
            }
            else {
                rb.velocity = Vector3.zero;
                currentPathIndex = 0;
            }

            // Only run movement on fixed updates
            yield return new WaitForFixedUpdate();
        }

        runningCoroutine = false;
    }

    /// <summary>
    /// This coroutine runs whenever the enemy is moving directly towards the player.
    /// The EnemyState variable used to track when to stop.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveStraightToTarget() {
        while (state == EnemyState.SimpleMovement) {
            // Get direction to move
            var direction = (GetTarget() - transform.position).normalized;
            // Remove any y coordinate (shouldn't be any)
            direction.y = 0;

            tf_.position += Time.deltaTime * baseSpeed * attributes.moveSpeed * direction;

            // Only run movement on fixed updates
            yield return new WaitForFixedUpdate();
        }

        runningCoroutine = false;
    }

    private IEnumerator updatePathfindingVector(int targetX, int targetZ) {
        while (state == EnemyState.AStarMovement) {
            // Set player position
            pathVectorList = Pathfinding.Instance.FindPath(transform.position, targetX, targetZ);

            // Recalculate the player path every second
            yield return new WaitForSeconds(1.0f);
        }
    }

    // Base function to use for firing projectiles
    protected void fireBullet(GameObject bullet, Vector3 direction, Shooter shooter, float projectileSpeed) {
        // Spawn bullet at barrel of gun
        var projectile = Instantiate(bullet, transform.position, Quaternion.identity);

        // Set shooter to holder of gun (enemy or player)
        projectile.GetComponent<Bullet>().SetShooter(shooter);

        // Give bullet its velocity
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
    }

    // Damage the player if they touch the enemy
    protected void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            playerHealth.AlterHealth(-1, DeathCauses.Enemy);
        }
    }
}


public enum EnemyState {
    AStarMovement,
    SimpleMovement,
    Attacking,

    // Initialization state
    Idle
}