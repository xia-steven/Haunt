using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// TODO: Rename file and class to enemy name
public class ThiefEnemy : EnemyBase {
    ThiefKnife knife;
    GameObject knifeObject;
    float windupSpeed = 1.0f;
    float dashSpeed = 10.0f;
    float windupTime = 0.3f;
    float dashTime = 0.25f;
    GameObject thiefSmoke;
    int minXSpawn = -17;
    int maxXSpawn = 17;
    int minZSpawn = -11;
    int maxZSpawn = 10;
    int maxSpawnAttempts = 10;

    Vector3 spawnPos;

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
    public override int GetEnemyID() {
        // TODO: Change returned value to enemyID (index in config file)
        return 7;
    }

    // Override attack function
    public override IEnumerator EnemyAttack() {
        // TODO: Remove or change debug statement
        Debug.Log("Thief Enemy starting attack");
        Vector3 direction = (IsPlayer.instance.transform.position - transform.position).normalized;

        // Calculate the rotation that points in the direction of the intersection point
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);


        // Set the rotation of the knife
        knifeObject.transform.rotation = rotation;
        knifeObject.transform.Rotate(-90, 0, 0);

        knifeObject.SetActive(true);

        // While attacking
        while (state == EnemyState.Attacking) {
            // slight backwards windup
            rb.velocity = -direction * windupSpeed;
            yield return new WaitForSeconds(windupTime);

            // dash towards the player
            rb.velocity = direction * dashSpeed;
            yield return new WaitForSeconds(dashTime);

            // teleport away
            rb.velocity = Vector3.zero;
            GameObject smoke = Instantiate(thiefSmoke, transform.position, Quaternion.identity);
            // Smoke will destroy itself
            transform.position = GetTeleportLocation();


            yield return new WaitForSeconds(attributes.attackSpeed);
        }

        knifeObject.SetActive(false);

        // Let update know that we're done
        runningCoroutine = false;
    }


    Vector3 GetTeleportLocation() {
        bool valid = false;
        int count = 0;

        int xCoord = 0;
        int zCoord = 0;
        Vector3 testPos = spawnPos;

        while (valid == false && count < maxSpawnAttempts) {
            xCoord = Random.Range(minXSpawn, maxXSpawn + 1);
            zCoord = Random.Range(minZSpawn, maxZSpawn + 1);
            testPos = new Vector3(xCoord, 0.5f, zCoord);
            Vector3 camCoords = Camera.main.WorldToScreenPoint(testPos);
            // Check if outside of camera bounds
            if (camCoords.x > Screen.width || camCoords.x < 0 || camCoords.y > Screen.height || camCoords.y < 0) {
                // Make sure the enemy can't spawn in the center island
                if (xCoord > 4 || xCoord < -4 || zCoord < -3 || zCoord > 3) {
                    if (Pathfinding.Instance.GetGrid().GetGridObject(testPos).isWalkable) {
                        valid = true;
                    }
                    // If valid is still true, we can spawn here
                }
            }

            ++count;
        }

        if (valid == false) {
            Debug.Log("Failed to find a suitable spawn point after " + count + " attempts.  Defaulting to enemy spawn");
            return spawnPos;
        }


        return testPos;
    }
}