using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BossHasHealth))]
public class IsBoss : MonoBehaviour
{
    string configName = "BossData";
    BossAttributes bossData;

    BossHasHealth health;
    Rigidbody rb;

    Vector3 direction;

    // Cleric spawning variables
    [SerializeField] List<Transform> spawners;
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
    

    // Start is called before the first frame update
    void Start() {
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
        startRotation = transform.position - 50f * Vector3.back;

        clericPrefab = Resources.Load<GameObject>("Prefabs/Enemy/Cleric");


        basicBulletPrefab = Resources.Load<GameObject>("Prefabs/Weapons/EnemyBasicBullet");
        arbalestBulletPrefab = Resources.Load<GameObject>("Prefabs/EnemyWeapons/ArbalestShot");
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

            if(attackIndex < 40)
            {
                StartCoroutine(basicAttack());
            }
            else if (attackIndex < 70)
            {
                StartCoroutine(arbalestAttack());
            }
            else if (attackIndex < 90)
            {
                StartCoroutine(GroundPound());
            }
            else if (attackIndex < 100)
            {
                // Fire more lasers as health decreases
                int laserNum = 1 + (int)((1 - health.GetHealth() / bossData.health) * 4);
                StartCoroutine(FireLaser(laserNum));
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

    IEnumerator GroundPound()
    {
        canMoveInFixedUpdate = false;

        // Make sure fixedUpdate gets disabled
        yield return null;

        Vector3 targetPos = new Vector3(0, 0.5f, 0);

        //setDirection = true;
        direction = (targetPos - transform.position).normalized;

        // Get to the location
        while(Vector3.Distance(targetPos, transform.position) > 0.25f)
        {
            rb.velocity = new Vector3(bossData.moveToCenterSpeed * direction.x, 0, bossData.moveToCenterSpeed * direction.z);

            yield return new WaitForFixedUpdate();
        }

        rb.position = targetPos;

        rb.velocity = Vector3.zero;

        // Raise sprite in the air
        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / bossData.shockwaveWindup;

        Vector3 initialScale = sprite.transform.localScale;

        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / bossData.shockwaveWindup;

            // Scale sprite up
            sprite.transform.localScale = initialScale * (1 + progress * 2.0f);


            yield return null;
        }

        // Ground pound down
        initial_time = Time.time;
        progress = (Time.time - initial_time) / bossData.shockwavePound;

        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / bossData.shockwavePound;

            // Scale sprite down
            sprite.transform.localScale = initialScale * (1 + (1-progress) * 2.0f);


            yield return null;
        }

        // Spawn shockwave line renderer
        // Code adapted from https://www.loekvandenouweland.com/content/use-linerenderer-in-unity-to-draw-a-circle.html
        int segmentCount = 36;
        lineRenderer.useWorldSpace = true;
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

        while(progress < 1.0f)
        {
            progress = (Time.time - initial_time) / bossData.shockwaveTime;

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
        yield return new WaitForSeconds(bossData.attackSpeed);
        attacking = false;
        canMoveInFixedUpdate = true;
    }


    IEnumerator FireLaser(int laserCount)
    {
        canMoveInFixedUpdate = false;

        // Make sure fixedUpdate gets disabled
        yield return null;

        Vector3 targetPos = new Vector3(0, 0.5f, 0);

        direction = (targetPos - transform.position).normalized;

        // Get to the location
        while (Vector3.Distance(targetPos, transform.position) > 0.5f)
        {
            rb.velocity = new Vector3(bossData.moveToCenterSpeed * direction.x, 0, bossData.moveToCenterSpeed * direction.z);

            yield return new WaitForFixedUpdate();
        }

        rb.position = targetPos;

        rb.velocity = Vector3.zero;


        // Warmup laser
        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / bossData.laserWindup;

        Color initialColor = lineRenderer.material.color;
        lineRenderer.enabled = true;
        lineRenderer.material.color = warmupColor;

        float initialStartWidth = lineRenderer.startWidth;
        float initialEndWidth = lineRenderer.endWidth;

        Vector3[] laserPoints = new Vector3[laserCount * 2];

        float offsetPerLaser = 360f / (float)laserCount;

        for(int a = 0; a < laserCount * 2; ++a)
        {
            if(a % 2 == 0)
            {
                laserPoints[a] =  new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
            }
            else
            {
                laserPoints[a] = Quaternion.Euler(0, offsetPerLaser * ((a + 1) / 2), 0) * startRotation;
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
                laserPoints[(b * 2) + 1] = Quaternion.Euler(0, bossData.laserRotateSpeed, 0) * laserPoints[(b* 2) + 1];
            }

            lineRenderer.SetPositions(laserPoints);

            for (int b = 0; b < laserCount; ++b)
            {
                RaycastHit hitObj;
                // Only raycast for the player
                int layerMask = LayerMask.GetMask("Player");

                if (Physics.Raycast(lineRenderer.GetPosition(b * 2), (lineRenderer.GetPosition((b * 2) + 1) - lineRenderer.GetPosition(b * 2)).normalized,
                    out hitObj, lineRenderer.GetPosition((b * 2) + 1).magnitude, layerMask))
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
            int randomLocIndex = Random.Range(0, spawners.Count);
            Vector3 spawnPos = spawners[randomLocIndex].position + new Vector3(0, 0.6f, 0);
            GameObject spawnedEnemy = Instantiate(clericPrefab, spawnPos, Quaternion.identity); 
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
