using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BossHasHealth))]
[RequireComponent(typeof(LineRenderer))]
public class IsBoss : MonoBehaviour
{
    string configName = "BossData";
    BossAttributes bossData;

    BossHasHealth health;
    Rigidbody rb;

    // Corners of path to follow
    Vector3 leftBottomCorner = new Vector3(-8, 0.5f, -6);
    Vector3 rightBottomCorner = new Vector3(8, 0.5f, -6);
    Vector3 leftTopCorner = new Vector3(-8, 0.5f, 5);
    Vector3 rightTopCorner = new Vector3(8, 0.5f, 5);

    bool setDirection = false;
    Vector3 direction;

    // Cleric spawning variables
    [SerializeField] List<Transform> spawners;
    GameObject clericPrefab;

    // Possible projectiles to shoot
    List<GameObject> projectiles;

    float attackCooldown = 0f;

    // Ground pound variables
    public bool enabledGroundPound = false;
    bool groundPounding = false;
    [SerializeField] GameObject sprite;

    // Laser variables
    public bool enabledLaser = false;
    bool prepLaser = false;
    bool warmupLaser = false;
    bool lasering = false;
    private LineRenderer lineRenderer;
    Vector3 startCorner;

    [SerializeField] Color warmupColor;


    // Start is called before the first frame update
    void Start()
    {
        // Load config
        bossData = ConfigManager.GetData<BossAttributes>(configName);

        // Get components
        health = GetComponent<BossHasHealth>();
        health.setMaxHealth(bossData.health);
        rb = GetComponent<Rigidbody>();

        projectiles = new List<GameObject>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        clericPrefab = Resources.Load<GameObject>("Prefabs/Enemy/Cleric");


        // Load projectiles
        // 4/12 basic, 3/12 magic, 2/12 demolition, 1/12 arbalest, 1/12 ground pound, 1/12 laser
        projectiles.Add(Resources.Load<GameObject>("Prefabs/EnemyWeapons/ArbalestShot"));
        projectiles.Add(Resources.Load<GameObject>("Prefabs/EnemyWeapons/MagicArcherShot"));
        projectiles.Add(Resources.Load<GameObject>("Prefabs/EnemyWeapons/MagicArcherShot"));
        projectiles.Add(Resources.Load<GameObject>("Prefabs/EnemyWeapons/MagicArcherShot"));
        projectiles.Add(Resources.Load<GameObject>("Prefabs/EnemyWeapons/DemolitionShot"));
        projectiles.Add(Resources.Load<GameObject>("Prefabs/EnemyWeapons/DemolitionShot"));
        projectiles.Add(Resources.Load<GameObject>("Prefabs/Weapons/EnemyBasicBullet"));
        projectiles.Add(Resources.Load<GameObject>("Prefabs/Weapons/EnemyBasicBullet"));
        projectiles.Add(Resources.Load<GameObject>("Prefabs/Weapons/EnemyBasicBullet"));
        projectiles.Add(Resources.Load<GameObject>("Prefabs/Weapons/EnemyBasicBullet"));
    }

    private void Update()
    {
        if (attackCooldown <= 0f && !groundPounding && !lasering)
        {
            int randIndex = 0;
            if (prepLaser || warmupLaser || lasering || (!enabledGroundPound && !enabledLaser))
            {
                // Avoid double prepping/firing a laser
                randIndex = Random.Range(0, projectiles.Count);
            } else if (!enabledLaser)
            {
                // Add ground pound to the list
                randIndex = Random.Range(0, projectiles.Count + 1);
            } else
            {
                // Add laser and ground pound to the list
                randIndex = Random.Range(0, projectiles.Count + 2);
            }


            if(randIndex == projectiles.Count)
            {
                groundPounding = true;
                StartCoroutine(GroundPound());
                return;
            }
            else if (randIndex == projectiles.Count + 1)
            {
                prepLaser = true;
                return;
            }

            Vector3 playerDir = (IsPlayer.instance.transform.position - transform.position).normalized;
            fireBullet(projectiles[randIndex], playerDir, Shooter.Enemy, bossData.projectileSpeed, bossData.projectileLifetime);
            attackCooldown = bossData.attackSpeed;
        }
        attackCooldown -= Time.deltaTime;
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

        // Give bullet its velocity
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
    }

