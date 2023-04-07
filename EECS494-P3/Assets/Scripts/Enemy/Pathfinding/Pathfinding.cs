using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding {
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 30;

    public static Pathfinding Instance { get; private set; }

    private readonly Grid<PathNode> grid;
    private HashSet<PathNode> openList;
    private HashSet<PathNode> closedList;

    public Pathfinding(int width, int height, Vector3 origin) {
        Instance = this;
        grid = new Grid<PathNode>(width, height, 1f, origin,
            (g, x, z) => new PathNode(g, x, z));
    }

    public Grid<PathNode> GetGrid() {
        return grid;
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition) {
        grid.GetXZ(endWorldPosition, out var endX, out var endZ);
        return FindPath(startWorldPosition, endX, endZ);
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, int endX, int endZ) {
        grid.GetXZ(startWorldPosition, out var startX, out var startZ);

        var path = FindPath(startX, startZ, endX, endZ);
        if (path == null) {
            return null;
        }

        var vectorPath = new List<Vector3>();
        foreach (var pathNode in path) {
            vectorPath.Add(new Vector3(pathNode.x, 0, pathNode.z) * grid.GetCellSize() +
                           grid.GetCellSize() * .5f * Vector3.one);
        }

        return vectorPath;
    }

    private List<PathNode> FindPath(int startX, int startZ, int endX, int endZ) {
        var startNode = grid.GetGridObject(startX, startZ);
        var endNode = grid.GetGridObject(endX, endZ);

        if (startNode == null || endNode == null) {
            // Invalid Path
            return null;
        }

        openList = new HashSet<PathNode> { startNode };
        closedList = new HashSet<PathNode>();

        for (var x = 0; x < grid.GetWidth(); x++) {
            for (var z = 0; z < grid.GetHeight(); z++) {
                var pathNode = grid.GetGridObject(x, z);
                pathNode.gCost = 99999999;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0) {
            var currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode) {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbourNode in GetNeighbourList(currentNode)) {
                if (closedList.Contains(neighbourNode)) {
                    continue;
                }

                if (!neighbourNode.isWalkable) {
                    closedList.Add(neighbourNode);
                    continue;
                }

                var tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // Out of nodes on the openList
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode) {
        var neighbourList = new List<PathNode>();

        if (currentNode.x - 1 >= 0) {
            // Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z));
            // Left Down
            if (currentNode.z - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z - 1));
            // Left Up
            if (currentNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z + 1));
        }

        if (currentNode.x + 1 < grid.GetWidth()) {
            // Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z));
            // Right Down
            if (currentNode.z - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z - 1));
            // Right Up
            if (currentNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z + 1));
        }

        // Down
        if (currentNode.z - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.z - 1));
        // Up
        if (currentNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.z + 1));

        return neighbourList;
    }

    public PathNode GetNode(int x, int z) {
        return grid.GetGridObject(x, z);
    }

    private static List<PathNode> CalculatePath(PathNode endNode) {
        var path = new List<PathNode> { endNode };
        var currentNode = endNode;
        while (currentNode.cameFromNode != null) {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }

        path.Reverse();
        return path;
    }

    private static int CalculateDistanceCost(PathNode a, PathNode b) {
        var xDistance = Mathf.Abs(a.x - b.x);
        var zDistance = Mathf.Abs(a.z - b.z);
        var remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private static PathNode GetLowestFCostNode(HashSet<PathNode> pathNodeList) {
        var lowestFCostNode = pathNodeList.First();

        foreach (var node in pathNodeList) {
            if (node.fCost < lowestFCostNode.fCost) {
                lowestFCostNode = node;
            }
        }

        return lowestFCostNode;
    }
}