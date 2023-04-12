using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasSwordUpgrade : MonoBehaviour {
    private GameObject sword;
    private Subscription<PlayerDodgeEvent> dodgeEvent;
    private bool isSwinging = false;
    private bool switchSwing = false;
    public float swingArc;
    public float swingTime;

    // Start is called before the first frame update
    void Start() {
        sword = Resources.Load<GameObject>("Prefabs/Weapons/SwingSword");
        dodgeEvent = EventBus.Subscribe<PlayerDodgeEvent>(_OnDodge);
    }

    // Attach shield on dodge start and destroy it on dodge finish
    private void _OnDodge(PlayerDodgeEvent e) {
        if (!e.start && !isSwinging) {
            StartCoroutine(SwingSword(e.direction));
        }
    }

    protected void OnDestroy() {
        EventBus.Unsubscribe(dodgeEvent);
    }

    private IEnumerator SwingSword(Vector3 direction) {
        Debug.Log("Sword swing: " + direction);

        isSwinging = true;
        if (direction.x > 0) {
            switchSwing = true;
        }
        else {
            switchSwing = false;
        }

        // Spawn actual swinging sword
        GameObject swingSword = Instantiate(sword, transform);

        // Shift rotation half a swing to the left so swing is started in correct location
        Quaternion shift = Quaternion.AngleAxis((-swingArc / 2f) - 90, Vector3.up);
        direction = shift * direction;

        // Calculate the rotation for the start of the swinging sword
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        Debug.Log("Look rotation: " + rotation);

        // Set the initial rotation and position of the swinging sword
        swingSword.transform.rotation = rotation;
        swingSword.GetComponent<SwingSword>().SetUp(swingArc / swingTime);
        // Check if sword needs to be moved to other side of player
        /*
        if (switchSwing)
        {
            swingSword.transform.position = new Vector3(swingSword.transform.position.x - 0.5f, swingSword.transform.position.y, swingSword.transform.position.z);
        }
        */

        yield return new WaitForSeconds(swingTime);

        Destroy(swingSword);
        isSwinging = false;
    }
}