using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 5f;
    public float dodgeRollDuration = 0.01f;
    public float dodgeRollSpeed = 10f;
    public float dodgeRollCooldown = 1f;

    private Rigidbody rb;
    private Vector3 movement;
    private float movementX;
    private float movementZ;
    private float dodgeRollCooldownTimer = 0f;
    private bool playerEnabled = true;
    private bool isDodging = false;
    private bool dodgePressed = false;


    Subscription<DisablePlayerEvent> disableMoveSub;
    Subscription<EnablePlayerEvent> enableMoveSub;


    private void Start() {
        rb = GetComponent<Rigidbody>();
        disableMoveSub = EventBus.Subscribe<DisablePlayerEvent>(_OnDisableMovement);
        enableMoveSub = EventBus.Subscribe<EnablePlayerEvent>(_OnEnableMovement);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(disableMoveSub);
        EventBus.Unsubscribe(enableMoveSub);
    }

    public void OnDodge() {
        if (dodgeRollCooldownTimer > 0) {
            return;
        }

        dodgePressed = true;
    }

    public void OnMove(InputValue movementValue) {
        movementX = movementValue.Get<Vector2>().x;
        movementZ = movementValue.Get<Vector2>().y;
    }

    public void OnFire() {
        if (!playerEnabled) return;
        EventBus.Publish<FireEvent>(new FireEvent(this.gameObject));
    }

    public void OnReload()
    {
        if (!playerEnabled) return;
        EventBus.Publish<ReloadEvent>(new ReloadEvent(this.gameObject));
    }

    void _OnDisableMovement(DisablePlayerEvent dpme)
    {
        playerEnabled = false;
    }

    void _OnEnableMovement(EnablePlayerEvent epme)
    {
        playerEnabled = true;
    }

    private void Update() {
        if (!playerEnabled) return;

        if (!isDodging) {
            movement.x = movementX;
            movement.z = movementZ;
        }

        if (dodgePressed && !isDodging) {
            if (movementX == 0 && movementZ == 0) {
                dodgePressed = false;
            }
            else {
                StartCoroutine(Dodge());
            }
        }
    }

    IEnumerator Dodge() {
        isDodging = true;

        float initial_time = Time.time;
        float progress = 0;

        rb.useGravity = false;
        rb.velocity = movement.normalized * dodgeRollSpeed;

        while (progress < 1.0f && playerEnabled) {
            progress = (Time.time - initial_time) / dodgeRollDuration;

            yield return null;
        }

        rb.velocity = Vector3.zero;
        rb.useGravity = true;
        isDodging = false;
    }


    private void FixedUpdate()
    {
        if (!playerEnabled) return;

        if (!isDodging) {
            rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement.normalized);
        }
    }
}