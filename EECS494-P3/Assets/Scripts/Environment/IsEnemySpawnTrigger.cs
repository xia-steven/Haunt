using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsEnemySpawnTrigger : MonoBehaviour {
    [SerializeField] Transform spawnLocation;
    [SerializeField] GameObject enemyToSpawn;

    bool spawned;

    private void OnTriggerEnter(Collider other) {
        if (!spawned) {
            GameObject enemy = Instantiate(enemyToSpawn, spawnLocation.position, Quaternion.Euler(60, 0, 0));
            spawned = true;
        }
    }
}