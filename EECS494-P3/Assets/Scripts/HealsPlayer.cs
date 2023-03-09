using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealsPlayer : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<HasHealth>().AlterHealth(healAmount);
            Destroy(gameObject);
        }
    }
}
