using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemy.Pathfinding {
    public class AStar {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        public static AStar Instance { get; private set; }

        private readonly Grid<PathNode> grid;
        private HashSet<PathNode> openList;
        private HashSet<PathNode> closedList;

        public AStar(int width, int height, Vector3 origin) {
            Instance = this;
            grid = new Grid<PathNode>(width, height, 1f, origin, static (g, x, z) => new PathNode(g, x, z));
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

            return path?.Select(pathNode =>
                    new Vector3(pathNode.x, 0, pathNode.z) * grid.GetCellSize() +
                    grid.GetCellSize() * .5f * Vector3.one)
                .ToList();
        }

        private IEnumerable<PathNode> FindPath(int startX, int startZ, int endX, int endZ) {
            var startNode = grid.GetGridObject(startX, startZ);
            var endNode = grid.GetGridObject(endX, endZ);

            if (startNode == null || endNode == null)
                // Invalid Path
                return null;

            openList = new HashSet<PathNode> { startNode };
            closedList = new HashSet<PathNode>();

            for (var x = 0; x < grid.GetWidth(); x++)
            for (var z = 0; z < grid.GetHeight(); z++) {
                var pathNode = grid.GetGridObject(x, z);
                pathNode.gCost = 99999999;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0) {
                var currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode) return CalculatePath(endNode);

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (var neighbourNode in GetNeighbourList(currentNode)
                             .Where(neighbourNode => !closedList.Contains(neighbourNode))) {
                    switch (neighbourNode.isWalkable) {
                        case false:
                            closedList.Add(neighbourNode);
                            continue;
                    }

                    var tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost >= neighbourNode.gCost) continue;
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    openList.Add(neighbourNode);
                }
            }

            // Out of nodes on the openList
            return null;
        }

        private IEnumerable<PathNode> GetNeighbourList(PathNode currentNode) {
            var neighbourList = new List<PathNode>();

            switch (currentNode.x - 1) {
                case >= 0: {
                    // Left
                    neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z));
                    switch (currentNode.z - 1) {
                        // Left Down
                        case >= 0:
                            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z - 1));
                            break;
                    }
                    // Left Up
                    if (currentNode.z + 1 < grid.GetHeight())
                        neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z + 1));
                    break;
                }
            }

            if (currentNode.x + 1 < grid.GetWidth()) {
                // Right
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z));
                switch (currentNode.z - 1) {
                    // Right Down
                    case >= 0:
                        neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z - 1));
                        break;
                }
                // Right Up
                if (currentNode.z + 1 < grid.GetHeight())
                    neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z + 1));
            }

            switch (currentNode.z - 1) {
                // Down
                case >= 0:
                    neighbourList.Add(GetNode(currentNode.x, currentNode.z - 1));
                    break;
            }
            // Up
            if (currentNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.z + 1));

            return neighbourList;
        }

        public PathNode GetNode(int x, int z) {
            return grid.GetGridObject(x, z);
        }

        private static IEnumerable<PathNode> CalculatePath(PathNode endNode) {
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

            foreach (var node in pathNodeList.Where(node => node.fCost < lowestFCostNode.fCost))
                lowestFCostNode = node;

            return lowestFCostNode;
        }
    }
}