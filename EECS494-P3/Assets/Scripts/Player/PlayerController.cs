using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 5f;
    public float dodgeRollDuration = 0.1f;
    public float dodgeRollSpeed = 10f;
    public float dodgeRollCooldown = 1f;

    private Rigidbody rb;
    private TrailRenderer tr;
    private Vector3 movement;
    private float movementX;
    private float movementZ;
    private float dodgeRollCooldownTimer = 0f;
    private bool playerEnabled = true;
    private bool isDodging = false;
    private bool dodgePressed = false;
    private float dodgeRollTimer = 1f;
    Subscription<DisablePlayerEvent> disableMoveSub;
    Subscription<EnablePlayerEvent> enableMoveSub;



    private void Start() {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<TrailRenderer>();
        disableMoveSub = EventBus.Subscribe<DisablePlayerEvent>(_OnDisableMovement);
        enableMoveSub = EventBus.Subscribe<EnablePlayerEvent>(_OnEnableMovement);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(disableMoveSub);
        EventBus.Unsubscribe(enableMoveSub);
    }

    public void OnDodge(InputAction.CallbackContext value) {
        if (dodgeRollCooldownTimer > 0) {
            return;
        }

        dodgePressed = true;
    }

    public void OnMove(InputAction.CallbackContext value) {
        movementX = value.ReadValue<Vector2>().x;
        movementZ = value.ReadValue<Vector2>().y;
    }

    public void OnFire(InputAction.CallbackContext value) {
        if (!playerEnabled) return;
        if (value.started)
        {
            EventBus.Publish<FireEvent>(new FireEvent(this.gameObject, true));
        } else if (value.canceled)
        {
            EventBus.Publish<FireEvent>(new FireEvent(this.gameObject, false));
        }
    }

    public void OnReload(InputAction.CallbackContext value) {
        if (!playerEnabled) return;
        EventBus.Publish<ReloadEvent>(new ReloadEvent(this.gameObject));
    }

    public void OnSwapWeapon(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            if (value.ReadValue<float>() > 0)
            {
                EventBus.Publish<SwapEvent>(new SwapEvent(1));
            }
            else if (value.ReadValue<float>() < 0)
            {
                EventBus.Publish<SwapEvent>(new SwapEvent(-1));
            }
        }
    }

    private void StartDodge()
    {
        isDodging = true;
        rb.useGravity = false;
        dodgeRollTimer = dodgeRollDuration;
        dodgeRollCooldownTimer = dodgeRollCooldown;
        rb.velocity = movement.normalized * dodgeRollSpeed;

        tr.emitting = true;
    }

    private void StopDodge()
    {
        dodgeRollTimer = 0;
        rb.useGravity = true;
        isDodging = false;
        rb.velocity = Vector3.zero;
        
        tr.emitting = false;
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
            movement.y = 0f;
            movement.z = movementZ;
        }

        if (dodgePressed)
        {
            dodgePressed = false;

            if (movementX != 0 || movementZ != 0)
            {
                StartDodge();
            }
        }
        
        // enter this condtl during a dodge
        if (dodgeRollTimer > 0f)
        {
            dodgeRollTimer -= Time.deltaTime;
            if (!playerEnabled)
            {
                StopDodge();
            }
        }

        if (isDodging && dodgeRollTimer <= 0f)
        {
            StopDodge();
        }
        

        if (dodgeRollCooldownTimer > 0f)
        {
            dodgeRollCooldownTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (!playerEnabled) return;

        if (!isDodging) {
            rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement.normalized);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // manually reset dodge (cancel it) if we hit a wall;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))    
        {
            StopDodge();
        }
    }
}