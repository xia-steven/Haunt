using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HasEnemyHealth))]
public class EnemyBase : MonoBehaviour {
    // Base enemy variables
    protected float baseSpeed = 3.0f;
    private HasEnemyHealth enemyHealth;
    protected Rigidbody rb;
    // Used to determine if the enemy is currently running a coroutine
    protected bool runningCoroutine = false;
    protected EnemyState state = EnemyState.Idle;

    // Enemy attributes 
    protected EnemyAttributes attributes;

    // Pathfinding variables
    protected int currentPathIndex;
    protected List<Vector3> pathVectorList;
    protected Transform tf_;

    protected void Start() {
        // Initialize components and transform
        rb = GetComponent<Rigidbody>();
        enemyHealth = GetComponent<HasEnemyHealth>();
        tf_ = transform;

        // Get enemy attributes
        attributes = EnemyDataManager.GetEnemyAttributes(GetEnemyID());

        // Set max health
        enemyHealth.setMaxHealth(attributes.health);
    }

    protected void FixedUpdate() {
        // Get player/target position
        Vector3 targetPosition = GetTarget();

        // First, check if the enemy cannot pathfind directly to the player/target
        Vector3 playerDirection = (targetPosition - transform.position).normalized;
        RaycastHit hit;
        // Ignore hits on other enemies
        int layerMask = ~LayerMask.GetMask("Enemy");

        if (state != EnemyState.AStarMovement &&
            Physics.Raycast(transform.position, playerDirection, out hit, Vector3.Distance(targetPosition, transform.position), layerMask))
        {
            // Confirm that the raycast did not hit the player
            if (hit.transform.gameObject.tag != "Player")
            {
                if (!runningCoroutine)
                {
                    state = EnemyState.AStarMovement;
                    runningCoroutine = true;
                    StartCoroutine(MoveWithAStar());
                }
                else
                {
                    // Set idle to wait for previous state to finish
                    state = EnemyState.Idle;
                }
                return;
            }
        }

        // Second, check if the enemy can attack the player from their current distance
        if (state != EnemyState.Attacking && Vector3.Distance(targetPosition, transform.position) <= attributes.targetDistance)
        {
            if(!runningCoroutine)
            {
                state = EnemyState.Attacking;
                runningCoroutine = true;
                StartCoroutine(EnemyAttack());
            } else
            {
                // Set idle to wait for previous state to finish
                state = EnemyState.Idle;
            }
            return;
        }

        // Finally, pathfind directly to the player
        if (state != EnemyState.SimpleMovement && Vector3.Distance(targetPosition, transform.position) > attributes.targetDistance)
        {
            if (!runningCoroutine)
            {
                state = EnemyState.SimpleMovement;
                runningCoroutine = true;
                StartCoroutine(MoveStraightToPlayer());
            }
            else
            {
                // Set idle to wait for previous state to finish
                state = EnemyState.Idle;
            }
        }
    }

    /// <summary>
    /// This function is used to get the enemy's attributes from the config value.
    /// Each unique enemy must override this to get the correct attributes
    /// </summary>
    /// <returns></returns>
    public virtual int GetEnemyID()
    {
        // MUST BE OVERRIDDEN TO RETURN CORRECT ENEMY ID
        return 0;
    }

    /// <summary>
    /// This function is used to get the target of this enemy.  Almost always is the player, except 
    /// for cleric enemies.
    /// </summary>
    /// <returns>Returns the target position of the enemy</returns>
    public virtual Vector3 GetTarget()
    {
        return IsPlayer.instance.transform.position;
    }

    /// <summary>
    /// This function is used for the enemy to perform their attack.  
    /// Each unique enemy must override this to perform an attack.
    /// Each overridden function MUST set runningCoroutine to false before exiting
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator EnemyAttack()
    {
        Debug.Log("How did we get here?");
        // Should ALWAYS be overridden
        while (state == EnemyState.Attacking)
        {
            yield return null;
        }
        runningCoroutine = false;
    }

    /// <summary>
    /// This coroutine runs whenever the enemy is moving with A*.  The EnemyState
    /// variable is used to track when to stop.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator MoveWithAStar()
    {
        // Start player position updating
        StartCoroutine(updatePathfindingVector());

        while (state == EnemyState.AStarMovement)
        {
            if (pathVectorList != null)
            {
                var targetPosition = pathVectorList[currentPathIndex] + PathfindingController.map.origin;
                if (Vector3.Distance(transform.position, targetPosition) > 0.5f)
                {
                    var moveDir = (targetPosition - transform.position).normalized;
                    tf_.position += Time.deltaTime * baseSpeed * moveDir * attributes.moveSpeed;
                }
                else
                {
                    currentPathIndex++;
                    if (currentPathIndex >= pathVectorList.Count)
                    {
                        pathVectorList = null;
                        rb.velocity = Vector3.zero;
                    }
                }
            }
            else
            {
                rb.velocity = Vector3.zero;
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
    IEnumerator MoveStraightToPlayer()
    {
        while(state == EnemyState.SimpleMovement)
        {
            // Get player position
            Vector3 playerPosition = IsPlayer.instance.transform.position;

            // Get direction to move
            Vector3 direction = (playerPosition - transform.position).normalized;
            // Remove any y coordinate (shouldn't be any)
            direction.y = 0;

            tf_.position += Time.deltaTime * baseSpeed * direction * attributes.moveSpeed;

            // Only run movement on fixed updates
            yield return new WaitForFixedUpdate();
        }

        runningCoroutine = false;
    }

    IEnumerator updatePathfindingVector()
    {
        while(state == EnemyState.AStarMovement)
        {
            // Set player position
            pathVectorList = Pathfinding.Instance.FindPath(transform.position, IsPlayer.instance.transform.position);

            // Recalculate the player path every second
            yield return new WaitForSeconds(1.0f);
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