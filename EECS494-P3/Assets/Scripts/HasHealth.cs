using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 3;
    private int health;

    private void Awake()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    public virtual bool AlterHealth(int healthDelta)
    {
        if (health + healthDelta > maxHealth) return false;
        health += healthDelta;
        Debug.Log(health);
        return true;

    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
