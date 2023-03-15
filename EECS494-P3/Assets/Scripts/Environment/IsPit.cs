using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPit : MonoBehaviour {
    float pitDepth = 1.0f;
    Vector3 horizontalOffset = Vector3.zero;
    float xSize;
    float zSize;

    private void Start() {
        BoxCollider col = GetComponent<BoxCollider>();
        xSize = col.size.x;
        zSize = col.size.z;
    }


    private void OnTriggerEnter(Collider other) {
        IsPlayer isPlayer = other.GetComponent<IsPlayer>();

        if (isPlayer == null) {
            return;
        }


        Debug.Log("Player over the pit");

        Debug.Log("Our position: " + transform.position);
        Debug.Log("Player position " + other.transform.position);

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
    }

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


    private Vector3 teleportOffset(Transform other) {
        Vector3 offset = Vector3.zero;
        offset.y = 1.0f + pitDepth;
        offset += horizontalOffset;

        return offset;
    }
}