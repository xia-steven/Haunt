using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPit : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Fell into a pit");
        Vector3 fellPos = other.transform.position;
        fellPos.y += 2;

        other.transform.position = fellPos;
    }
}
