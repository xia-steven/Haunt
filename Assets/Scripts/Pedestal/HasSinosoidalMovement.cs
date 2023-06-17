using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasSinosoidalMovement : MonoBehaviour
{
    [Header("0 = x, 1 = y, 2 = z")]
    [Range(0,2)]
    [SerializeField] int direction;
    [SerializeField] float magnitude = 5f;
    [SerializeField] bool reverseDirection = false;

    float initialTime;

    private void Start()
    {
        initialTime = Time.time;
    }


    // Update is called once per frame
    void Update()
    {
        float movementAmount = reverseDirection? (-Mathf.Cos(Time.time - initialTime) * magnitude * Time.deltaTime) : (Mathf.Cos(Time.time - initialTime) * magnitude * Time.deltaTime);
        Vector3 amountToMove = transform.position;
        if(direction == 0)
        {
            // x
            amountToMove.x += movementAmount;
        }
        else if (direction == 1)
        {
            // y
            amountToMove.y += movementAmount;
        }
        else if (direction == 2)
        {
            // z
            amountToMove.z += movementAmount;
        }
        transform.position = amountToMove;
    }
}
