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

    private Transform player;

    private Vector3 offset;

    bool isMoving;
    public bool IsMoving {
        get { return isMoving; }
        set { isMoving = value; }
    }


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        // Room bounds are set per scene
    }

    // Update is called once per frame
    void LateUpdate()
    {
        offset = player.position - transform.position;
        // Remove y offset
        offset.y = 0;

        // Only adjust cam if offset is too high
        if (offset.x < maxXoffset && offset.x > minXoffset && 
            (offset.z + camOffsetFromPlayer.z < maxZoffset && offset.z + camOffsetFromPlayer.z > minZoffset))
        {
            IsMoving = false;
            return;
        }

        IsMoving = true;

        // Make sure offset doesn't exceed values
        offset.x = Mathf.Clamp(offset.x + camOffsetFromPlayer.x, minXoffset, maxXoffset);
        float newXPos = Mathf.Clamp(player.position.x - offset.x + camOffsetFromPlayer.x, xRoomMin, xRoomMax);

        offset.z = Mathf.Clamp(offset.z + camOffsetFromPlayer.z, minZoffset, maxZoffset);
        float newZPos = Mathf.Clamp(player.position.z - offset.z + camOffsetFromPlayer.z, zRoomMin, zRoomMax);
        
        

        Vector3 newPos = Vector3.Lerp(transform.position, (new Vector3(newXPos, this.transform.position.y, newZPos)), camMoveSpeed * Time.deltaTime);
        this.transform.position = newPos;
    }

    public bool InVerticalRoom()
    {
        return zRoomMax != zRoomMin;
    }
}
