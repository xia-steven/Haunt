using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasHealth : MonoBehaviour {
    [SerializeField] protected float maxHealth = 3;
    protected float health;

    private void Awake() {
        Debug.Log("Setting Health to " + maxHealth);
        health = maxHealth;
    }

    // Update is called once per frame
    public virtual void AlterHealth(float healthDelta) {
        health += healthDelta;
    }

    public float GetHealth() {
        return health;
    }

    public float GetMaxHealth() {
        return maxHealth;
    }
}