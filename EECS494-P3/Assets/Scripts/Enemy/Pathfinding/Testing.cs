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
    [SerializeField] public int width = 20;
    [SerializeField] public int height = 10;

    private Pathfinding pathfinding;

    private void Start() {
        pathfinding = new Pathfinding(width, height);
        InvokeRepeating(nameof(findNewPath), 0f, 0.5f);
    }

    private void findNewPath() {
        if (Input.GetMouseButton(0)) {
            var mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            Debug.Log("Mouse click detected");
            Debug.Log(mouseWorldPosition.x + " " + mouseWorldPosition.z);
            pathfinding.GetGrid().GetXZ(mouseWorldPosition, out var x, out var z);
            pathfinding.FindPath(0, 0, x, z);
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            var mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXZ(mouseWorldPosition, out var x, out var z);
            pathfinding.GetNode(x, z).SetIsWalkable(!pathfinding.GetNode(x, z).isWalkable);
        }
    }
}