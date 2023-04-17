using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BossHasHealth))]
public class IsBoss : MonoBehaviour
{
    public static List<GameObject> spawnedClerics;


    string configName = "BossData";
    BossAttributes bossData;

    BossHasHealth health;
    Rigidbody rb;

    Vector3 direction;

    [SerializeField] List<Transform> spawners;

    // Cleric spawning variables
    [SerializeField] List<Transform> clericSpawners;
    GameObject clericPrefab;

    GameObject basicBulletPrefab;
    GameObject arbalestBulletPrefab;

    // Special attack variables
    [SerializeField] GameObject sprite;
    private LineRenderer lineRenderer;
    private SphereCollider lineCollider;
    Vector3 startRotation;

    bool attacking = false;
    bool canMoveInFixedUpdate = true;

    [SerializeField] Color warmupColor;

    List<GameObject> possibleEnemies;
    

    // Start is called before the first frame update
    void Awake() {
        // Load config
        bossData = ConfigManager.GetData<BossAttributes>(configName);

        // Get components
        health = GetComponent<BossHasHealth>();
        health.setMaxHealth(bossData.health);
        rb = GetComponent<Rigidbody>();

        lineRenderer = GetComponentInChildren<LineRenderer>();
        lineCollider = GetComponentInChildren<SphereCollider>();
        lineRenderer.enabled = false;
        lineCollider.enabled = false;
        startRotation = transform.forward;

        clericPrefab = Resources.Load<GameObject>("Prefabs/Enemy/Cleric");

        if(spawnedClerics == null)
        {
            spawnedClerics = new List<GameObject>();
        }

        basicBulletPrefab = Resources.Load<GameObject>("Prefabs/Weapons/EnemyBasicBullet");
        arbalestBulletPrefab = Resources.Load<GameObject>("Prefabs/EnemyWeapons/ArbalestShot");

        ArbalestProjectile arbalestProj = arbalestBulletPrefab.GetComponent<ArbalestProjectile>();
        arbalestProj.rotationSpeed = 30f;

        // Load enemies into list to spawn
        possibleEnemies = new List<GameObject>();

        possibleEnemies.Add(Resources.Load<GameObject>("Prefabs/Enemy/Demolition"));
        possibleEnemies.Add(Resources.Load<GameObject>("Prefabs/Enemy/Thief"));
        possibleEnemies.Add(Resources.Load<GameObject>("Prefabs/Enemy/Arbalest"));
        possibleEnemies.Add(Resources.Load<GameObject>("Prefabs/Enemy/EliteArcher"));
    }

    private void Update()
    {
        if(health.GetHealth() == 0) {
            rb.velocity = Vector3.zero;
            return;
        }

        if(!attacking)
        {
            attacking = true;
            int attackIndex = Random.Range(0, 100);

            if(attackIndex < 35)
            {
                StartCoroutine(basicAttack());
            }
            else if (attackIndex < 60)
            {
                StartCoroutine(arbalestAttack());
            }
            else if (attackIndex < 75)
            {
                StartCoroutine(GroundPound(new Vector3(0, 0.5f, 0)));
            }
            else if (attackIndex < 85)
            {
                // Fire more lasers as health decreases
                int laserNum = 1 + (int)((1 - health.GetHealth() / bossData.health) * 4);
                StartCoroutine(FireLaser(laserNum));
            }
            else if (attackIndex < 100)
            {
                // Spawn random enemies
                StartCoroutine(spawnRandomEnemies());
            }
        }

    }

    private void FixedUpdate()
    {
        if (!canMoveInFixedUpdate || health.GetHealth() == 0) return;


        // Boss follows the player at a slow speed
        Vector3 targetPos = IsPlayer.instance.transform.position;

        direction = (targetPos - transform.position).normalized;


        rb.velocity = direction * bossData.moveSpeed;
    }

