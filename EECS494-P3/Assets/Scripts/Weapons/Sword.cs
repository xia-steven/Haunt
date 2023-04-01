using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    private bool isSwinging = false;
    private bool switchSwing = false;

    [SerializeField] private float swingArc = 90f;
    [SerializeField] private float swingTime = 0.5f;
    // Sprite of sword - necessary so it can be flipped
    [SerializeField] protected GameObject swordSprite;
    protected GameObject wielder;
    protected GameObject swingingSword;

    protected override void Awake()
    {
        base.Awake();

        currentClipAmount = 0;
        fullClipAmount = 0;
        type = "sword";
        screenShakeStrength = 0f;

        Subscribe();

        spriteRenderer = swordSprite.GetComponent<SpriteRenderer>();
        wielder = this.transform.parent.gameObject;
        swingingSword = Resources.Load<GameObject>("Prefabs/Weapons/SwingSword");
    }

    // Overwrite FixedUpdate to alter rotation of sword and change firing method
    protected override void FixedUpdate()
    {
        if (!isPlayer) return;

        // Get the screen position of the cursor
        Vector3 screenPos = Input.mousePosition;
        Vector3 direction = Vector3.zero;

        // Create a ray from the camera through the cursor position
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // Find the point where the ray intersects the plane that contains the player
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        if (groundPlane.Raycast(ray, out float distanceToGround) && playerEnabled)
        {
            // Calculate the direction vector from the player to the intersection point
            Vector3 hitPoint = ray.GetPoint(distanceToGround);
            direction = hitPoint - transform.position;

            // Check sword placement based on cursor location
            if (direction.x < 0)
            {
                switchSwing = true;
            } else
            {
                switchSwing = false;
            }

            // Shift rotation half a swing to the left so swing is started in correct location
            Quaternion shift = Quaternion.AngleAxis((-swingArc / 2f) - 90, Vector3.up );
            direction = shift * direction;
        }

        // Swing sword if mouse is clicked, swing delay has passed, and not currently swinging
        if (firing && (Time.time - lastTap + swingTime >= tapDelay) && !isSwinging)
        {
            WeaponFire(direction);
        }
    }

    protected override void WeaponFire(Vector3 direction)
    {
        Debug.Log("Sword swung");
        lastTap = Time.time;
        StartCoroutine(SwingSword(direction));
    }

    private IEnumerator SwingSword(Vector3 direction)
    {
        isSwinging = true;
        swordSprite.SetActive(false);

        // Spawn actual swinging sword
        GameObject swingSword = Instantiate(swingingSword, transform);

        // Calculate the rotation for the start of the swinging sword
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

        // Set the initial rotation and position of the swinging sword
        swingSword.transform.rotation = rotation;
        swingSword.GetComponent<SwingSword>().SetUp(swingArc / swingTime);
        // Check if sword needs to be moved to other side of player
        if (switchSwing)
        {
            swingSword.transform.position = new Vector3(swingSword.transform.position.x - 0.5f, swingSword.transform.position.y, swingSword.transform.position.z);
        }

        yield return new WaitForSeconds(swingTime);

        Destroy(swingSword);
        swordSprite.SetActive(true);
        isSwinging = false;
    }
}
