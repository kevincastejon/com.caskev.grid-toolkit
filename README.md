# GridToolkit

[![Documentation](https://img.shields.io/badge/docs-GitHub%20Pages-blue)](https://kevincastejon.github.io/com.caskev.unity-grid-toolkit/)

Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.

---
## Usage

All you need to use this API is a bi-dimensional array of tiles ordered in row major order (see below).

What is a *tile* ? Any object (custom class, struct, component, ...) that implements the very light **ITile** interface of this library (**ITile3D** for the 3D API). This interface requires three properties getters:
- *bool* **IsWalkable** . Must return if the tile can be walk/see throught (for pathfinding/raycasting)
- *int* **X** . Must return the horizontal position of the tile into the grid
- *int* **Y** . Must return the vertical position of the tile into the grid

This API is using a namespace so you have to add a using instruction to the scripts that will need this library:
```cs
using KevinCastejon.GridToolkit;
```

---
#### MajorOrder

When working with two-dimensional arrays there is two ways of storing tiles, first rows then lines or the opposite.  
This is called the **Major Order**. The most common major order used in C languages (and the one used in this library) is the row major order, meaning that the first index of the array represents the row index and the second index represents the column index.

It can be counter intuitive as the row index actually indicates the vertical position of the tile in the grid, and the column index indicates the horizontal position of the tile in the grid.
For more information you can refer to this [Wikipedia article](https://en.wikipedia.org/wiki/Row-_and_column-major_order).

---

## API

---

### Extraction

---
Allows you to extract tiles on a grid.  
Provides shape extraction (rectangles, circles, cones and lines) and neighbors extraction with a lot of parameters.

---
You can extract tiles from shapes.

- **GetTilesInARectangle**
```cs
YourCustomTileType[] tiles = Extraction.GetTilesInARectangle(grid, centerTile, rectangleSize);
```
- **GetTilesInACircle**
```cs
YourCustomTileType[] tiles = Extraction.GetTilesInACircle(grid, centerTile, radius);
```
- **GetTilesInACone**
```cs
YourCustomTileType[] tiles = Extraction.GetTilesInACone(grid, startTile, length, openingAngle, direction);
```
- **GetTilesOnALine**
```cs
YourCustomTileType[] tiles = Extraction.GetTilesOnALine(grid, startTile, length, direction);
```

---
You can extract neighbors of a tile (if existing).

- **GetTileNeighbour**
```cs
YourCustomTileType upperNeighbour = Extraction.GetTileNeighbour(grid, tile, Vector2Int.up);
```
- **GetTileNeighbours**
```cs
YourCustomTileType[] neighbours = Extraction.GetTileNeighbours(grid, tile);
```
- **GetTileOrthogonalsNeighbours**
```cs
YourCustomTileType[] orthogonalNeighbours = Extraction.GetTileOrthogonalsNeighbours(grid, tile);
```
- **GetTileDiagonalsNeighbours**
```cs
YourCustomTileType[] diagonalsNeighbours = Extraction.GetTileDiagonalsNeighbours(grid, tile);
```

---
Each extraction method has a variant to check if a specific tile would be extracted

- **IsTileInARectangle**
```cs
bool isTileInARectangle = Extraction3D.IsTileInARectangle(grid, tile, centerTile, rectangleSize);
```
- **IsTileInACircle**
```cs
bool isTileInACircle = Extraction3D.IsTileInACircle(grid, tile, centerTile, radius);
```
- **IsTileInACone**
```cs
bool isTileInACone = Extraction.IsTileInACone(grid, tile, centerTile, length, openingAngle, direction);
```
- **IsTilesOnALine**
```cs
bool isTilesOnALine = Extraction.IsTilesOnALine(grid, tile, centerTile, length, direction);
```
- **IsTileNeighbor**
```cs
bool isTileRightNeighbor = Extraction.IsTileNeighbor(tile, neighbor, Vector2Int.right);
```
- **IsTileOrthogonalNeighbor**
```cs
bool isTileOrthogonalNeighbor = Extraction.IsTileOrthogonalNeighbor(tile, neighbor);
```
- **IsTileDiagonalNeighbor**
```cs
bool isTileDiagonalNeighbor = Extraction.IsTileDiagonalNeighbor(tile, neighbor);
```
- **IsTileAnyNeighbor**
```cs
bool isTileNeighbor = Extraction.IsTileAnyNeighbor(tile, neighbor);
```

---

### Raycasting

---
Allows you to cast lines of sight and cones of vision on a grid

---
You can get the **line of sight** from a tile (a line that "stops" at the first encountered unwalkable tile).  
Many signatures are available to specify the length and direction of the line.

- **GetLineOfSight**
```cs
YourCustomTileType[] lineOfSight = Raycasting.GetLineOfSight(grid, startTile, destinationTile);
```
---
You can get the **cone of vision** from a tile.  
Many signatures are available to specify the length and direction of the cone.

- **GetConeOfVision**
```cs
YourCustomTileType[] coneOfVision = Raycasting.GetConeOfVision(grid, startTile, openingAngle, destinationTile);
```
---
You can check if a line of sight or a cone of vision is clear (no non-walkable tile encountered)

- **IsLineOfSightClear**
```cs
bool isLineClear = Raycasting.IsLineOfSightClear(grid, startTile, destinationTile);
```
- **IsConeOfVisionClear**
```cs
bool isConeClear = Raycasting.IsConeOfVisionClear(grid, startTile, destinationTile);
```

---

### Pathfinding

---
Allows you to calculate paths between tiles.  
This API offers a method which generates and returns a direction map. A direction map can be seen as a "layer" on top of the user grid that indicates, for each accessible tile, the direction to the next tile, ultimately leading to the target tile.  
A direction map holds all the paths to a target tile from all the accessible tiles on the grid.  
Storing this DirectionMap object allows you to reconstruct paths between tiles without having to recalculate them every time, which can be costly in terms of performance.  

*Note that, obviously, any path calculation is valid as long as the user grid, and walkable states of the tiles, remains unchanged*

---

#### DirectionMap

You can generate a **DirectionMap** object that holds pre-calculated paths data.  
This way of doing pathfinding is useful for some usages (like Tower Defenses and more) because it calculates once all the paths between one tile, called the "**target**", and all the accessible tiles from it.

To generate the **DirectionMap** object, use the **GenerateDirectionMap** method that needs the *grid* and the *target* tile from which to calculate the paths, as parameters.

```cs
DirectionMap directionMap = Pathfinding.GenerateDirectionMap(grid, targetTile);
```

Once the **DirectionMap** object is generated, you can use its several and almost "*cost free*" methods and properties.

---

You can retrieve the tile that has been used as the target to generate this **DirectionMap**.

- **GetTarget**
```cs
YourCustomTileType targetTile = directionMap.GetTarget(grid);
```

You can get all the tiles on the path from a tile to the target.

- **GetPathToTarget**
```cs
YourCustomTileType[] tiles = directionMap.GetPathToTarget(grid, startTile);
```

Or you can get all the tiles on the path from the target to a tile.

- **GetPathFromTarget**
```cs
YourCustomTileType[] tiles = directionMap.GetPathFromTarget(grid, destinationTile);
```

You can know if a tile is accessible from the target tile. This is useful before calling the following **DirectionMap** methods that only takes an accessible tile as parameter.

- **IsTileAccessible**
```cs
bool isTileAccessible = directionMap.IsTileAccessible(grid, tile);
```

You can get the next tile on the path between the target and a tile.

- **GetNextTileFromTile**
```cs
YourCustomTileType nextTile = directionMap.GetNextTileFromTile(grid, tile);
```

You can get the next tile direction on the path between the target and a tile (in 2D grid coordinates). NextTileDirection is an enum representing the eight possible directions.

- **GetNextTileDirectionFromTile**
```cs
NextTileDirection nextTileDirection = directionMap.GetNextTileDirectionFromTile(grid, tile);
```

You can serialize the generated **DirectionMap** to a byte array. Usefull for path baking in edit time.

- **ToByteArray**
```cs
byte[] serializedDirectionMap = directionMap.ToByteArray();
```

You can deserialize a byte array to a **DirectionMap**. Usefull for loading baked paths at runtime.

- **FromByteArray**
```cs
DirectionMap directionMap = DirectionMap.FromByteArray(grid, serializedDirectionMap);
```
