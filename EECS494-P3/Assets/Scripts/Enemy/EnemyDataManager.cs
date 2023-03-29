using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDataManager : MonoBehaviour
{
    static EnemyDataManager instance;

    static EnemyData data;

    string configName = "EnemyData";

    // Enforce singleton in awake
    void Awake()
    {
        //enforce singleton
        if (instance == null) instance = this;
        else Destroy(this.gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Load config data in start to make sure config manager is ready
    private void Start()
    {

        // Load data
        data = ConfigManager.GetData<EnemyData>(configName);
    }

    public static EnemyAttributes GetEnemyAttributes(int enemyID)
    {
        if(enemyID < 0 || enemyID >= data.allEnemies.Count)
        {
            Debug.LogWarning("Enemy Data Manager: could not find enemy with index/ID " + enemyID + ".  Returning null.");
            return null;
        }
        else
        {
            return data.allEnemies[enemyID];
        }
    }
}
