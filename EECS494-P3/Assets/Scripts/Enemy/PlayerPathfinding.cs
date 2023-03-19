using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPathfinding : MonoBehaviour {
    private Pathfinding pathfinding;

    private void setUnwalkableX(int start, int length, int y) {
        for (var i = start; i < start + length; ++i) {
            pathfinding.GetNode(i, y).SetIsWalkable(false);
        }
    }

    private void setUnwalkableY(int start, int length, int x) {
        for (var i = start; i < start + length; ++i) {
            pathfinding.GetNode(x, i).SetIsWalkable(false);
        }
    }

    private void setUnwalkableXY(int xStart, int yStart, int xLength, int yLength) {
        for (var i = xStart; i < xStart + xLength; ++i) {
            for (var j = yStart; j < yStart + yLength; ++j) {
                pathfinding.GetNode(i, j).SetIsWalkable(false);
            }
        }
    }

    private void Start() {
        pathfinding = new Pathfinding(35, 22);

        // ------------------Left 1/3 of Arena-------------------
        // Bottom left
        setUnwalkableXY(2, 0, 2, 2);
        setUnwalkableXY(0, 2, 2, 2);

        // Middle left
        setUnwalkableX(0, 4, 5);
        setUnwalkableY(5, 4, 3);
        setUnwalkableX(1, 1, 9);

        // Bottom middle
        setUnwalkableY(0, 5, 8);

        // Top left
        setUnwalkableX(0, 1, 17);
        setUnwalkableX(1, 1, 16);
        setUnwalkableX(2, 1, 15);
        setUnwalkableX(3, 1, 14);

        setUnwalkableX(4, 1, 21);
        setUnwalkableX(5, 1, 20);
        setUnwalkableX(6, 1, 19);
        setUnwalkableX(7, 1, 18);

        setUnwalkableXY(0, 18, 2, 2);
        setUnwalkableXY(2, 20, 2, 2);

        // ------------------Right 1/3 of Arena-------------------
        // Bottom right
        setUnwalkableXY(31, 0, 2, 2);
        setUnwalkableXY(33, 2, 2, 2);

        // Middle right
        setUnwalkableX(31, 4, 5);
        setUnwalkableY(5, 4, 31);
        setUnwalkableX(33, 1, 9);

        // Bottom middle
        setUnwalkableY(0, 5, 26);

        // Top right
        setUnwalkableX(34, 1, 17);
        setUnwalkableX(33, 1, 16);
        setUnwalkableX(32, 1, 15);
        setUnwalkableX(31, 1, 14);

        setUnwalkableX(30, 1, 21);
        setUnwalkableX(29, 1, 20);
        setUnwalkableX(28, 1, 19);
        setUnwalkableX(27, 1, 18);

        setUnwalkableXY(33, 18, 2, 2);
        setUnwalkableXY(31, 20, 2, 2);

        // ------------------Right 1/3 of Arena-------------------
        // Bottom
        setUnwalkableY(1, 3, 15);
        setUnwalkableY(1, 3, 19);
        setUnwalkableX(14, 7, 5);

        // Top
        setUnwalkableY(19, 3, 17);
        setUnwalkableX(14, 7, 18);

        setUnwalkableX(10, 1, 20);
        setUnwalkableX(24, 1, 20);

        // Middle
        setUnwalkableX(11, 13, 8);
        setUnwalkableX(11, 13, 14);
        setUnwalkableY(9, 5, 13);
        setUnwalkableY(9, 5, 21);

        InvokeRepeating(nameof(PublishPosition), 0, 1f);
    }

    private void PublishPosition() {
        EventBus.Publish(new PlayerPositionEvent(transform.position));
    }
}