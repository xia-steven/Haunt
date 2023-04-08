using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    public static EnemyWaveData spawnData;
    public static List<int> meleeTable;
    public static List<int> rangedTable;

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

        //spawn attack enemies
        for (int i = 0; i < difficulty; ++i)
        {
            /* GET ENEMY SPAWN POINT */
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);

            /* GET ENEMY TYPE */
            //see if is melee
            // Make sure the day isn't negative for the tutorial
            int gameDay = (GameControl.Day - 1) >= 0 ? GameControl.Day - 1 : 0;
            bool isMelee = Random.value < spawnData.nightlyPropMelee[gameDay];

            //get idx of potentialMembers that new enemy will be
            int spawnIdx;
            if (isMelee)
            {
                spawnIdx = meleeTable[Random.Range(0, meleeTable.Count)];
            }
            else
            {
                spawnIdx = rangedTable[Random.Range(0, rangedTable.Count)];
            }

            /* INSTANTIATE ENEMY */
            //instantiate new enemy and get IsWaveMember component
            newMember = Object.Instantiate(potentialMembers[spawnIdx], spawnPos, Quaternion.identity).GetComponent<IsWaveMember>();

            /* POST WORK */
            newMember.Init(this, nextId);
            newMember.gameObject.SetActive(false);
            numActiveMembers++;
            members.Add(nextId++, newMember);
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


    public bool IsOver()
    {
        return active && (numActiveMembers <= 0 || Time.time - startTime >= duration);
    }

    public void LoseMember(int id)
    {
        members.Remove(id);
        numActiveMembers--;
    }
}