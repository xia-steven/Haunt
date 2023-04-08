using Player;
using UnityEngine;

public class Debug_ChasePlayer : MonoBehaviour {
    [SerializeField] private float speed = 1f;

    // Update is called once per frame
    private void Update() {
        var toPlayerVector = (IsPlayer.instance.transform.position - transform.position);
        switch (toPlayerVector.magnitude) {
            case > 1: {
                var posDelta = new Vector3(toPlayerVector.x, toPlayerVector.y, 0).normalized * speed;
                transform.position += posDelta * Time.deltaTime;
                break;
            }
        }
    }
}