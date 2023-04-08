using System.Collections;
using System.Collections.Generic;
using ConfigDataTypes;
using Enemy.Weapons;
using Events;
using JSON_Parsing;
using Player;
using UnityEngine;
using Weapons;

namespace Enemy {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BossHasHealth))]
    [RequireComponent(typeof(LineRenderer))]
    public class IsBoss : MonoBehaviour {
        private readonly string configName = "BossData";
        private BossAttributes bossData;

        private BossHasHealth health;
        private Rigidbody rb;

        // Corners of path to follow
        private readonly Vector3 leftBottomCorner = new(-8, 0.5f, -6);
        private readonly Vector3 rightBottomCorner = new(8, 0.5f, -6);
        private readonly Vector3 leftTopCorner = new(-8, 0.5f, 5);
        private readonly Vector3 rightTopCorner = new(8, 0.5f, 5);

        private bool setDirection;
        private Vector3 direction;

        // Cleric spawning variables
        [SerializeField] private List<Transform> spawners;
        private GameObject clericPrefab;

        // Possible projectiles to shoot
        private List<GameObject> projectiles;

        private float attackCooldown;

        // Ground pound variables
        public bool enabledGroundPound;
        private bool groundPounding;
        [SerializeField] private GameObject sprite;

        // Laser variables
        public bool enabledLaser;
        private bool prepLaser;
        private bool warmupLaser;
        private bool lasering;
        private LineRenderer lineRenderer;
        private Vector3 startCorner;

        [SerializeField] private Color warmupColor;


        // Start is called before the first frame update
        private void Start() {
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

        private void Update() {
            switch (attackCooldown) {
                case <= 0f when !groundPounding && !lasering: {
                    int randIndex;
                    if (prepLaser || warmupLaser || lasering || (!enabledGroundPound && !enabledLaser))
                        // Avoid double prepping/firing a laser
                        randIndex = Random.Range(0, projectiles.Count);
                    else
                        randIndex = enabledLaser switch {
                            // Add ground pound to the list
                            false => Random.Range(0, projectiles.Count + 1),
                            _ => Random.Range(0, projectiles.Count + 2)
                        };

                    if (randIndex == projectiles.Count) {
                        groundPounding = true;
                        StartCoroutine(GroundPound());
                        return;
                    }

                    if (randIndex == projectiles.Count + 1) {
                        prepLaser = true;
                        return;
                    }

                    var playerDir = (IsPlayer.instance.transform.position - transform.position).normalized;
                    fireBullet(projectiles[randIndex], playerDir, Shooter.Enemy, bossData.projectileSpeed,
                        bossData.projectileLifetime);
                    attackCooldown = bossData.attackSpeed;
                    break;
                }
            }

            attackCooldown -= Time.deltaTime;
        }


        // Base function to use for firing projectiles
        private void fireBullet(GameObject bullet, Vector3 direction_, Shooter shooter, float projectileSpeed,
            float lifetime) {
            // Spawn bullet at barrel of gun
            var projectile = Instantiate(bullet, transform.position, Quaternion.identity);

            var enemyBull = projectile.GetComponent<EnemyBasicBullet>();
            // Set shooter to holder of gun (enemy or player)
            enemyBull.SetShooter(shooter);
            enemyBull.setLifetime(lifetime);

            // Give bullet its velocity
            projectile.GetComponent<Rigidbody>().velocity = direction_ * projectileSpeed;
        }

        private void FixedUpdate() {
            if (groundPounding || warmupLaser) return;

            var roundedPos = transform.position;
            roundedPos.x = (int)roundedPos.x;
            roundedPos.z = (int)roundedPos.z;
            roundedPos.y = 0.5f;

            var inCorner = roundedPos == leftBottomCorner || roundedPos == rightBottomCorner ||
                           roundedPos == rightTopCorner || roundedPos == leftTopCorner;

            switch (prepLaser) {
                // Start a laser if requested
                case true when inCorner:
                    warmupLaser = true;
                    prepLaser = false;
                    StartCoroutine(startLaser(rb.velocity.normalized));
                    startCorner = transform.position;
                    rb.velocity = Vector3.zero;
                    return;
            }

            lasering = lasering switch {
                true when inCorner && startCorner != transform.position => false,
                _ => lasering
            };

            // Set velocity to follow the path
            if (roundedPos == leftBottomCorner || onPath(leftBottomCorner, rightBottomCorner, roundedPos)) {
                setDirection = false;
                rb.velocity = new Vector3(bossData.moveSpeed, 0, 0);
            }
            else if (roundedPos == rightBottomCorner || onPath(rightBottomCorner, rightTopCorner, roundedPos)) {
                setDirection = false;
                rb.velocity = new Vector3(0, 0, bossData.moveSpeed);
            }
            else if (roundedPos == rightTopCorner || onPath(rightTopCorner, leftTopCorner, roundedPos)) {
                setDirection = false;
                rb.velocity = new Vector3(-bossData.moveSpeed, 0, 0);
            }
            else if (roundedPos == leftTopCorner || onPath(leftTopCorner, leftBottomCorner, roundedPos)) {
                setDirection = false;
                rb.velocity = new Vector3(0, 0, -bossData.moveSpeed);
            }
            else {
                switch (setDirection) {
                    case false:
                        setDirection = true;
                        direction = (Vector3.zero - transform.position).normalized;
                        rb.velocity = new Vector3(bossData.moveSpeed * direction.x, 0,
                            bossData.moveSpeed * direction.z);
                        break;
                    default:
                        rb.velocity = new Vector3(bossData.moveSpeed * direction.x, 0,
                            bossData.moveSpeed * direction.z);
                        break;
                }
            }
        }

        // Code adapted from https://forum.unity.com/threads/how-to-check-a-vector3-position-is-between-two-other-vector3-along-a-line.461474/
        private static bool onPath(Vector3 left, Vector3 right, Vector3 candidate) {
            return Vector3.Dot((right - left).normalized, (candidate - right).normalized) == -1f &&
                   Vector3.Dot((left - right).normalized, (candidate - left).normalized) == -1f;
        }

        private IEnumerator GroundPound() {
            var targetPos = IsPlayer.instance.transform.position;

            setDirection = true;
            direction = (targetPos - transform.position).normalized;

            // Get to the location
            while (Vector3.Distance(targetPos, transform.position) > 1f) {
                rb.velocity = new Vector3(bossData.moveSpeed * direction.x * 2, 0,
                    bossData.moveSpeed * direction.z * 2);

                yield return new WaitForFixedUpdate();
            }

            rb.velocity = Vector3.zero;

            // Raise sprite in the air
            var initial_time = Time.time;
            var progress = (Time.time - initial_time) / bossData.groundPoundWindup;

            var initialScale = sprite.transform.localScale;

            while (progress < 1.0f) {
                progress = (Time.time - initial_time) / bossData.groundPoundWindup;

                // Scale sprite up
                sprite.transform.localScale = initialScale * (1 + progress * 2.0f);


                yield return null;
            }

            // Ground pound down
            initial_time = Time.time;
            progress = (Time.time - initial_time) / bossData.groundPoundTime;

            while (progress < 1.0f) {
                progress = (Time.time - initial_time) / bossData.groundPoundTime;

                // Scale sprite up
                sprite.transform.localScale = initialScale * (1 + (1 - progress) * 2.0f);


                yield return null;
            }

            // Spawn ground pound radius
            sprite.transform.localScale = initialScale;

            var projectile = Instantiate(projectiles[4], transform.position, Quaternion.identity);
            projectile.GetComponent<IsExplosive>().setExplosiveRadius(3f);
            Destroy(projectile, 0.01f);

            // Reverse direction to avoid going out of bounds
            direction = -direction;

            groundPounding = false;
        }


        private IEnumerator startLaser(Vector3 laserDirection) {
            // Warmup laser
            var initial_time = Time.time;
            var progress = (Time.time - initial_time) / bossData.laserWindup;

            lineRenderer.enabled = true;
            lineRenderer.material.color = warmupColor;
            var position = transform.position;
            lineRenderer.SetPositions(new[] {
                new Vector3(position.x, position.y - 0.1f, position.z),
                position - laserDirection * 50
            });


            while (progress < 1.0f) {
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
            while (lasering) {
                var position1 = transform.position;
                lineRenderer.SetPositions(new[] {
                    new(position1.x, position1.y - 0.1f, position1.z), position1 - laserDirection * 50
                });
                // Only raycast for the player
                var layerMask = LayerMask.GetMask("Player");

                if (Physics.Raycast(transform.position, (transform.position - laserDirection * 50).normalized,
                        out var hitObj, 50f, layerMask))
                    if (hitObj.collider.TryGetComponent(out PlayerHasHealth hitPlayer))
                        // Damage player
                        hitPlayer.AlterHealth(-1, DeathCauses.Enemy);

                yield return new WaitForFixedUpdate();
            }

            lineRenderer.enabled = false;
            prepLaser = false;
            warmupLaser = false;
        }


        public void SpawnClerics(int spawnCount) {
            for (var a = 0; a < spawnCount; ++a) {
                var randomLocIndex = Random.Range(0, spawners.Count);
                var spawnPos = spawners[randomLocIndex].position + new Vector3(0, 0.6f, 0);
                Instantiate(clericPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}