    IEnumerator basicAttack()
    {
        int bulletCount = Random.Range(5, 11);

        for(int a = 0; a < bulletCount; ++a)
        {
            // Random offset for each bullet
            Quaternion randRotation = Quaternion.AngleAxis(Random.Range(-15f, 15f), Vector3.up);
            Vector3 fireDir = randRotation * direction;
            fireBullet(basicBulletPrefab, fireDir, Shooter.Enemy, bossData.projectileSpeed, bossData.projectileLifetime);
            yield return new WaitForSeconds(bossData.attackSpeed / bulletCount);
        }


        yield return new WaitForSeconds(bossData.attackSpeed);
        attacking = false;
    }


    IEnumerator arbalestAttack()
    {
        int bulletCount = Random.Range(3, 7);

        for (int a = 0; a < bulletCount; ++a)
        {
            // Random offset for each bullet
            Quaternion randRotation = Quaternion.AngleAxis(Random.Range(-15f, 15f), Vector3.up);
            Vector3 fireDir = randRotation * direction;
            fireBullet(arbalestBulletPrefab, fireDir, Shooter.Enemy, bossData.projectileSpeed, bossData.projectileLifetime);
            yield return new WaitForSeconds(bossData.attackSpeed / bulletCount);
        }


        yield return new WaitForSeconds(bossData.attackSpeed);
        attacking = false;
    }

