using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dodgeRollDuration = 0.5f;
    public float dodgeRollDistance = 5f;
    public float dodgeRollCooldown = 1f;

    private Rigidbody rb;
    private Vector3 movement;
    private float movementX;
    private float movementY;
    private bool isDodging = false;
    private bool dodgePressed = false;
    private float dodgeRollTimer = 0f;
    private float dodgeRollCooldownTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnDodge()
    {
        if (dodgeRollCooldownTimer > 0)
        {
            return;

        }

        dodgePressed = true;

    }
    
    public void OnMove(InputValue movementValue)
    {
        movementX = movementValue.Get<Vector2>().x;
        movementY = movementValue.Get<Vector2>().y;

    }

    private void Update()
    {
        if (!isDodging)
        {
            movement.x = movementX;
            movement.y = movementY;
        }

        if (dodgePressed)
        {
            if (movementX == 0 && movementY == 0)
            {
                dodgePressed = false;
            }
            else
            {
                isDodging = true;
                dodgePressed = false;
                dodgeRollTimer = dodgeRollDuration;
                dodgeRollCooldownTimer = dodgeRollCooldown;

                float dodgeRollDirection = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
                rb.velocity = Quaternion.Euler(0, 0, dodgeRollDirection) * Vector3.right * dodgeRollDistance; 
            }
            
        }

        if (dodgeRollTimer > 0f)
        {
            dodgeRollTimer -= Time.deltaTime;
            if (dodgeRollTimer <= 0f)
            {
                isDodging = false;
                rb.velocity = Vector3.zero;
            }
        }

        if (dodgeRollCooldownTimer > 0f)
        {
            dodgeRollCooldownTimer -= Time.deltaTime;

        }
    }

    private void FixedUpdate()
    {
        if (!isDodging)
        {
            rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement.normalized);
        }
    }
}
