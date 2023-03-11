using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using UnityEngine;

public class Grid {
    public int xMax, xMin, yMax, yMin; //inclusive
    private int width;
    private int height;
    private float cellSize;
    private Vector3 origin;
    private int[,] grid;

    public Grid(int w_, int h_, float cs_, Vector3 origin_) {
        width = w_;
        height = h_;
        cellSize = cs_;
        origin = origin_;

        grid = new int[w_, h_];
    }

    public Vector3 getPosition(int x, int y) {
        return new Vector3(x, y) * cellSize + origin;
    }

    public void getXY(Vector3 pos, out int x, out int y) {
        x = Mathf.FloorToInt((pos.x - origin.x) / cellSize);
        y = Mathf.FloorToInt((pos.y - origin.y) / cellSize);
    }

    public int getVal(int x, int y) {
        if (x > xMax || x < xMin || y > yMax || y < yMin) {
            return int.MaxValue;
        }

        return grid[x, y];
    }

    public void setVal(int x, int y, int val) {
        if (x >= xMin && y >= yMin && x <= xMax && y <= yMax) {
            grid[x, y] = val;
        }
    }

    public void setVal(Vector3 pos, int val) {
        getXY(pos, out var x, out var y);
        setVal(x, y, val);
    }
}