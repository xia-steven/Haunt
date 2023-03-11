using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_MoveToCenter : MonoBehaviour
{
    [SerializeField] float speedMin = 0.2f;
    [SerializeField] float speedMax = 0.6f;

    float speed;

    private void Start()
    {
        speed = Random.Range(speedMin, speedMax);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = (Vector3.zero - transform.position).normalized * speed;
        transform.position += movement * Time.deltaTime;
    }
}
