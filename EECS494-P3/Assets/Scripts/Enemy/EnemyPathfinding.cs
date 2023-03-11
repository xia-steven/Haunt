using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding {
    private Grid<Node> grid;

    public EnemyPathfinding(int w, int h) {
        grid = new Grid<Node>(w, h, 1f, Vector3.zero, (Grid<Node> g, int x, int y) => new Node(g, x, y));
    }
}