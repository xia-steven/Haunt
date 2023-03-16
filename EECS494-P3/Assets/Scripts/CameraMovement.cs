using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float camMoveSpeed = 0.5f;
    [SerializeField] float maxXoffset = 1f;
    [SerializeField] float minXoffset = -1f;
    [SerializeField] float maxZoffset = 1f;
    [SerializeField] float minZoffset = -1f;
    [SerializeField] float xRoomMin = 0f; // ADJUST IN SCENES
    [SerializeField] float xRoomMax = 0f; // ADJUST IN SCENES
    [SerializeField] float zRoomMin = 0f; // ADJUST IN SCENES
    [SerializeField] float zRoomMax = 0f; // ADJUST IN SCENES

    [SerializeField] Vector3 camOffsetFromPlayer = new Vector3( 0f, 11.5f, -7.0f );

    [Space(10)]
    [Header("New vertical rules for custom level")]
    [SerializeField] bool smartVerticalMovement = false;
    [SerializeField] float allowedOffsetBeforeSwitching = 4;

    private Transform player;

    private Vector3 offset;

    float lastZDirection = 1f;
    float wiggleDirection; 
    float lastPlayerZPosition;

    bool isMoving;
    public bool IsMoving {
        get { return isMoving; }
        set { isMoving = value; }
    }


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        // Get the starter room
        wiggleDirection = 0;
        if (!smartVerticalMovement) camMoveSpeed *= 4;
        // Room bounds are set per scene
    }

    // Update is called once per frame
    void LateUpdate()
    {
        offset = player.position - transform.position;
        // Remove y offset
        offset.y = 0;

        float desiredCameraZOffset = 0;

        if (smartVerticalMovement)
        {
            if (lastZDirection > 0)
            {
                desiredCameraZOffset = minZoffset;
            }
            else
            {
                desiredCameraZOffset = maxZoffset;
            }
        }

        // Only adjust cam if offset is too high
        if (offset.x < maxXoffset && offset.x > minXoffset && 
            (!smartVerticalMovement && offset.z + camOffsetFromPlayer.z < maxZoffset && offset.z + camOffsetFromPlayer.z > minZoffset ||
            smartVerticalMovement && Mathf.Abs(offset.z -  desiredCameraZOffset) <= .02f))
        {
            IsMoving = false;
            return;
        }

        IsMoving = true;

        // Make sure offset doesn't exceed values
        offset.x = Mathf.Clamp(offset.x + camOffsetFromPlayer.x, minXoffset, maxXoffset);
        float newXPos = Mathf.Clamp(player.position.x - offset.x + camOffsetFromPlayer.x, xRoomMin, xRoomMax);

        float newZPos = 0;
        if (!smartVerticalMovement)
        {
            offset.z = Mathf.Clamp(offset.z + camOffsetFromPlayer.z, minZoffset, maxZoffset);
            newZPos = Mathf.Clamp(player.position.z - offset.z + camOffsetFromPlayer.z, zRoomMin, zRoomMax);
        }
        else
        {
            //if holding player at bottom of screen
            if (lastZDirection > 0)
            {
                //if player lower than preferred by too much, change to put player at top of screen
                if (wiggleDirection <= -allowedOffsetBeforeSwitching)
                {
                    lastZDirection = -1; //switch to hold player at bottom of screen
                    wiggleDirection = 0; 
                    desiredCameraZOffset = maxZoffset;
                }
                newZPos = Mathf.Clamp(player.position.z - desiredCameraZOffset + camOffsetFromPlayer.z, zRoomMin, zRoomMax);
            }
            //else hold player at top of screen
            else
            {

                //if player higher than last direction set by too much, change to put player at top of screen
                if (wiggleDirection >= allowedOffsetBeforeSwitching)
                {
                    lastZDirection = 1; //switch to hold player at bottom of screen
                    wiggleDirection = 0; 
                    desiredCameraZOffset = minZoffset;
                }
                newZPos = Mathf.Clamp(player.position.z - desiredCameraZOffset + camOffsetFromPlayer.z, zRoomMin, zRoomMax);
            }
            wiggleDirection += player.position.z - lastPlayerZPosition;
            wiggleDirection = Mathf.Clamp(wiggleDirection, -allowedOffsetBeforeSwitching, allowedOffsetBeforeSwitching);
            lastPlayerZPosition = player.position.z;
        }
        

        Vector3 newPos = Vector3.Lerp(transform.position, (new Vector3(newXPos, this.transform.position.y, newZPos)), camMoveSpeed * Time.deltaTime);
        this.transform.position = newPos;
    }

    public bool InVerticalRoom()
    {
        return zRoomMax != zRoomMin;
    }
}
