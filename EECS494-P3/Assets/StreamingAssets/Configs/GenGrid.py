import json

dimension = {"x": 59, "y": 39}
origin = {"x": -29.5, "y": 0, "z": -19.5}

map_dict = {"key": "map", "dimension": dimension, "origin": origin, "unwalkableTiles": []}


def setUnwalkableXY(xStart, yStart, xLength, yLength):
    for i in range(xStart, xStart + xLength):
        for j in range(yStart, yStart + yLength):
            if {"x": i, "y": j} not in map_dict["unwalkableTiles"]:
                map_dict["unwalkableTiles"].append({"x": i, "y": j})
            if {"x": dimension['x'] - 1 - i, "y": j} not in map_dict["unwalkableTiles"]:
                map_dict["unwalkableTiles"].append({"x": dimension['x'] - 1 - i, "y": j})


setUnwalkableXY(4, 0, 26, 2)
setUnwalkableXY(0, 4, 2, 31)

setUnwalkableXY(5, 5, 6, 2)
setUnwalkableXY(16, 5, 6, 2)
setUnwalkableXY(22, 5, 2, 4)

setUnwalkableXY(2, 11, 3, 1)
setUnwalkableXY(8, 11, 6, 1)

setUnwalkableXY(13, 18, 1, 5)

setUnwalkableXY(2, 29, 3, 1)
setUnwalkableXY(8, 29, 6, 1)

setUnwalkableXY(4, 37, 26, 2)
setUnwalkableXY(8, 36, 6, 1)
setUnwalkableXY(22, 35, 8, 2)

setUnwalkableXY(22, 14, 3, 1)
setUnwalkableXY(22, 15, 6, 2)
setUnwalkableXY(23, 17, 2, 7)

with open('map.json', 'w') as f:
    json.dump(map_dict, f, indent=4)
