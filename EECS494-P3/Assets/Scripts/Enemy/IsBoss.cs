using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HasEnemyHealth))]
public class IsBoss : MonoBehaviour {
    string configName = "BossData";
    BossAttributes bossData;

    HasEnemyHealth health;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start() {
        // Load config
        bossData = ConfigManager.GetData<BossAttributes>(configName);

        // Get components
        health = GetComponent<HasEnemyHealth>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() { }
}