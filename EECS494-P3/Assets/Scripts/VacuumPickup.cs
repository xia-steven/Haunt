using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumPickup : MonoBehaviour
{
    private float vacuumDistance = 4f;
    private float speed = 1.5f;

    private void FixedUpdate()
    {
        if (!GameControl.GamePaused)
        {
            float distance = Vector3.Distance(transform.position, IsPlayer.instance.transform.position);
            if (distance <= vacuumDistance && distance >= 0.01f)
            {
                speed = (vacuumDistance / distance) * speed;
                float step = speed * Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, IsPlayer.instance.transform.position, step);
            } else
            {
                // Reset speed when too far away
                speed = 1.5f;
            }
        }
    }
}
