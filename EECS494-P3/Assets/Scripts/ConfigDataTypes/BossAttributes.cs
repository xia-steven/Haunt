using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BossAttributes : Savable
{
    public string name;
    public float moveSpeed;
    public float moveToCenterSpeed;
    public float attackSpeed;
    public float projectileSpeed;
    public float projectileLifetime;
    public float shockwaveWindup;
    public float shockwavePound;
    public float shockwaveTime;
    public float laserWindup;
    public float laserTime;
    public float laserRotateSpeed;
    public float health;

    public override string ToString()
    {
        string output = "Boss attributes: Name is " + name + ", moveSpeed is " + moveSpeed +
            ", attackSpeed is " + attackSpeed + ", projectileSpeed is " + projectileSpeed +
            ", projectileLifetime is " + projectileLifetime +
            ", groundPoundWindup is " + shockwaveWindup +
            ", groundPoundTime is " + shockwaveTime +
            ", laserWindup is " + laserWindup +
            ", and health is " + health;
        return output;
    }
}
