using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    private Grid<Node> grid;
    private int x, y;

    public int g, h, f;
    public Node prev;

    public Node(Grid<Node> grid, int x, int y) {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }
}