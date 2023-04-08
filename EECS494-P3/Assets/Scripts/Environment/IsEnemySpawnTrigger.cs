using UnityEngine;

namespace Environment {
    public class IsEnemySpawnTrigger : MonoBehaviour {
        [SerializeField] private Transform spawnLocation;
        [SerializeField] private GameObject enemyToSpawn;

        private bool spawned;

        private void OnTriggerEnter(Collider other) {
            switch (spawned) {
                case false: {
                    Instantiate(enemyToSpawn, spawnLocation.position, Quaternion.Euler(60, 0, 0));
                    spawned = true;
                    break;
                }
            }
        }
    }
}