using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanFallInPit : MonoBehaviour
{
    private float groundDistance = 0.5f;
    private float pitResetDistance = 1.1f;
    private bool playerEnabled = true;
    private bool overPit = false;
    private bool isDodging = false;
    private Vector3 horizontalOffset;
    Subscription<OverPitEvent> overPitEventSubscription;
    Subscription<DisablePlayerEvent> disablePlayerEventSubscription;
    Subscription<EnablePlayerEvent> enablePlayerEventSubscription;
    Subscription<PlayerDodgeEvent> playerDodgeEventSubscription;
    [SerializeField] SpriteRenderer objectSprite;

    private void Awake()
    {
        overPitEventSubscription = EventBus.Subscribe<OverPitEvent>(_OnOverPit);
        disablePlayerEventSubscription = EventBus.Subscribe<DisablePlayerEvent>(_OnDisableMovement);
        enablePlayerEventSubscription = EventBus.Subscribe<EnablePlayerEvent>(_OnEnableMovement);
        playerDodgeEventSubscription = EventBus.Subscribe<PlayerDodgeEvent>(_OnDodge);
    }

    private void _OnOverPit(OverPitEvent e)
    {
        // Check if event is about this gameObject
        if (e.entered == gameObject)
        {
            horizontalOffset = e.horizontalOffset;
            overPit = e.over;
        }
    }

    private void _OnDodge(PlayerDodgeEvent e)
    {
        if (e.start)
        {
            isDodging = true;
        }
        else
        {
            isDodging = false;
        }
    }

    void _OnDisableMovement(DisablePlayerEvent dpme)
    {
        playerEnabled = false;
    }

    void _OnEnableMovement(EnablePlayerEvent epme)
    {
        playerEnabled = true;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(overPitEventSubscription);
        EventBus.Unsubscribe(disablePlayerEventSubscription);
        EventBus.Unsubscribe(enablePlayerEventSubscription);
        EventBus.Unsubscribe(playerDodgeEventSubscription);
    }

    private void FixedUpdate()
    {
        if (overPit)
        {
            Vector3 rightRayLocation = new Vector3(transform.position.x + 3 * (objectSprite.bounds.size.x / 8), transform.position.y, transform.position.z);
            Vector3 leftRayLocation = new Vector3(transform.position.x - 3 * (objectSprite.bounds.size.x / 8), transform.position.y, transform.position.z);
            Vector3 topRayLocation = new Vector3(transform.position.x, transform.position.y, transform.position.z + 3 * (objectSprite.bounds.size.z / 8));
            Vector3 bottomRayLocation = new Vector3(transform.position.x, transform.position.y, transform.position.z - 3 * (objectSprite.bounds.size.z / 8));

            Ray rightRay = new Ray(rightRayLocation, Vector3.down);
            Ray leftRay = new Ray(leftRayLocation, Vector3.down);
            Ray topRay = new Ray(topRayLocation, Vector3.down);
            Ray bottomRay = new Ray(bottomRayLocation, Vector3.down);

            Debug.DrawRay(rightRay.origin, rightRay.direction * groundDistance, Color.red);
            Debug.DrawRay(leftRay.origin, leftRay.direction * groundDistance, Color.red);
            Debug.DrawRay(topRay.origin, topRay.direction * groundDistance, Color.red);
            Debug.DrawRay(bottomRay.origin, bottomRay.direction * groundDistance, Color.red);

            // Check under object with all four rays - if no ground under all, make player "fall" into pit
            if (!Physics.Raycast(rightRay, groundDistance) && !Physics.Raycast(leftRay, groundDistance) && !Physics.Raycast(topRay, groundDistance) && !Physics.Raycast(bottomRay, groundDistance))
            {
                if (gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    PlayerPit();
                }
                else
                {
                    EnemyPit();
                }
            }
        }
    }

    private void PlayerPit()
    {
        if (!playerEnabled) return;

        if (isDodging) return;

        Debug.Log("Player fell into pit");

        // Play pit falling animation

        Vector3 adjustedPosition = transform.position + (horizontalOffset * pitResetDistance);
        transform.position = adjustedPosition;
        GetComponent<PlayerHasHealth>().AlterHealth(-1);
    }

    private void EnemyPit()
    {
        Debug.Log("Enemy fell into pit");

        // Play pit falling animation

        // Enemy always dies on falling into pit
        Destroy(gameObject);
    }
}
