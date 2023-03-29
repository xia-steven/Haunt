using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BossAttributes : Savable
{
    public string name;
    public float moveSpeed;
    public float targetDistance;
    public int health;

    public override string ToString()
    {
        string output = "Boss attributes: Name is " + name + ", moveSpeed is " + moveSpeed +
            ", targetDistance is " + targetDistance + ", and health is " + health;
        return output;
    }
}