    private void FixedUpdate()
    {
        if (groundPounding || warmupLaser) return;

        Vector3 roundedPos = transform.position;
        roundedPos.x = (int)roundedPos.x;
        roundedPos.z = (int)roundedPos.z;
        roundedPos.y = 0.5f;

        bool inCorner = (roundedPos == leftBottomCorner || roundedPos == rightBottomCorner ||
            roundedPos == rightTopCorner || roundedPos == leftTopCorner);

        // Start a laser if requested
        if (prepLaser && inCorner)
        {
            warmupLaser = true;
            prepLaser = false;
            StartCoroutine(startLaser(rb.velocity.normalized));
            startCorner = transform.position;
            rb.velocity = Vector3.zero;
            return;
        }
        if(lasering && inCorner && startCorner != transform.position)
        {
            lasering = false;
        }


        // Set velocity to follow the path
        if(roundedPos == leftBottomCorner || onPath(leftBottomCorner, rightBottomCorner, roundedPos))
        {
            setDirection = false;
            rb.velocity = new Vector3(bossData.moveSpeed, 0, 0);
        }
        else if (roundedPos == rightBottomCorner || onPath(rightBottomCorner, rightTopCorner, roundedPos))
        {
            setDirection = false;
            rb.velocity = new Vector3(0, 0, bossData.moveSpeed);
        }
        else if (roundedPos == rightTopCorner || onPath(rightTopCorner, leftTopCorner, roundedPos))
        {
            setDirection = false;
            rb.velocity = new Vector3(-bossData.moveSpeed, 0, 0);
        }
        else if (roundedPos == leftTopCorner || onPath(leftTopCorner, leftBottomCorner, roundedPos))
        {
            setDirection = false;
            rb.velocity = new Vector3(0, 0, -bossData.moveSpeed);
        }
        else if (!setDirection)
        {
            setDirection = true;
            direction = (Vector3.zero - transform.position).normalized;
            rb.velocity = new Vector3(bossData.moveSpeed * direction.x, 0, bossData.moveSpeed * direction.z);
        } else
        {
            rb.velocity = new Vector3(bossData.moveSpeed * direction.x, 0, bossData.moveSpeed * direction.z);
        }
    }

    // Code adapted from https://forum.unity.com/threads/how-to-check-a-vector3-position-is-between-two-other-vector3-along-a-line.461474/
    private bool onPath(Vector3 left, Vector3 right, Vector3 candidate)
    {
        return Vector3.Dot((right - left).normalized, (candidate - right).normalized) == -1f &&
            Vector3.Dot((left - right).normalized, (candidate - left).normalized) == -1f;
    }

    IEnumerator GroundPound()
    {
        Vector3 targetPos = IsPlayer.instance.transform.position;

        setDirection = true;
        direction = (targetPos - transform.position).normalized;

        // Get to the location
        while(Vector3.Distance(targetPos, transform.position) > 1f)
        {
            rb.velocity = new Vector3(bossData.moveSpeed * direction.x * 2, 0, bossData.moveSpeed * direction.z * 2);

            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector3.zero;

        // Raise sprite in the air
        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / bossData.groundPoundWindup;

        Vector3 initialScale = sprite.transform.localScale;

        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / bossData.groundPoundWindup;

            // Scale sprite up
            sprite.transform.localScale = initialScale * (1 + progress * 2.0f);


            yield return null;
        }

        // Ground pound down
        initial_time = Time.time;
        progress = (Time.time - initial_time) / bossData.groundPoundTime;

        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / bossData.groundPoundTime;

            // Scale sprite up
            sprite.transform.localScale = initialScale * (1 + (1-progress) * 2.0f);


            yield return null;
        }

        // Spawn ground pound radius
        sprite.transform.localScale = initialScale;

        GameObject projectile = Instantiate(projectiles[4], transform.position, Quaternion.identity);
        projectile.GetComponent<IsExplosive>().setExplosiveRadius(3f);
        Destroy(projectile, 0.01f);

        // Reverse direction to avoid going out of bounds
        direction = -direction;

        groundPounding = false;
    }


    IEnumerator startLaser(Vector3 laserDirection)
    {
        // Warmup laser
        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / bossData.laserWindup;

        lineRenderer.enabled = true;
        lineRenderer.material.color = warmupColor;
        lineRenderer.SetPositions(new Vector3[] { 
            new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z)
            , transform.position - laserDirection * 50 });


        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / bossData.laserWindup;

            // Fade in line renderer
            // Set the width of the line
            lineRenderer.startWidth = progress / 2;
            lineRenderer.endWidth = progress / 2;

            yield return null;
        }

        // Set laser active
        lasering = true;
        warmupLaser = false;

        lineRenderer.material.color = new Color32(83, 178, 14, 219);

        // Wait to reach the corner again
        while(lasering)
        {
            lineRenderer.SetPositions(new Vector3[] {
            new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z)
            , transform.position - laserDirection * 50 });

            RaycastHit hitObj;
            // Only raycast for the player
            int layerMask = LayerMask.GetMask("Player");

            if(Physics.Raycast(transform.position, (transform.position - laserDirection * 50).normalized, out hitObj, 50f, layerMask))
            {
                PlayerHasHealth hitPlayer;
                if(hitObj.collider.TryGetComponent<PlayerHasHealth>(out hitPlayer))
                {
                    // Damage player
                    hitPlayer.AlterHealth(-1);
                }
            }

            yield return new WaitForFixedUpdate();
        }

        lineRenderer.enabled = false;
        prepLaser = false;
        warmupLaser = false;
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
}
