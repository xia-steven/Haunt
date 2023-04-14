using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MagicArcherProjectile : EnemyBasicBullet {
    Rigidbody rb;

    GameObject miniBullet;

    // Time since last reversal
    public float lastReverse;

    // Scale of the bullet
    float scale = 0.3f;
    int gridWidth = 5;

    int gridHeight = 5;

    // Bytes of the grid
    int grid;

    Vector3 direction;


    private void Start() {
        rb = GetComponent<Rigidbody>();

        direction = rb.velocity;

        // Get mini bullet prefab
        miniBullet = Resources.Load<GameObject>("Prefabs/EnemyWeapons/MagicArcherMiniBullet");

        // Instantiate grid
        initializeGrid();

        // Base of the grid
        Vector3 basePos = -(new Vector3(2, 0, 2) * scale);

        for (int a = 0; a < gridHeight; ++a) {
            for (int b = 0; b < gridWidth; ++b) {
                // If the grid needs a bullet in this location, check is 1
                int check = grid >> (b + gridWidth * a) & 1;
                if (check == 1) {
                    GameObject bullet = Instantiate(miniBullet, transform);
                    bullet.transform.localPosition =
                        Quaternion.LookRotation(direction) * (basePos + new Vector3(a, 0, b) * scale);
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

    void initializeGrid() {
        // Choose a random shape
        MagicArcherShapes shape = (MagicArcherShapes)Random.Range(0, 4);

        if (shape == MagicArcherShapes.Cross) {
            grid = 0b1000101010001000101010001;
        }
        else if (shape == MagicArcherShapes.Plus) {
            grid = 0b0010000100111110010000100;
        }
        else if (shape == MagicArcherShapes.Circle) {
            grid = 0b0111010001100011000101110;
        }
        else if (shape == MagicArcherShapes.Line) {
            grid = 0b0010000100001000010000100;
        }
    }
}


enum MagicArcherShapes {
    Cross,
    Plus,
    Circle,
    Line
}