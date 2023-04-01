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

    public static Dictionary<int, PedestalInfo> pedestalInfos;

    private void Start() {
        pedestalInfos = new Dictionary<int, PedestalInfo> {
            { 1, new PedestalInfo(new Vector3(10, 0, 0)) }, { 2, new PedestalInfo(new Vector3(-10, 0, 0)) },
            { 3, new PedestalInfo(new Vector3(0, 0, -9)) }
        };
        map = ConfigManager.GetData<MapData>("map");
        pathfinding = new Pathfinding(map.dimension.x, map.dimension.y, map.origin);
        foreach (var tile in map.unwalkableTiles) {
            Debug.Log("hello");
            pathfinding.GetNode(tile.x, tile.y).SetIsWalkable(false);
        }
    }
}