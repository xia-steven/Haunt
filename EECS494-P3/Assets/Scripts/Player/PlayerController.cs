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
    private bool isDodging = false;
    private bool dodgePressed = false;
    private float dodgeRollCooldownTimer = 0f;


    private void Start() {
        rb = GetComponent<Rigidbody>();
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

    public void OnFire(InputValue value) {
        Debug.Log(value);
        EventBus.Publish<FireEvent>(new FireEvent(this.gameObject));
    }

    public void OnReload() {
        EventBus.Publish<ReloadEvent>(new ReloadEvent(this.gameObject));
    }

    private void Update() {
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

        while (progress < 1.0f) {
            progress = (Time.time - initial_time) / dodgeRollDuration;

            yield return null;
        }

        rb.velocity = Vector3.zero;
        rb.useGravity = true;
        isDodging = false;
    }


    private void FixedUpdate() {
        if (!isDodging) {
            rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement.normalized);
        }
    }
}