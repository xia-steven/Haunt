using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasHealth : MonoBehaviour
{
    private int health = 6;

    // Update is called once per frame
    public void AlterHealth(int healthDelta)
    {
        health += healthDelta;
        Debug.Log(health);
    }
}
