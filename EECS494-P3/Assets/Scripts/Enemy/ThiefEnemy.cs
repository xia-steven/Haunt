using System.Collections;
using Enemy.Weapons;
using Player;
using UnityEngine;

namespace Enemy {
    public class ThiefEnemy : EnemyBase {
        private ThiefKnife knife;
        private GameObject knifeObject;
        private GameObject thiefSmoke;

        private Vector3 spawnPos;

        protected override void Start() {
            base.Start();

            // Get spawn location
            spawnPos = transform.position;

            knife = GetComponentInChildren<ThiefKnife>();
            Debug.Log(knife);
            knifeObject = knife.gameObject;
            knifeObject.SetActive(false);

            thiefSmoke = Resources.Load<GameObject>("Prefabs/Enemy/ThiefSmoke");
        }

        // Override enemy ID to load from config
        protected override int GetEnemyID() {
            return 7;
        }

        // Override attack function
        protected override IEnumerator EnemyAttack() {
            // TODO: Remove or change debug statement
            Debug.Log("Thief Enemy starting attack");
            var direction = (IsPlayer.instance.transform.position - transform.position).normalized;

            // Calculate the rotation that points in the direction of the intersection point
            var rotation = Quaternion.LookRotation(direction, Vector3.up);


            // Set the rotation of the knife
            knifeObject.transform.rotation = rotation;
            knifeObject.transform.Rotate(-90, 0, 0);

            knifeObject.SetActive(true);

            // While attacking
            while (state == EnemyState.Attacking) {
                // slight backwards windup
                rb.velocity = -direction * attributes.windupSpeed;
                yield return new WaitForSeconds(attributes.windupTime);

                // dash towards the player
                rb.velocity = direction * attributes.dashSpeed;
                yield return new WaitForSeconds(attributes.dashTime);

                // teleport away
                rb.velocity = Vector3.zero;
                Instantiate(thiefSmoke, transform.position, Quaternion.identity);
                // Smoke will destroy itself
                transform.position = GetTeleportLocation();


                yield return new WaitForSeconds(attributes.attackSpeed);
            }

            knifeObject.SetActive(false);

            // Let update know that we're done
            runningCoroutine = false;
        }


        private Vector3 GetTeleportLocation() {
            var valid = false;
            var count = 0;
            var testPos = spawnPos;

            while (valid == false && count < attributes.maxThiefSpawnAttempt) {
                var xCoord = Random.Range(attributes.minXThiefSpawn, attributes.maxXThiefSpawn + 1);
                var zCoord = Random.Range(attributes.minZThiefSpawn, attributes.maxZThiefSpawn + 1);
                testPos = new Vector3(xCoord, 0.5f, zCoord);
                if (Camera.main != null) {
                    var camCoords = Camera.main.WorldToScreenPoint(testPos);
                    // Check if outside of camera bounds
                    if (camCoords.x > Screen.width || camCoords.x < 0 || camCoords.y > Screen.height || camCoords.y < 0)
                        // Make sure the enemy can't spawn in the center island
                        if (xCoord > 4 || xCoord < -4 || zCoord < -3 || zCoord > 3)
                            if (Pathfinding.AStar.Instance.GetGrid().GetGridObject(testPos).isWalkable)
                                valid = true;
                    // If valid is still true, we can spawn here
                }

                ++count;
            }

            switch (valid) {
                case true:
                    return testPos;
                default:
                    Debug.Log("Failed to find a suitable spawn point after " + count +
                              " attempts.  Defaulting to enemy spawn");
                    return spawnPos;
            }
        }
    }
}