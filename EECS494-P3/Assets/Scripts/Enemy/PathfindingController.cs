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
    public Vector3 origin;
    public List<coordinate> unwalkableTiles;
}

public class PathfindingController : MonoBehaviour {
    private Pathfinding pathfinding;
    public static MapData map;

    private void Start() {
        map = ConfigManager.GetData<MapData>("map");
        pathfinding = new Pathfinding(map.dimension.x, map.dimension.y, map.origin);
        foreach (var tile in map.unwalkableTiles) {
            pathfinding.GetNode(tile.x, tile.y).SetIsWalkable(false);
        }
    }
}