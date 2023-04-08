using ConfigDataTypes;
using JSON_Parsing;
using UnityEngine;

namespace Enemy {
    public class EnemyDataManager : MonoBehaviour {
        private static EnemyDataManager instance;

        private static EnemyData data;

        private const string configName = "EnemyData";

        // Enforce singleton in awake
        private void Awake() {
            //enforce singleton
            if (instance == null) instance = this;
            else Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            // Load data
            data = ConfigManager.GetData<EnemyData>(configName);
            Debug.Log(data);
        }

        public static EnemyAttributes GetEnemyAttributes(int enemyID) {
            if (enemyID < 0 || enemyID >= data.allEnemies.Count) {
                Debug.LogWarning("Enemy Data Manager: could not find enemy with index/ID " + enemyID +
                                 ".  Returning null.");
                return null;
            }

            return data.allEnemies[enemyID];
        }
    }
}