    IEnumerator spawnRandomEnemies()
    {

        for(int a = 0; a < 3; ++a)
        {
            Transform location = spawners[Random.Range(0, spawners.Count)];
            Instantiate(possibleEnemies[Random.Range(0, possibleEnemies.Count)], location.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(bossData.attackSpeed);
        attacking = false;
    }

    // Base function to use for firing projectiles
    protected void fireBullet(GameObject bullet, Vector3 direction, Shooter shooter, float projectileSpeed, float lifetime)
    {
        // Spawn bullet at barrel of gun
        GameObject projectile = Instantiate(bullet, transform.position, Quaternion.identity);

        EnemyBasicBullet enemyBull = projectile.GetComponent<EnemyBasicBullet>();
        // Set shooter to holder of gun (enemy or player)
        enemyBull.SetShooter(shooter);
        enemyBull.setLifetime(lifetime);
        // Allow bullets to travel through initial walls
        enemyBull.fromBoss = true;

        // Give bullet its velocity
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
    }

    public IEnumerator GroundPound(Vector3 targetPos, bool cutscene = false)
    {
        if(!cutscene)
        {
            canMoveInFixedUpdate = false;
        }

        // Make sure fixedUpdate gets disabled
        yield return null;

        //setDirection = true;
        direction = (targetPos - transform.position).normalized;

        float moveTime = Vector3.Distance(targetPos, transform.position) / bossData.moveToCenterSpeed;
        Vector3 initialPosition = transform.position;

        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / moveTime;

        if(cutscene)
        {
            initial_time = Time.realtimeSinceStartup;
            progress = (Time.realtimeSinceStartup - initial_time) / moveTime;
        }

        // Get to the location
        while (progress < 1.0f)
        {
            if (cutscene)
            {
                progress = (Time.realtimeSinceStartup - initial_time) / moveTime;
            }
            else
            {
                progress = (Time.time - initial_time) / moveTime;
            }

            transform.position = Vector3.Lerp(initialPosition, targetPos, progress);

            yield return null;
        }

        if(!cutscene)
        {
            rb.position = targetPos; 
            rb.velocity = Vector3.zero;
        }


        // Raise sprite in the air
        initial_time = Time.time;
        progress = (Time.time - initial_time) / bossData.shockwaveWindup;

        if (cutscene)
        {
            initial_time = Time.realtimeSinceStartup;
            progress = (Time.realtimeSinceStartup - initial_time) / bossData.shockwaveWindup;
        }

        Vector3 initialPos = transform.position;

        while (progress < 1.0f)
        {
            if (cutscene)
            {
                progress = (Time.realtimeSinceStartup - initial_time) / bossData.shockwaveWindup;
            }
            else
            {
                progress = (Time.time - initial_time) / bossData.shockwaveWindup;
            }

            // Move transform up
            transform.position = initialPos + new Vector3(0, 3.0f * progress, 0);


            yield return null;
        }

        // Ground pound down
        initial_time = Time.time;
        progress = (Time.time - initial_time) / bossData.shockwavePound;

        if (cutscene)
        {
            initial_time = Time.realtimeSinceStartup;
            progress = (Time.realtimeSinceStartup - initial_time) / bossData.shockwavePound;
        }

        while (progress < 1.0f)
        {
            if (cutscene)
            {
                progress = (Time.realtimeSinceStartup - initial_time) / bossData.shockwavePound;
            }
            else
            {
                progress = (Time.time - initial_time) / bossData.shockwavePound;
            }

            // Move transform down
            transform.position = initialPos + new Vector3(0, 3.0f * (1 - progress), 0);


            yield return null;
        }

        // Spawn shockwave line renderer
        // Code adapted from https://www.loekvandenouweland.com/content/use-linerenderer-in-unity-to-draw-a-circle.html
        int segmentCount = 100;
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = segmentCount + 1;
        lineRenderer.enabled = true;
        // Reset radius before enabling
        lineCollider.radius = 0.01f;
        lineCollider.enabled = true;

        float finalRadius = 30.0f;

        int pointCount = segmentCount + 1;
        Vector3[] points = new Vector3[pointCount];

        initial_time = Time.time;
        progress = (Time.time - initial_time) / bossData.shockwaveTime;

        if (cutscene)
        {
            initial_time = Time.realtimeSinceStartup;
            progress = (Time.realtimeSinceStartup - initial_time) / bossData.shockwaveTime;
        }

        while (progress < 1.0f)
        {
            if (cutscene)
            {
                progress = (Time.realtimeSinceStartup - initial_time) / bossData.shockwaveTime;
            }
            else
            {
                progress = (Time.time - initial_time) / bossData.shockwaveTime;
            }

            for (int i = 0; i < pointCount; ++i)
            {
                float radians = Mathf.Deg2Rad * (i * 360f / segmentCount);
                points[i] = new Vector3(Mathf.Sin(radians) * finalRadius * (0.01f + progress), 0.5f, Mathf.Cos(radians) * finalRadius * (0.01f + progress));
            }

            lineRenderer.SetPositions(points);
            lineCollider.radius = (finalRadius * progress);

            yield return null;
        }

        lineRenderer.enabled = false;
        lineCollider.enabled = false;
        if(!cutscene)
        {
            yield return new WaitForSeconds(bossData.attackSpeed);
            attacking = false;
            canMoveInFixedUpdate = true;
        }
    }


    IEnumerator FireLaser(int laserCount)
    {
        canMoveInFixedUpdate = false;

        // Make sure fixedUpdate gets disabled
        yield return null;

        Vector3 targetPos = new Vector3(0, 0.5f, 0);

        direction = (targetPos - transform.position).normalized;

        float moveTime = Vector3.Distance(targetPos, transform.position) / bossData.moveToCenterSpeed;
        Vector3 initialPosition = transform.position;

        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / moveTime;

        // Get to the location
        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / moveTime;

            transform.position = Vector3.Lerp(initialPosition, targetPos, progress);

            yield return null;
        }

        rb.position = targetPos;

        rb.velocity = Vector3.zero;


        // Warmup laser
        initial_time = Time.time;
        progress = (Time.time - initial_time) / bossData.laserWindup;

        Color initialColor = lineRenderer.material.color;
        lineRenderer.enabled = true;
        lineRenderer.material.color = warmupColor;

        float initialStartWidth = lineRenderer.startWidth;
        float initialEndWidth = lineRenderer.endWidth;

        Vector3[] laserPoints = new Vector3[laserCount * 2];

        float offsetPerLaser = 360f / (float)laserCount;

        //int wallMask = LayerMask.GetMask("Wall");

        for(int a = 0; a < laserCount * 2; ++a)
        {
            if(a % 2 == 0)
            {
                laserPoints[a] =  new Vector3(transform.position.x, transform.position.y, transform.position.z);
            }
            else
            {
                float magnitude = 50f;
                // If we hit a wall, reduce distance
                //if(Physics.Raycast(transform.position, Quaternion.Euler(0, offsetPerLaser * ((a + 1) / 2), 0) * startRotation, 
                //    out RaycastHit hit, magnitude, wallMask))
                //{
                //    magnitude = Vector3.Distance(hit.point, transform.position);
                //}
                laserPoints[a] = Quaternion.Euler(0, offsetPerLaser * ((a + 1) / 2), 0) * startRotation * magnitude;
            }
        }

        lineRenderer.positionCount = laserCount * 2;
        lineRenderer.SetPositions(laserPoints);


        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / bossData.laserWindup;

            // Fade in line renderer
            // Set the width of the line
            lineRenderer.startWidth = initialStartWidth * progress;
            lineRenderer.endWidth = initialEndWidth * progress;

            yield return null;
        }


        lineRenderer.material.color = initialColor;

        initial_time = Time.time;
        progress = (Time.time - initial_time) / bossData.laserTime;

        // Rotate with the laser
        while (progress < 1.0f && health.GetHealth() > 0)
        {
            progress = (Time.time - initial_time) / bossData.laserTime;

            startRotation = Quaternion.Euler(0, bossData.laserRotateSpeed,0) * startRotation;

            // Update lasers
            for(int b = 0; b < laserCount; ++b)
            {
                float magnitude = 50f;
                // If we hit a wall, reduce distance
                //if (Physics.Raycast(transform.position, Quaternion.Euler(0, bossData.laserRotateSpeed, 0) * laserPoints[(b * 2) + 1].normalized,
                //    out RaycastHit hit, magnitude, wallMask))
                //{
                //    magnitude = Vector3.Distance(hit.point, transform.position);
                //d}
                laserPoints[(b * 2) + 1] = Quaternion.Euler(0, bossData.laserRotateSpeed, 0) * laserPoints[(b* 2) + 1].normalized * magnitude;
            }

            lineRenderer.SetPositions(laserPoints);

            for (int b = 0; b < laserCount; ++b)
            {
                RaycastHit hitObj;
                // Only raycast for the player
                int layerMask = LayerMask.GetMask("Player");

                if (Physics.Raycast(lineRenderer.GetPosition(b * 2), (lineRenderer.GetPosition((b * 2) + 1) - lineRenderer.GetPosition(b * 2)).normalized,
                    out hitObj, Vector3.Distance(lineRenderer.GetPosition((b * 2) + 1), lineRenderer.GetPosition(b * 2)) + 1f, layerMask))
                {
                    PlayerHasHealth hitPlayer;
                    if (hitObj.collider.TryGetComponent<PlayerHasHealth>(out hitPlayer))
                    {
                        // Damage player
                        hitPlayer.AlterHealth(-1, DeathCauses.Enemy);
                    }
                }
            }

            

            yield return new WaitForFixedUpdate();
        }

        lineRenderer.enabled = false;

        yield return new WaitForSeconds(bossData.attackSpeed);
        attacking = false;
        canMoveInFixedUpdate = true;
    }


    public void SpawnClerics(int spawnCount)
    {
        for(int a = 0; a < spawnCount; ++a)
        {
            int randomLocIndex = Random.Range(0, clericSpawners.Count);
            Vector3 spawnPos = clericSpawners[randomLocIndex].position + new Vector3(0, 0.6f, 0);
            GameObject spawnedEnemy = Instantiate(clericPrefab, spawnPos, Quaternion.identity);
            spawnedClerics.Add(spawnedEnemy);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // On trigger enter for laser and shockwave
        PlayerHasHealth playerHealth = other.GetComponent<PlayerHasHealth>();
        if (playerHealth != null)
        {
            Debug.Log("Player damaged from boss");
            playerHealth.AlterHealth(-1, DeathCauses.Enemy);
        }

    }
}
