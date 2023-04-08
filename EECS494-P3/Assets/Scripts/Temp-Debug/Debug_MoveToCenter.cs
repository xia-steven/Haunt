using UnityEngine;

public class Debug_MoveToCenter : MonoBehaviour {
    [SerializeField] private float speedMin = 0.2f;
    [SerializeField] private float speedMax = 0.6f;

    private float speed;

    private void Start() {
        speed = Random.Range(speedMin, speedMax);
    }

    // Update is called once per frame
    private void Update() {
        var movement = (Vector3.zero - transform.position).normalized * speed;
        transform.position += movement * Time.deltaTime;
    }
}