using System.Collections;
using UnityEngine;

namespace Weapons {
    public class Sword : Weapon {
        private bool isSwinging;
        private bool switchSwing;

        [SerializeField] private float swingArc = 90f;

        [SerializeField] private float swingTime = 0.5f;

        // Sprite of sword - necessary so it can be flipped
        [SerializeField] protected GameObject swordSprite;
        protected GameObject wielder;
        private GameObject swingingSword;

        protected override void Awake() {
            speedMultiplier = 1.1f;

            base.Awake();

            currentClipAmount = 0;
            fullClipAmount = 0;
            type = "sword";
            screenShakeStrength = 0f;

            Subscribe();

            spriteRenderer = swordSprite.GetComponent<SpriteRenderer>();
            wielder = transform.parent.gameObject;
            swingingSword = Resources.Load<GameObject>("Prefabs/Weapons/SwingSword");
        }

        // Overwrite FixedUpdate to alter rotation of sword and change firing method
        protected override void FixedUpdate() {
            switch (shotByPlayer) {
                case false:
                    return;
            }

            // Get the screen position of the cursor
            var screenPos = Input.mousePosition;
            var direction = Vector3.zero;

            // Create a ray from the camera through the cursor position
            if (Camera.main != null) {
                var ray = Camera.main.ScreenPointToRay(screenPos);

                // Find the point where the ray intersects the plane that contains the player
                var groundPlane = new Plane(Vector3.up, transform.position);
                if (groundPlane.Raycast(ray, out var distanceToGround) && playerEnabled) {
                    // Calculate the direction vector from the player to the intersection point
                    var hitPoint = ray.GetPoint(distanceToGround);
                    direction = hitPoint - transform.position;

                    switchSwing = direction.x switch {
                        // Check sword placement based on cursor location
                        < 0 => true,
                        _ => false
                    };

                    // Shift rotation half a swing to the left so swing is started in correct location
                    var shift = Quaternion.AngleAxis((-swingArc / 2f) - 90, Vector3.up);
                    direction = shift * direction;
                }
            }

            switch (firing) {
                // Swing sword if mouse is clicked, swing delay has passed, and not currently swinging
                case true when (Time.time - lastTap + swingTime >= tapDelay) && !isSwinging:
                    WeaponFire(direction);
                    break;
            }
        }

        protected override void WeaponFire(Vector3 direction) {
            Debug.Log("Sword swung");
            lastTap = Time.time;
            StartCoroutine(SwingSword(direction));
        }

        private IEnumerator SwingSword(Vector3 direction) {
            isSwinging = true;
            swordSprite.SetActive(false);

            // Spawn actual swinging sword
            var swingSword = Instantiate(swingingSword, transform);

            // Calculate the rotation for the start of the swinging sword
            var rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Set the initial rotation and position of the swinging sword
            swingSword.transform.rotation = rotation;
            swingSword.GetComponent<SwingSword>().SetUp(swingArc / swingTime);
            swingSword.transform.position = switchSwing switch {
                // Check if sword needs to be moved to other side of player
                true => new Vector3(swingSword.transform.position.x - 0.5f, swingSword.transform.position.y,
                    swingSword.transform.position.z),
                _ => swingSword.transform.position
            };

            yield return new WaitForSeconds(swingTime);

            Destroy(swingSword);
            swordSprite.SetActive(true);
            isSwinging = false;
        }
    }
}