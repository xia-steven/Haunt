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
    [SerializeField] float playerOffsetProportion = 0.6f;


    private Transform player;

    private Vector3 offset;
    private Vector3 playerOffset;
    private Vector3 mousePosition;

    bool locked = false;


    bool isMoving;
    public bool IsMoving {
        get { return isMoving; }
        set { isMoving = value; }
    }

    Subscription<TutorialLockCameraEvent> lockSub;
    Subscription<TutorialUnlockCameraEvent> unlockSub;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        // Room bounds are set per scene

        lockSub = EventBus.Subscribe<TutorialLockCameraEvent>(onCameraLock);
        unlockSub = EventBus.Subscribe<TutorialUnlockCameraEvent>(onCameraUnlock);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(lockSub);
        EventBus.Unsubscribe(unlockSub);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(locked)
        {
            return;
        }
        Vector3 mouse = Input.mousePosition;
        mouse.z = 1;
        mousePosition = (Camera.main.ScreenToWorldPoint(mouse));
        Vector3 normalVec = (new Vector3(0, -1, 1)).normalized;

        mousePosition = mousePosition + normalVec * (-mousePosition.y / normalVec.y); 

        offset = player.position * playerOffsetProportion + mousePosition * (1 - playerOffsetProportion);
        // Remove y offset
        offset.y = 0;


        // Make sure offset doesn't exceed values farther than the player (plus a small offset)
        offset.x = Mathf.Clamp(offset.x, minXoffset + player.position.x, maxXoffset + player.position.x);
        float newXPos = Mathf.Clamp(offset.x + camOffsetFromPlayer.x, xRoomMin, xRoomMax);

        offset.z = Mathf.Clamp(offset.z, minZoffset + player.position.z, maxZoffset + player.position.z);
        float newZPos = Mathf.Clamp(offset.z + camOffsetFromPlayer.z, zRoomMin, zRoomMax);


        Vector3 newPos = Vector3.Lerp(transform.position, (new Vector3(newXPos, this.transform.position.y, newZPos)), camMoveSpeed * Time.deltaTime);
        this.transform.position = newPos;
    }

    void onCameraLock(TutorialLockCameraEvent tlce)
    {
        locked = true;
        transform.position = tlce.cameraLockedLocation;
    }

    void onCameraUnlock(TutorialUnlockCameraEvent tuce)
    {
        locked = false;
    }
}
