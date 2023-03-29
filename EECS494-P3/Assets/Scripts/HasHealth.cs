using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasHealth : MonoBehaviour {
    [SerializeField] protected int maxHealth = 3;
    protected int health;

    private void Awake() {
        health = maxHealth;
    }

    // Update is called once per frame
    public virtual void AlterHealth(int healthDelta) {
        health += healthDelta;
    }

    public int GetHealth() {
        return health;
    }

    public int GetMaxHealth() {
        return maxHealth;
    }
}