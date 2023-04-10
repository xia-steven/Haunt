using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    public static EnemyWaveData spawnData;
    public static List<int> meleeTable;
    public static List<int> rangedTable;
    static int curEnemies = 0;

    const int maxEnemies = 25;

    const float timeBetweenSpawns = .6f;
    const int maxPedestalEnemies = 1;

    private Dictionary<int, IsWaveMember> members = new Dictionary<int, IsWaveMember>();

    private static List<GameObject> potentialMembers;
    private List<Transform> spawnPoints;

    public readonly int difficulty;
    public readonly float duration;
    private float startTime;

    bool spawnPedestals;
    bool active = false;

    bool spawningTutorial = false;

    int numActiveMembers = 0;
    int nextId = 0;

    //for now, difficulty will represent the number of melee enemies to spawn
    public Wave(int _difficulty, float _duration, List<Transform> _spawnPoints, bool _spawnPedestals = true)
    {
        difficulty = _difficulty;
        duration = _duration;
        spawnPoints = _spawnPoints;
        spawnPedestals = _spawnPedestals;

        //help with overhead
        if (potentialMembers == null)
        {
            potentialMembers = new List<GameObject>();
            foreach (EnemyWaveData.SpawnAttributes att in spawnData.enemySpawnData)
            {
                potentialMembers.Add(Resources.Load<GameObject>(att.path));
            }
        }

        Init();
    }


    private void Init()
    {
        Vector3 spawnPos;
        IsWaveMember newMember;

        if (spawningTutorial) return;

        //spawn attack enemies
        for (int i = 0; i < difficulty;)
        {
            if (curEnemies >= maxEnemies)
            {
                Debug.Log("max enemies reached");
                break;
            }
            /* GET ENEMY SPAWN POINT */
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);

            /* GET ENEMY TYPE */
            //see if is melee
            // Make sure the day isn't negative for the tutorial
            int gameDay = (GameControl.Day - 1) >= 0 ? GameControl.Day - 1 : 0;

            bool isMelee;

            int spawnWeight = difficulty + 1;
            int iters = 0;
            
            int spawnIdx = 0;

            while (spawnWeight > difficulty - i && iters < 3)
            {
                isMelee = Random.value < spawnData.nightlyPropMelee[gameDay];

                //get idx of potentialMembers that new enemy will be
                if (isMelee)
                {
                    spawnIdx = meleeTable[Random.Range(0, meleeTable.Count)];
                }
                else
                {
                    spawnIdx = rangedTable[Random.Range(0, rangedTable.Count)];
                }

                spawnWeight = spawnData.enemySpawnData[spawnIdx].cost;
                ++iters;
            }

            //default to torch peasant
            if (iters >= 3) spawnIdx = 0;

            /* INSTANTIATE ENEMY */
            //instantiate new enemy and get IsWaveMember component
            newMember = Object.Instantiate(potentialMembers[spawnIdx], spawnPos, Quaternion.identity).GetComponent<IsWaveMember>();

            /* POST WORK */
            newMember.Init(this, nextId);
            newMember.gameObject.SetActive(false);
            numActiveMembers++;
            members.Add(nextId++, newMember);

            i += spawnWeight;
            ++curEnemies;
        }

        //spawn pedestal enemies
        if (spawnPedestals)
        {
            for (int i = 0; i < maxPedestalEnemies; ++i)
            {
                spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);
                newMember = Object.Instantiate(potentialMembers[potentialMembers.Count-1], spawnPos, Quaternion.identity).GetComponent<IsWaveMember>();
                newMember.Init(this, nextId);
                newMember.gameObject.SetActive(false);
                numActiveMembers++;
                members.Add(nextId++, newMember);
            }
        }

    }

    public void Spawn()
    {
        if (active)
        {
            Debug.LogError("Error: Attempted to spawn a wave that is already active");
            return;
        }

        IsWaveMediator.instance.Spawn(members.Values, timeBetweenSpawns);


        active = true;
        startTime = Time.time;
    }

    public IEnumerator SpawnTutorial()
    {
        if(spawningTutorial)
        {
            // Return early
            yield break;
        }
        spawningTutorial = true;

        GameObject torch = potentialMembers[0];
        GameObject pitchfork = potentialMembers[1];
        GameObject archer = potentialMembers[2];

        // Spawn 2 torch enemies and 2 pitchfork
        Vector3 spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);
        GameObject.Instantiate(torch, spawnPos, Quaternion.identity);
        spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);
        GameObject.Instantiate(torch, spawnPos, Quaternion.identity);

        spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);
        GameObject.Instantiate(pitchfork, spawnPos, Quaternion.identity);
        spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);
        GameObject.Instantiate(pitchfork, spawnPos, Quaternion.identity);

        // Wait 10 seconds
        yield return new WaitForSeconds(10f);

        // Spawn 3 archers
        for(int a = 0; a < 3; ++a)
        {
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);
            GameObject.Instantiate(archer, spawnPos, Quaternion.identity);
        }

        // Wait 10 more seconds
        yield return new WaitForSeconds(10f);


        int spawnCount = 0;

        // Spawn more enemies while the night is ending
        while(GameControl.NightEnding)
        {
            for (int a = 0; a < 3 && spawnCount < 25; ++a)
            {
                // Archer, torch, or pitchfork
                spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);
                GameObject.Instantiate(potentialMembers[Random.Range(0,3)], spawnPos, Quaternion.identity);
                spawnCount++;
            }


            // Cleric
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);
            GameObject.Instantiate(potentialMembers[potentialMembers.Count - 1], spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(10f);
        }

    }

    public bool IsOver()
    {
        return active && (numActiveMembers <= 0 || Time.time - startTime >= duration);
    }

    public void LoseMember(int id)
    {
        --curEnemies;
        members.Remove(id);
        numActiveMembers--;
    }

    ~Wave()
    {
        for (int i = 0; i < members.Count; ++i)
        {
            --curEnemies;
        }
    }
}