using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPit : MonoBehaviour {
    Vector3 horizontalOffset = Vector3.zero;
    float xSize;
    float zSize;

    private void Start() {
        BoxCollider col = GetComponent<BoxCollider>();
        xSize = col.size.x;
        zSize = col.size.z;
    }

    private void OnTriggerEnter(Collider other) {
        float xOffsetMag = Mathf.Abs(other.transform.position.x - transform.position.x);
        float zOffsetMag = Mathf.Abs(other.transform.position.z - transform.position.z);

        // Player on the "Left side" of the pit
        if (other.transform.position.x <= transform.position.x && xOffsetMag >= xSize / 2f) {
            horizontalOffset = Vector3.left * (xSize / 2f) + Vector3.left;
        }
        // Player on the "Right side" of the pit
        else if (other.transform.position.x > transform.position.x && xOffsetMag >= xSize / 2f) {
            horizontalOffset = Vector3.right * (xSize / 2f) + Vector3.right;
        }
        // Player is "Behind" the pit
        else if (other.transform.position.z <= transform.position.z && zOffsetMag >= zSize / 2f) {
            horizontalOffset = Vector3.back * (zSize / 2f) + Vector3.back;
        }
        // Player is "In front" of the pit
        else if (other.transform.position.z > transform.position.z && zOffsetMag >= zSize / 2f) {
            horizontalOffset = Vector3.forward * (zSize / 2f) + Vector3.forward;
        }
        // Else not sure where player is, default to left of the pit
        else {
            Debug.LogWarning("Couldn't calculate what direction the player entered the pit area from.");
            horizontalOffset = Vector3.left * (xSize / 2f) + Vector3.left;
        }

        BoxCollider col = GetComponent<BoxCollider>();
        Vector3 relocationPosition = horizontalOffset + transform.position + col.center;
        relocationPosition = new Vector3(relocationPosition.x, 0.5f, relocationPosition.z);

        Debug.Log("Relocation position: " + relocationPosition);

        EventBus.Publish<OverPitEvent>(new OverPitEvent(other.gameObject, relocationPosition, true));
    }


    private void OnTriggerExit(Collider other)
    {
        EventBus.Publish<OverPitEvent>(new OverPitEvent(other.gameObject, horizontalOffset, false));
    }

    private Vector3 teleportOffset(Transform other) {
        Vector3 offset = Vector3.zero;
        //offset.y = 1.0f + pitDepth;
        offset += horizontalOffset;
        // Make sure player doesn't spawn in the ground
        offset.y = 0.5f;

        return offset;
    }
}

public class OverPitEvent 
{
    public GameObject entered;
    public Vector3 horizontalOffset;
    public bool over;
    public OverPitEvent(GameObject _entered, Vector3 _horizontalOffset, bool _over)
    {
        entered = _entered;
        horizontalOffset = _horizontalOffset;
        over = _over;
    }
}