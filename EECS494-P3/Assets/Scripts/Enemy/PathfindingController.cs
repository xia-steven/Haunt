using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class coordinate {
    public int x;
    public int y;
}

public class MapData : Savable {
    public coordinate dimension;
    public List<coordinate> unwalkableTiles;
}

public class Pair<T, V> {
    public T first;
    public V second;

    public Pair(T first_, V second_) {
        first = first_;
        second = second_;
    }
}

public class PathfindingController : MonoBehaviour {
    private static Pathfinding pathfinding;
    public static MapData map;

    public static Dictionary<int, PedestalInfo> pedestalInfos;

    private void Start() {
        pedestalInfos = new Dictionary<int, PedestalInfo> {
            { 1, new PedestalInfo(new Vector3(10, 0, 0)) }, { 2, new PedestalInfo(new Vector3(-10, 0, 0)) },
            { 3, new PedestalInfo(new Vector3(0, 0, -9)) }
        };
        map = ConfigManager.GetData<MapData>("map");
        pathfinding = new Pathfinding(map.dimension.x, map.dimension.y,
            new Vector3(-(float)map.dimension.x / 2, 0, -(float)map.dimension.y / 2));
        foreach (var tile in map.unwalkableTiles) {
            pathfinding.GetNode(tile.x, tile.y).SetIsWalkable(false);
        }
    }

    public static void FindClosestWalkable(Vector3 worldPosition, out int targetX, out int targetZ) {
        var grid = pathfinding.GetGrid();
        grid.GetXZ(worldPosition, out var x, out var z);
        targetX = x;
        targetZ = z;
        var visited = new HashSet<Pair<int, int>>();
        var bfs = new Queue<Pair<int, int>>();
        bfs.Enqueue(new Pair<int, int>(x, z));
        visited.Add(new Pair<int, int>(x, z));
        while (bfs.Count > 0) {
            var front = bfs.Peek();
            if (grid.GetGridObject(front.first, front.second).isWalkable) {
                targetX = front.first;
                targetZ = front.second;
                return;
            }

            if (!visited.Contains(new Pair<int, int>(front.first, front.second + 1))) {
                bfs.Enqueue(new Pair<int, int>(front.first, front.second + 1));
                visited.Add(new Pair<int, int>(front.first, front.second + 1));
            }

            if (!visited.Contains(new Pair<int, int>(front.first, front.second - 1))) {
                bfs.Enqueue(new Pair<int, int>(front.first, front.second - 1));
                visited.Add(new Pair<int, int>(front.first, front.second - 1));
            }

            if (!visited.Contains(new Pair<int, int>(front.first + 1, front.second))) {
                bfs.Enqueue(new Pair<int, int>(front.first + 1, front.second));
                visited.Add(new Pair<int, int>(front.first + 1, front.second));
            }

            if (!visited.Contains(new Pair<int, int>(front.first - 1, front.second))) {
                bfs.Enqueue(new Pair<int, int>(front.first - 1, front.second));
                visited.Add(new Pair<int, int>(front.first - 1, front.second));
            }

            bfs.Dequeue();
        }
    }
}