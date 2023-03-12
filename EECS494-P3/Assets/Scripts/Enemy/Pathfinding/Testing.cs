/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using UnityEngine;
using CodeMonkey.Utils;

public class Testing : MonoBehaviour {
    [SerializeField] private PathfindingDebugStepVisual pathfindingDebugStepVisual;
    [SerializeField] private PathfindingVisual pathfindingVisual;
    private Pathfinding pathfinding;

    private void Start() {
        pathfinding = new Pathfinding(20, 10);
        pathfindingDebugStepVisual.Setup(pathfinding.GetGrid());
        pathfindingVisual.SetGrid(pathfinding.GetGrid());
        InvokeRepeating(nameof(findNewPath), 0f, 0.5f);
    }

    private void findNewPath() {
        if (Input.GetMouseButton(0)) {
            var mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out var x, out var y);
            pathfinding.FindPath(0, 0, x, y);
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            var mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out var x, out var y);
            pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
        }
    }
}