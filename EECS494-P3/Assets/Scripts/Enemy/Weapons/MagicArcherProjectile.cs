using UnityEngine;
using Weapons;

namespace Enemy.Weapons {
    [RequireComponent(typeof(Rigidbody))]
    public class MagicArcherProjectile : EnemyBasicBullet {
        private Rigidbody rb;

        private GameObject miniBullet;

        // Time since last reversal
        public float lastReverse;

        // Scale of the bullet
        private const float scale = 0.23f;
        private const int gridWidth = 5;

        private const int gridHeight = 5;

        // Bytes of the grid
        private int grid;

        private Vector3 direction;


        private void Start() {
            rb = GetComponent<Rigidbody>();

            direction = rb.velocity;

            // Get mini bullet prefab
            miniBullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/MagicArcherMiniBullet");

            // Instantiate grid
            initializeGrid();

            // Base of the grid
            var basePos = -(new Vector3(2, 0, 2) * scale);

            for (var a = 0; a < gridHeight; ++a)
            for (var b = 0; b < gridWidth; ++b) {
                // If the grid needs a bullet in this location, check is 1
                var check = (grid >> (b + gridWidth * a)) & 1;
                switch (check) {
                    case 1: {
                        var bullet = Instantiate(miniBullet, transform);
                        bullet.transform.localPosition =
                            Quaternion.LookRotation(direction) * (basePos + new Vector3(a, 0, b) * scale);
                        break;
                    }
                }
            }
        }

        public new void setLifetime(float lifetime) {
            bulletLife = lifetime;
        }

        public new void OnTriggerEnter(Collider other) {
            base.OnTriggerEnter(other);
        }

        private void initializeGrid() {
            // Choose a random shape
            var shape = (MagicArcherShapes)Random.Range(0, 4);

            grid = shape switch {
                MagicArcherShapes.Cross => 0b1000101010001000101010001,
                MagicArcherShapes.Plus => 0b0010000100111110010000100,
                MagicArcherShapes.Circle => 0b0111010001100011000101110,
                MagicArcherShapes.Line => 0b0010000100001000010000100,
                _ => grid
            };
        }
    }


    internal enum MagicArcherShapes {
        Cross,
        Plus,
        Circle,
        Line
    }
}