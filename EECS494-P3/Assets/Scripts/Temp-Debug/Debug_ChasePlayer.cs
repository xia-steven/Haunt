using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_ChasePlayer : MonoBehaviour
{
    [SerializeField] float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        Vector3 toPlayerVector = (IsPlayer.instance.transform.position - transform.position);
        if (toPlayerVector.magnitude > 1)
        {
            Vector3 posDelta = new Vector3(toPlayerVector.x, toPlayerVector.y, 0).normalized * speed;
            transform.position += posDelta * Time.deltaTime;
        }
    }
}
