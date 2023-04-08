using Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Player {
    public class PlayerController : MonoBehaviour {
        public float moveSpeed = 5f;
        public float dodgeRollDuration = 0.1f;
        public float dodgeRollSpeed = 10f;
        public float dodgeRollCooldown = 1f;

        private const float extraRayDist = .05f;

        private Rigidbody rb;
        private Collider col;
        private TrailRenderer tr;
        private Animator animator;
        private Vector3 movement;
        private float movementX;
        private float movementZ;
        private float dodgeRollCooldownTimer;
        private bool playerEnabled = true;
        private bool isDodging;
        private bool dodgePressed;
        private float dodgeRollTimer = 1f;
        private Subscription<DisablePlayerEvent> disableMoveSub;
        private Subscription<EnablePlayerEvent> enableMoveSub;
        private Subscription<TutorialDodgeStartEvent> dodgeStartSub;
        private Subscription<TutorialDodgeEndEvent> dodgeEndSub;

        private Sprite interactSprite;
        private static readonly int Walking = Animator.StringToHash("walking");


        private void Start() {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            tr = GetComponent<TrailRenderer>();
            animator = GetComponent<Animator>();
            disableMoveSub = EventBus.Subscribe<DisablePlayerEvent>(_OnDisableMovement);
            enableMoveSub = EventBus.Subscribe<EnablePlayerEvent>(_OnEnableMovement);
            dodgeStartSub = EventBus.Subscribe<TutorialDodgeStartEvent>(StartDodge);
            dodgeEndSub = EventBus.Subscribe<TutorialDodgeEndEvent>(StopDodge);
        }

        private void OnDestroy() {
            EventBus.Unsubscribe(disableMoveSub);
            EventBus.Unsubscribe(enableMoveSub);
            EventBus.Unsubscribe(dodgeStartSub);
            EventBus.Unsubscribe(dodgeEndSub);
        }

        public void OnDodge(InputAction.CallbackContext value) {
            var sceneName = SceneManager.GetActiveScene().name;
            if (!playerEnabled || sceneName == "HubWorld" || sceneName == "TutorialHubWorld") return;

            switch (dodgeRollCooldownTimer) {
                case > 0:
                    return;
            }

            dodgePressed = value.started switch {
                true => true,
                _ => dodgePressed
            };
        }

        public void OnMove(InputAction.CallbackContext value) {
            switch (playerEnabled) {
                case false:
                    return;
            }

            movementX = value.ReadValue<Vector2>().x;
            movementZ = value.ReadValue<Vector2>().y;

            var rayStart = col.bounds.center - Vector3.up * (col.bounds.extents.y - .1f);

            var horizontalExtent = col.bounds.extents.x + extraRayDist;
            var verticalExtent = col.bounds.extents.z + extraRayDist;

            var ignoreMask = ~LayerMask.GetMask("Special", "Tutorial", "PlayerUtility");

            Debug.DrawRay(rayStart, Vector3.right * horizontalExtent);
            // left/right checks
            if ((Physics.Raycast(rayStart, Vector3.left, out _, horizontalExtent, ignoreMask) && movementX < -.1f) ||
                (Physics.Raycast(rayStart, Vector3.right, out _, horizontalExtent, ignoreMask) && movementX > .1f))
                movementX = 0;
            if ((Physics.Raycast(rayStart, Vector3.forward, out _, verticalExtent, ignoreMask) && movementZ > .1f) ||
                (Physics.Raycast(rayStart, Vector3.back, out _, verticalExtent, ignoreMask) && movementZ < -.1f))
                movementZ = 0;

            animator.SetBool(Walking, true);
        }

        public void OnFire(InputAction.CallbackContext value) {
            switch (playerEnabled) {
                case false:
                    return;
            }

            switch (value.started) {
                case true:
                    EventBus.Publish(new FireEvent(gameObject, true));
                    break;
                default: {
                    switch (value.canceled) {
                        case true:
                            EventBus.Publish(new FireEvent(gameObject, false));
                            break;
                    }

                    break;
                }
            }
        }

        public void OnReload(InputAction.CallbackContext value) {
            switch (playerEnabled) {
                case false:
                    return;
            }

            switch (value.started) {
                case true:
                    EventBus.Publish(new ReloadEvent(gameObject));
                    break;
            }
        }

        public void OnSwapWeapon(InputAction.CallbackContext value) {
            switch (value.started) {
                case true when value.ReadValue<float>() > 0:
                    EventBus.Publish(new SwapEvent(1));
                    break;
                case true: {
                    if (value.ReadValue<float>() < 0) EventBus.Publish(new SwapEvent(-1));
                    break;
                }
            }
        }

        public void OnSwapSpecificWeapon(InputAction.CallbackContext value) {
            switch (value.started) {
                case true:
                    EventBus.Publish(new SwapSpecificEvent(int.Parse(value.control.name)));
                    break;
            }
        }

        public void OnInteract(InputAction.CallbackContext value) {
            switch (value.started) {
                case true:
                    EventBus.Publish(new TryInteractEvent());
                    break;
            }
        }

        private void StartDodge(TutorialDodgeStartEvent tutorDodge = null) {
            EventBus.Publish(new PlayerDodgeEvent(true, movement));
            col.enabled = false; // start iframes, turn back on when dodge ends
            isDodging = true;
            //rb.useGravity = false;
            dodgeRollTimer = dodgeRollDuration;
            dodgeRollCooldownTimer = dodgeRollCooldown;
            if (tutorDodge != null)
                rb.velocity = tutorDodge.direction * dodgeRollSpeed;
            else
                rb.velocity = movement.normalized * dodgeRollSpeed;

            tr.emitting = true;
            animator.SetBool(Walking, false);
        }

        private void StopDodge(TutorialDodgeEndEvent tutorDodge = null) {
            EventBus.Publish(new PlayerDodgeEvent(false, movement));
            col.enabled = true;
            dodgeRollTimer = 0;
            isDodging = false;
            rb.velocity = Vector3.zero;

            tr.emitting = false;
            animator.SetBool(Walking, true);
        }

        private void _OnDisableMovement(DisablePlayerEvent dpme) {
            playerEnabled = false;
        }

        private void _OnEnableMovement(EnablePlayerEvent epme) {
            playerEnabled = true;
        }

        private bool IsWall(Vector3 direction) {
            var wallLayer = LayerMask.GetMask($"Walls");
            Debug.DrawRay(transform.position, direction, Color.magenta);
            return Physics.Raycast(transform.position, direction, out _, col.bounds.extents.magnitude, wallLayer);
        }

        private void Update() {
            switch (playerEnabled) {
                case false:
                    return;
            }


            switch (isDodging) {
                case false: {
                    movement.x = movementX;
                    movement.y = 0f;
                    movement.z = movementZ;
                    if (movement == Vector3.zero) animator.SetBool(Walking, false);
                    break;
                }
            }

            switch (dodgePressed) {
                case true: {
                    dodgePressed = false;

                    if (movementX != 0 || movementZ != 0) StartDodge();
                    break;
                }
            }

            switch (dodgeRollTimer) {
                // enter this condtl during a dodge
                case > 0f: {
                    dodgeRollTimer -= Time.deltaTime;
                    switch (playerEnabled) {
                        case false:
                            StopDodge();
                            break;
                    }

                    break;
                }
            }

            switch (isDodging) {
                case true when dodgeRollTimer <= 0f:
                    StopDodge();
                    break;
            }


            switch (dodgeRollCooldownTimer) {
                case > 0f:
                    dodgeRollCooldownTimer -= Time.deltaTime;
                    break;
            }
        }

        private void FixedUpdate() {
            switch (playerEnabled) {
                case false:
                    return;
            }

            switch (isDodging) {
                //Debug.Log(IsWall(movement));
                case false:
                    rb.MovePosition(rb.position +
                                    moveSpeed * PlayerModifiers.moveSpeed * Time.fixedDeltaTime * movement.normalized);
                    break;
            }
        }

        private void OnCollisionEnter(Collision collision) {
            // manually reset dodge (cancel it) if we hit a wall;
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall")) StopDodge();
        }
    }
}