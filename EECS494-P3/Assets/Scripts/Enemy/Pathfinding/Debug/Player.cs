using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] public int width = 20;
    [SerializeField] public int height = 10;
    public EnemyBase enemy;

    private Pathfinding pathfinding;

    private void Start() {
        pathfinding = new Pathfinding(width, height);
        InvokeRepeating(nameof(findNewPath), 0, 0.5f);
    }

    private void findNewPath() {
        Debug.Log("finding path");
        var pos = transform.position;
        pathfinding.GetGrid().GetXZ(pos, out var playerX, out var playerZ);
        pathfinding.FindPath(0, 0, playerX, playerZ);
        enemy.SetTargetPosition(pos);
    }

    private void FixedUpdate() {
        if (Input.GetKey(KeyCode.W)) {
            transform.Translate(3 * Time.deltaTime * Vector3.forward);
        }

        if (Input.GetKey(KeyCode.S)) {
            transform.Translate(3 * Time.deltaTime * Vector3.back);
        }

        if (Input.GetKey(KeyCode.A)) {
            transform.Translate(3 * Time.deltaTime * Vector3.left);
        }

        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(3 * Time.deltaTime * Vector3.right);
        }
    }
}