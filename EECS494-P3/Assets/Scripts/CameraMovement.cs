using Events;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    [SerializeField] private float camMoveSpeed = 0.5f;
    [SerializeField] private float maxXoffset = 1f;
    [SerializeField] private float minXoffset = -1f;
    [SerializeField] private float maxZoffset = 1f;
    [SerializeField] private float minZoffset = -1f;
    [SerializeField] private float xRoomMin; // ADJUST IN SCENES
    [SerializeField] private float xRoomMax; // ADJUST IN SCENES
    [SerializeField] private float zRoomMin; // ADJUST IN SCENES
    [SerializeField] private float zRoomMax; // ADJUST IN SCENES

    [SerializeField] private Vector3 camOffsetFromPlayer = new(0f, 11.5f, -7.0f);
    [SerializeField] private float playerOffsetProportion = 0.6f;


    private Transform player;

    private Vector3 offset;
    private Vector3 playerOffset;
    private Vector3 mousePosition;

    private bool locked;


    public bool IsMoving { get; set; }

    private Subscription<TutorialLockCameraEvent> lockSub;
    private Subscription<TutorialUnlockCameraEvent> unlockSub;

    // Start is called before the first frame update
    private void Start() {
        player = GameObject.Find("Player").transform;
        // Room bounds are set per scene

        lockSub = EventBus.Subscribe<TutorialLockCameraEvent>(onCameraLock);
        unlockSub = EventBus.Subscribe<TutorialUnlockCameraEvent>(onCameraUnlock);
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(lockSub);
        EventBus.Unsubscribe(unlockSub);
    }

    // Update is called once per frame
    private void LateUpdate() {
        switch (locked) {
            case true:
                return;
        }

        var mouse = Input.mousePosition;
        mouse.z = 1;
        if (Camera.main != null) mousePosition = (Camera.main.ScreenToWorldPoint(mouse));
        var normalVec = (new Vector3(0, -1, 1)).normalized;

        mousePosition = mousePosition + normalVec * (-mousePosition.y / normalVec.y);

        offset = player.position * playerOffsetProportion + mousePosition * (1 - playerOffsetProportion);
        // Remove y offset
        offset.y = 0;


        // Make sure offset doesn't exceed values farther than the player (plus a small offset)
        offset.x = Mathf.Clamp(offset.x, minXoffset + player.position.x, maxXoffset + player.position.x);
        var newXPos = Mathf.Clamp(offset.x + camOffsetFromPlayer.x, xRoomMin, xRoomMax);

        offset.z = Mathf.Clamp(offset.z, minZoffset + player.position.z, maxZoffset + player.position.z);
        var newZPos = Mathf.Clamp(offset.z + camOffsetFromPlayer.z, zRoomMin, zRoomMax);


        var newPos = Vector3.Lerp(transform.position, (new Vector3(newXPos, transform.position.y, newZPos)),
            camMoveSpeed * Time.deltaTime);
        transform.position = newPos;
    }

    private void onCameraLock(TutorialLockCameraEvent tlce) {
        locked = true;
        transform.position = tlce.cameraLockedLocation;
    }

    private void onCameraUnlock(TutorialUnlockCameraEvent tuce) {
        locked = false;
    }
}