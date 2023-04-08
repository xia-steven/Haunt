using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BossAttributes : Savable {
    public string name;
    public float moveSpeed;
    public float attackSpeed;
    public float projectileSpeed;
    public float projectileLifetime;
    public float groundPoundWindup;
    public float groundPoundTime;
    public float laserWindup;
    public float health;

    public override string ToString() {
        string output = "Boss attributes: Name is " + name + ", moveSpeed is " + moveSpeed +
                        ", attackSpeed is " + attackSpeed + ", projectileSpeed is " + projectileSpeed +
                        ", projectileLifetime is " + projectileLifetime +
                        ", groundPoundWindup is " + groundPoundWindup +
                        ", groundPoundTime is " + groundPoundTime +
                        ", laserWindup is " + laserWindup +
                        ", and health is " + health;
        return output;
    }
}