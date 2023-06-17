using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasSwordUpgrade : MonoBehaviour {
    private GameObject sword;
    private Subscription<SwingSwordEvent> swordEvent;
    private bool isSwinging = false;
    public float swingArc;
    public float swingTime;
    private float swingDelay = 1f;
    private float lastSwing;

    // Start is called before the first frame update
    void Start() {
        sword = Resources.Load<GameObject>("Prefabs/Weapons/SwingSword");
        swordEvent = EventBus.Subscribe<SwingSwordEvent>(_OnSword);
        lastSwing = Time.time;
    }

    // Swing sword when event is called
    private void _OnSword(SwingSwordEvent e) {
        if (!isSwinging && (Time.time - lastSwing) >= swingDelay) {
            StartCoroutine(SwingSword());
        }
    }

    protected void OnDestroy() {
        EventBus.Unsubscribe(swordEvent);
        StopAllCoroutines();
        isSwinging = false;
    }

    private IEnumerator SwingSword() {
        isSwinging = true;
        lastSwing = Time.time;

        // Change sword visual
        EventBus.Publish(new SwordVisualEvent(true, swingDelay));

        // Get the screen position of the cursor
        Vector3 screenPos = Input.mousePosition;
        Vector3 direction = Vector3.zero;

        // Create a ray from the camera through the cursor position
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // Find the point where the ray intersects the plane that contains the player
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        if (groundPlane.Raycast(ray, out float distanceToGround))
        {
            // Calculate the direction vector from the player to the intersection point
            Vector3 hitPoint = ray.GetPoint(distanceToGround);
            direction = hitPoint - transform.position;
        }

        // Spawn actual swinging sword
        GameObject swingSword = Instantiate(sword, transform);

        // Shift rotation half a swing to the left so swing is started in correct location
        Quaternion shift = Quaternion.AngleAxis((-swingArc / 2f) - 90, Vector3.up);
        direction = shift * direction;

        // Calculate the rotation for the start of the swinging sword
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

        // Set the initial rotation and position of the swinging sword
        swingSword.transform.rotation = rotation;
        swingSword.GetComponent<SwingSword>().SetUp(swingArc / swingTime);

        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Audio/Weapons/sword"), transform.position);

        yield return new WaitForSeconds(swingTime);

        // Change sword visual
        EventBus.Publish(new SwordVisualEvent(false, swingDelay));

        Destroy(swingSword);
        isSwinging = false;
    }
}

public class SwordVisualEvent 
{
    public bool started;
    public float delay;
    public SwordVisualEvent(bool _started, float _delay)
    {
        started = _started;
        delay = _delay;
    }
}