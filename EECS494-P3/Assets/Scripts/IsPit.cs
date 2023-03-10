using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPit : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Transform normal: ");
        
        Vector3 fellPos = collision.transform.position;
        fellPos.y += 2;


        collision.transform.position = fellPos;
    }
}
