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
            EventBus.Publish(new DamageEvent(damageAmount));
            collision.gameObject.GetComponent<HasHealth>().AlterHealth(damageAmount);
        }
    }
}
 
public class DamageEvent
{
    public int damage_amt = -1;
    public DamageEvent(int _damage_amt) { damage_amt = _damage_amt; }
}