using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 5f;
    public float dodgeRollDuration = 0.1f;
    public float dodgeRollSpeed = 10f;
    public float dodgeRollCooldown = 1f;

    const float extraRayDist = .05f;

    private Rigidbody rb;
    private Collider col;
    private TrailRenderer tr;
    private Animator animator;
    private PlayerInput pi;
    private Vector3 movement;
    private float movementX;
    private float movementZ;
    private float dodgeRollCooldownTimer = 0f;
    private bool playerEnabled = true;
    private bool isDodging = false;
    private bool dodgePressed = false;
    private float dodgeRollTimer = 1f;
    private float lastSound = 0f;
    private float walkSoundDelay = 0.4f;
    private AudioClip walkSound;
    private AudioClip dodgeSound;
    Subscription<DisablePlayerEvent> disableMoveSub;
    Subscription<EnablePlayerEvent> enableMoveSub;
    Subscription<TutorialDodgeStartEvent> dodgeStartSub;
    Subscription<TutorialDodgeEndEvent> dodgeEndSub;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        tr = GetComponent<TrailRenderer>();
        animator = GetComponent<Animator>();
        pi = GetComponent<PlayerInput>();
        disableMoveSub = EventBus.Subscribe<DisablePlayerEvent>(_OnDisableMovement);
        enableMoveSub = EventBus.Subscribe<EnablePlayerEvent>(_OnEnableMovement);
        dodgeStartSub = EventBus.Subscribe<TutorialDodgeStartEvent>(StartDodge);
        dodgeEndSub = EventBus.Subscribe<TutorialDodgeEndEvent>(StopDodge);
        walkSound = Resources.Load<AudioClip>("Audio/Movement/Walk");
        dodgeSound = Resources.Load<AudioClip>("Audio/Movement/Dodge");
    }

    
    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        Debug.Log("Attempting to change player control scheme");
        if (s.name == "HubWorld" || s.name == "TutorialHubWorld")
        {
            Debug.Log("Player control scheme set to Hub");
            StartCoroutine(LoadControls("Hub"));
        }
        else
        {
            Debug.Log("Player control scheme set to Gameplay");
            StartCoroutine(LoadControls("Player"));
        }
    }

    // Allows for delay before attempting controls switch
    private IEnumerator LoadControls(string controls)
    {
        yield return new WaitForSeconds(0.1f);

        if (pi == null)
        {
            pi = GetComponent<PlayerInput>();
        }

        pi.SwitchCurrentActionMap(controls);
    }
    

    private void OnDestroy() {
        EventBus.Unsubscribe(disableMoveSub);
        EventBus.Unsubscribe(enableMoveSub);
        EventBus.Unsubscribe(dodgeStartSub);
        EventBus.Unsubscribe(dodgeEndSub);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnDodge(InputAction.CallbackContext value) {
        if (dodgeRollCooldownTimer > 0 || !playerEnabled) {
            return;
        }

        if (value.started) {
            dodgePressed = true;
        }
    }

    public void OnMove(InputAction.CallbackContext value) {
        if (!playerEnabled) return;

        movementX = value.ReadValue<Vector2>().x;
        movementZ = value.ReadValue<Vector2>().y;

        Vector3 rayStart = col.bounds.center - Vector3.up * (col.bounds.extents.y - .1f);

        float horizontalExtent = col.bounds.extents.x + extraRayDist;
        float verticalExtent = col.bounds.extents.z + extraRayDist;

        int ignoreMask = ~LayerMask.GetMask("Special", "Tutorial", "PlayerUtility");

        RaycastHit ray1;
        RaycastHit ray2;
        RaycastHit ray3;
        RaycastHit ray4;

        Debug.DrawRay(rayStart, Vector3.right * horizontalExtent);
        // left/right checks
        if ((Physics.Raycast(rayStart, Vector3.left, out ray1, horizontalExtent, ignoreMask) && movementX < -.1f) ||
            (Physics.Raycast(rayStart, Vector3.right, out ray2, horizontalExtent, ignoreMask) && movementX > .1f))
            movementX = 0;
        if ((Physics.Raycast(rayStart, Vector3.forward, out ray3, verticalExtent, ignoreMask) && movementZ > .1f) ||
            (Physics.Raycast(rayStart, Vector3.back, out ray4, verticalExtent, ignoreMask) && movementZ < -.1f))
            movementZ = 0;

        animator.SetBool("walking", true);
    }

    public void OnFire(InputAction.CallbackContext value) {
        if (!playerEnabled) return;
        if (value.started) {
            EventBus.Publish<FireEvent>(new FireEvent(this.gameObject, true));
        }
        else if (value.canceled) {
            EventBus.Publish<FireEvent>(new FireEvent(this.gameObject, false));
        }
    }

    public void OnReload(InputAction.CallbackContext value) {
        if (!playerEnabled) return;
        if (value.started) {
            EventBus.Publish<ReloadEvent>(new ReloadEvent(this.gameObject));
        }
    }

    public void OnSwapWeapon(InputAction.CallbackContext value) {
        // Disable swapping when disabled
        if (!playerEnabled) return;

        if (value.started) {
            if (value.ReadValue<float>() > 0) {
                EventBus.Publish<SwapEvent>(new SwapEvent(1));
            }
            else if (value.ReadValue<float>() < 0) {
                EventBus.Publish<SwapEvent>(new SwapEvent(-1));
            }
        }
    }

    public void OnSwapSpecificWeapon(InputAction.CallbackContext value) {
        if (value.started) {
            EventBus.Publish<SwapSpecificEvent>(new SwapSpecificEvent(int.Parse(value.control.name)));
        }
    }

    public void OnInteract(InputAction.CallbackContext value) {
        // Disable interacting when player is disabled
        if (!playerEnabled) return;

        if (value.started) {
            TryInteractEvent e = new TryInteractEvent();
            e.button = value.control.name;
            EventBus.Publish(e);
        }
    }

    private void StartDodge(TutorialDodgeStartEvent tutorDodge = null) {
        EventBus.Publish<PlayerDodgeEvent>(new PlayerDodgeEvent(true, movement));
        isDodging = true;
        //rb.useGravity = false;
        dodgeRollTimer = dodgeRollDuration;
        dodgeRollCooldownTimer = dodgeRollCooldown;
        if (tutorDodge != null) {
            rb.velocity = tutorDodge.direction * dodgeRollSpeed;
        }
        else {
            rb.velocity = movement.normalized * dodgeRollSpeed;
        }

        tr.emitting = true;
        animator.SetBool("walking", false);
        AudioSource.PlayClipAtPoint(dodgeSound, transform.position);
    }

    private void StopDodge(TutorialDodgeEndEvent tutorDodge = null) {
        EventBus.Publish<PlayerDodgeEvent>(new PlayerDodgeEvent(false, movement));
        dodgeRollTimer = 0;
        isDodging = false;
        rb.velocity = Vector3.zero;

        tr.emitting = false;
        animator.SetBool("walking", true);
    }

    void _OnDisableMovement(DisablePlayerEvent dpme) {
        playerEnabled = false;
        movementX = 0f;
        movementZ = 0f;
        if (isDodging)
            StopDodge();
        if(!dpme.keepAnimatorEnabled)
        {
            animator.SetBool("walking", false);
        }
    }

    void _OnEnableMovement(EnablePlayerEvent epme) {
        playerEnabled = true;
    }

    private void Update() {
        if (!playerEnabled) return;

        animator.SetFloat("walkingSpeed", PlayerModifiers.moveSpeed);

        if (!isDodging) {
            movement.x = movementX;
            movement.y = 0f;
            movement.z = movementZ;
            if (movement == Vector3.zero) {
                animator.SetBool("walking", false);
            }
        }

        if (dodgePressed) {
            dodgePressed = false;

            if (movementX != 0 || movementZ != 0) {
                StartDodge();
            }
        }

        // enter this condtl during a dodge
        if (dodgeRollTimer > 0f) {
            dodgeRollTimer -= Time.deltaTime;
            if (!playerEnabled) {
                StopDodge();
            }
        }

        if (isDodging && dodgeRollTimer <= 0f) {
            StopDodge();
        }


        if (dodgeRollCooldownTimer > 0f) {
            dodgeRollCooldownTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate() {
        if (!playerEnabled) return;

        if (!isDodging) {
            //Debug.Log(IsWall(movement));
            rb.MovePosition(rb.position +
                            moveSpeed * PlayerModifiers.moveSpeed * Time.fixedDeltaTime * movement.normalized);
            // Play move sound if delay has passed and moving
            if (Time.time - lastSound >= walkSoundDelay && movement != Vector3.zero) {
                AudioSource.PlayClipAtPoint(walkSound, transform.position);
                lastSound = Time.time;
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        // manually reset dodge (cancel it) if we hit a wall when not perpendicular to the wall;
        Vector3 normal = collision.contacts[0].normal;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && Vector3.Angle(rb.velocity, -normal) < 85) {
            StopDodge();
        }
    }
}