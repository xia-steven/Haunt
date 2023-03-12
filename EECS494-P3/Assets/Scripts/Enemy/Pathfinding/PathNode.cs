/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode {
    private readonly Grid<PathNode> grid;
    public readonly int x;
    public readonly int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;
    public PathNode cameFromNode;

    public PathNode(Grid<PathNode> grid_, int x_, int y_) {
        grid = grid_;
        x = x_;
        y = y_;
        isWalkable = true;
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool walkable_) {
        isWalkable = walkable_;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString() {
        return x + "," + y;
    }
}