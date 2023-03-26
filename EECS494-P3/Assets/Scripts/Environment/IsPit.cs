using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPit : MonoBehaviour {
    float pitDepth = 1.0f;
    Vector3 horizontalOffset = Vector3.zero;
    float xSize;
    float zSize;
    private bool isDodging = false;
    private bool overPit = false;
    private float pitResetDistance = 1.1f;

    private Subscription<PlayerDodgeEvent> playerDodgeSubscription;
    private Subscription<OverPitEvent> overPitSubscription;

    private void Start() {
        BoxCollider col = GetComponent<BoxCollider>();
        xSize = col.size.x;
        zSize = col.size.z;

        playerDodgeSubscription = EventBus.Subscribe<PlayerDodgeEvent>(_OnPlayerDodge);
        overPitSubscription = EventBus.Subscribe<OverPitEvent>(_OnOverPit);
    }

    private void _OnPlayerDodge(PlayerDodgeEvent e)
    {
        isDodging = e.start;
    }

    private void _OnOverPit(OverPitEvent e)
    {
        GameObject player = e.player;

        // Checks that player is not dodging and over this current pit
        if (!isDodging && overPit)
        {
            Debug.Log("Player in pit");
            // Add offset to player position in the pit
            Vector3 adjustedPosition = player.transform.position + (horizontalOffset * pitResetDistance);
            // Round values to teleport the player to the center of a square
            // Removed for now as seems to only bring the player close to pits and doesn't seem necessary
            // adjustedPosition.z = Mathf.Round(adjustedPosition.z);
            // adjustedPosition.x = Mathf.Round(adjustedPosition.x);
            // Teleport player outside the pit
            player.transform.position = adjustedPosition;

            EventBus.Publish(new PlayerDamagedEvent(1));
        }
    }


    private void OnTriggerEnter(Collider other) {
        IsPlayer isPlayer = other.GetComponent<IsPlayer>();

        if (isPlayer == null) {
            return;
        }

        overPit = true;

        float xOffsetMag = Mathf.Abs(other.transform.position.x - transform.position.x);
        float zOffsetMag = Mathf.Abs(other.transform.position.z - transform.position.z);

        // Player on the "Left side" of the pit
        if (other.transform.position.x <= transform.position.x && xOffsetMag >= xSize / 2f) {
            horizontalOffset = Vector3.left;
        }
        // Player on the "Right side" of the pit
        else if (other.transform.position.x > transform.position.x && xOffsetMag >= xSize / 2f) {
            horizontalOffset = Vector3.right;
        }
        // Player is "Behind" the pit
        else if (other.transform.position.z <= transform.position.z && zOffsetMag >= zSize / 2f) {
            horizontalOffset = Vector3.back;
        }
        // Player is "In front" of the pit
        else if (other.transform.position.z > transform.position.z && zOffsetMag >= zSize / 2f) {
            horizontalOffset = Vector3.forward;
        }
        // Else not sure where player is, default to left of the pit
        else {
            Debug.LogWarning("Couldn't calculate what direction the player entered the pit area from.");
            horizontalOffset = Vector3.left;
        }

        Debug.Log(horizontalOffset);
    }


    private void OnTriggerExit(Collider other)
    {
        IsPlayer isPlayer = other.GetComponent<IsPlayer>();

        if (isPlayer == null)
        {
            return;
        }

        overPit = false;
    }

    /*
    private void OnTriggerExit(Collider other) {
        IsPlayer isPlayer = other.GetComponent<IsPlayer>();

        if (isPlayer == null) {
            return;
        }


        // If the player fell in the pit
        if (other.transform.position.y < transform.position.y) {
            Debug.Log("Player in pit");
            // Add offset to player position in the pit
            Vector3 adjustedPosition = other.transform.position + teleportOffset(other.transform);
            // Round values to teleport the player to the center of a square
            adjustedPosition.z = Mathf.Round(adjustedPosition.z);
            adjustedPosition.x = Mathf.Round(adjustedPosition.x);
            // Teleport player outside the pit
            other.transform.position = adjustedPosition;

            EventBus.Publish(new PlayerDamagedEvent(1));
        }
    }
    */


    private Vector3 teleportOffset(Transform other) {
        Vector3 offset = Vector3.zero;
        //offset.y = 1.0f + pitDepth;
        offset += horizontalOffset;
        // Make sure player doesn't spawn in the ground
        offset.y = 0.5f;

        return offset;
    }
}