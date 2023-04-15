using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumPickup : MonoBehaviour
{
    private float vacuumDistance = 3f;
    private float speed = 1f;

    private void FixedUpdate()
    {
        if (!GameControl.GamePaused)
        {
            float distance = Vector3.Distance(transform.position, IsPlayer.instance.transform.position);
            if (distance <= vacuumDistance && distance >= 0.01f)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, IsPlayer.instance.transform.position, step);
            }
        }
    }
}
