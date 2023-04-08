using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IsWaveMediator : MonoBehaviour {
    public static IsWaveMediator instance;


    // Start is called before the first frame update
    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (Wave.spawnData == null) Wave.spawnData = ConfigManager.GetData<EnemyWaveData>("EnemyWaveData");

        //if tutorial, use night 1 table
        if (SceneManager.GetActiveScene().name == "TutorialGameScene")
        {
            PopulateTables(1);
        }
        //else, make table for current night
        else if (SceneManager.GetActiveScene().name == "GameScene")
        {
            PopulateTables(Mathf.Clamp(GameControl.Day, 1, 3));
        }
    }

    private void PopulateTables(int night)
    {
        //reset tables
        Wave.meleeTable = new List<int>();
        Wave.rangedTable = new List<int>();

        //populate tables
        foreach (int idx in Wave.spawnData.nightlyEnemies[night-1].indices)
        {
            if (Wave.spawnData.enemySpawnData[idx].isMelee)
            {
                for(int i = 0; i < Wave.spawnData.enemySpawnData[idx].weight; ++i)
                {
                    Wave.meleeTable.Add(idx);
                }
            }
            else
            {
                for(int i = 0; i < Wave.spawnData.enemySpawnData[idx].weight; ++i)
                {
                    Wave.rangedTable.Add(idx);
                }
            }
        }
    }

    public void Spawn(Dictionary<int, IsWaveMember>.ValueCollection members, float timeBetweenSpawns) {
        StartCoroutine(TimedSpawn(members, timeBetweenSpawns));
    }

    private IEnumerator TimedSpawn(Dictionary<int, IsWaveMember>.ValueCollection members, float timeBetweenSpawns) {
        foreach (var m in new List<IsWaveMember>(members)) {
            if (m == null) continue;
            m.gameObject.SetActive(true);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    private void OnDestroy() {
        instance = null;
    }
}