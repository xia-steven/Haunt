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

    bool active = false;

    int numActiveMembers = 0;
    int nextId = 0;

    //for now, difficulty will represent the number of melee enemies to spawn
    public Wave(int _difficulty, float _duration, List<Transform> _spawnPoints) {
        difficulty = _difficulty;
        duration = _duration;
        spawnPoints = _spawnPoints;

        potentialMembers.Add(Resources.Load("Prefabs/Test Prefabs/Test_Enemy1") as GameObject); //melee
        potentialMembers.Add(Resources.Load("Prefabs/Test Prefabs/Test_Enemy3") as GameObject); //exorcist

        Init();
    }


    private void Init() {
        Vector3 spawnPos;
        IsWaveMember newMember;

        //Queue 
        for (int i = 0; i < 3; ++i) {
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);
            newMember = Object.Instantiate(potentialMembers[1], spawnPos, Quaternion.identity)
                .GetComponent<IsWaveMember>();
            newMember.Init(this, nextId);
            newMember.gameObject.SetActive(false);
            numActiveMembers++;
            members.Add(nextId++, newMember);
        }

        for (int i = 0; i < difficulty; ++i) {
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);
            newMember = Object.Instantiate(potentialMembers[0], spawnPos, Quaternion.identity)
                .GetComponent<IsWaveMember>();
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
    }


    public bool IsOver() {
        return active && numActiveMembers <= 0;
    }

    public void LoseMember(int id) {
        members.Remove(id);
        numActiveMembers--;
    }
}