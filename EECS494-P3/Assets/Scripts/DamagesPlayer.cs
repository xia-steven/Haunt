using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamagesPlayer : MonoBehaviour
{
    [SerializeField] private int damageAmount = -1;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<HasHealth>().AlterHealth(damageAmount);
        }
    }
}
 