using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave {
    const float timeBetweenSpawns = .4f;

    private Dictionary<int, IsWaveMember> members = new Dictionary<int, IsWaveMember>();

    private List<GameObject> potentialMembers = new List<GameObject>(); //May change later for dynamic difficulty
    private List<Transform> spawnPoints;

    public readonly int difficulty;
    public readonly float duration;
    private float startTime;

    bool spawnPedestals;
    bool active = false;

    int numActiveMembers = 0;
    int nextId = 0;

    //for now, difficulty will represent the number of melee enemies to spawn
    public Wave(int _difficulty, float _duration, List<Transform> _spawnPoints, bool _spawnPedestals = true) {
        difficulty = _difficulty;
        duration = _duration;
        spawnPoints = _spawnPoints;
        spawnPedestals = _spawnPedestals;

        potentialMembers.Add(Resources.Load("Prefabs/Enemy/PeasantTorch") as GameObject); //torch
        potentialMembers.Add(Resources.Load("Prefabs/Enemy/PeasantPitchfork") as GameObject); //pitchfork
        potentialMembers.Add(Resources.Load("Prefabs/Enemy/Cleric") as GameObject); //cleric

        Init();
    }


    private void Init() {
        Vector3 spawnPos;
        IsWaveMember newMember;

        //wave seed to decide composition. closer to 0 -> melee, closer to 1 -> ranged
        float enemyBalance = Random.value;
        

        for (int i = 0; i < difficulty; ++i) {
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);

            int randSpawnIdx = Random.value < enemyBalance ? 0 : 1;
            newMember = Object.Instantiate(potentialMembers[randSpawnIdx], spawnPos, Quaternion.identity).GetComponent<IsWaveMember>();

            newMember.Init(this, nextId);
            newMember.gameObject.SetActive(false);
            numActiveMembers++;
            members.Add(nextId++, newMember);
        }

        //spawn pedestal enemies
        for (int i = 0; i < 1; ++i) {
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);
            newMember = Object.Instantiate(potentialMembers[2], spawnPos, Quaternion.identity).GetComponent<IsWaveMember>();
            newMember.Init(this, nextId);
            newMember.gameObject.SetActive(false);
            numActiveMembers++;
            members.Add(nextId++, newMember);
        }
    }

    public void Spawn() {
        if (active) {
            Debug.LogError("Error: Attempted to spawn a wave that is already active");
            return;
        }

        IsWaveMediator.instance.Spawn(members.Values, timeBetweenSpawns);

        active = true;
        startTime = Time.time;
    }


    public bool IsOver() {
        return active && (numActiveMembers <= 0 || Time.time - startTime >= duration);
    }

    public void LoseMember(int id) {
        members.Remove(id);
        numActiveMembers--;
    }
}