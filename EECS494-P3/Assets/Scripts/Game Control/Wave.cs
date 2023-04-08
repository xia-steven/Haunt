using System.Collections.Generic;
using ConfigDataTypes;
using UnityEngine;

namespace Game_Control {
    public class Wave {
        public static EnemyWaveData spawnData;
        public static List<int> meleeTable;
        public static List<int> rangedTable;

        private const float timeBetweenSpawns = .6f;
        private const int maxPedestalEnemies = 1;

        private Dictionary<int, IsWaveMember> members = new();

        private static List<GameObject> potentialMembers;
        private List<Transform> spawnPoints;

        private int difficulty;
        private float duration;
        private float startTime;

        private bool spawnPedestals;
        private bool active;

        private int numActiveMembers;
        private int nextId;

        //for now, difficulty will represent the number of melee enemies to spawn
        public Wave(int _difficulty, float _duration, List<Transform> _spawnPoints, bool _spawnPedestals = true) {
            difficulty = _difficulty;
            duration = _duration;
            spawnPoints = _spawnPoints;
            spawnPedestals = _spawnPedestals;

            switch (potentialMembers) {
                //help with overhead
                case null: {
                    potentialMembers = new List<GameObject>();
                    foreach (var att in spawnData.enemySpawnData) potentialMembers.Add(Resources.Load<GameObject>(att.path));
                    break;
                }
            }

            Init();
        }


        private void Init() {
            Vector3 spawnPos;
            IsWaveMember newMember;

            //spawn attack enemies
            for (var i = 0; i < difficulty; ++i) {
                /* GET ENEMY SPAWN POINT */
                spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);

                /* GET ENEMY TYPE */
                //see if is melee
                // Make sure the day isn't negative for the tutorial
                var gameDay = GameControl.Day - 1 >= 0 ? GameControl.Day - 1 : 0;
                var isMelee = Random.value < spawnData.nightlyPropMelee[gameDay];

                //get idx of potentialMembers that new enemy will be
                int spawnIdx = isMelee switch {
                    true => meleeTable[Random.Range(0, meleeTable.Count)],
                    _ => rangedTable[Random.Range(0, rangedTable.Count)]
                };

                /* INSTANTIATE ENEMY */
                //instantiate new enemy and get IsWaveMember component
                newMember = Object.Instantiate(potentialMembers[spawnIdx], spawnPos, Quaternion.identity)
                    .GetComponent<IsWaveMember>();

                /* POST WORK */
                newMember.Init(this, nextId);
                newMember.gameObject.SetActive(false);
                numActiveMembers++;
                members.Add(nextId++, newMember);
            }

            switch (spawnPedestals) {
                //spawn pedestal enemies
                case true: {
                    for (var i = 0; i < maxPedestalEnemies; ++i) {
                        spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position + new Vector3(0, 0.6f, 0);
                        newMember = Object
                            .Instantiate(potentialMembers[^1], spawnPos, Quaternion.identity).GetComponent<IsWaveMember>();
                        newMember.Init(this, nextId);
                        newMember.gameObject.SetActive(false);
                        numActiveMembers++;
                        members.Add(nextId++, newMember);
                    }

                    break;
                }
            }
        }

        public void Spawn() {
            switch (active) {
                case true:
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
}