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
    private bool isDodging = false;
    private bool dodgePressed = false;
    private float dodgeRollTimer = 1f;
    private float dodgeRollCooldownTimer = 2f;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<TrailRenderer>();
    }

    public void OnDodge() {
        if (dodgeRollCooldownTimer > 0) {
            return;
        }

        dodgePressed = true;
    }

    public void OnMove(InputValue movementValue) {
        movementX = movementValue.Get<Vector2>().x;
        movementZ = movementValue.Get<Vector2>().y; // translate to XZ plane
    }

    public void OnFire() {
        EventBus.Publish<FireEvent>(new FireEvent(this.gameObject));
    }

    public void OnReload() {
        EventBus.Publish<ReloadEvent>(new ReloadEvent(this.gameObject));
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
    private void Update() {
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

        if (dodgeRollTimer > 0f)
        {
            dodgeRollTimer -= Time.deltaTime;
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

    private void FixedUpdate() {
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