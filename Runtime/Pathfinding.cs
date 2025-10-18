using Codice.Utils;
using PriorityQueueUnityPort;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    /// <summary>
    /// Allows you to calculate paths between tiles.  
    /// This API offers several ways to do pathfinding, depending on your needs.
    /// 
    /// You can generate objects that can be seen as layers of data on top of your grid. Once generated, these objects allows you to get paths with almost no performance cost.  
    /// 
    /// A DirectionPath object holds direction data for all tiles on the path between two tiles.  
    /// A DijkstraPath object holds both direction and distance data for all tiles on the path between two tiles.  
    /// 
    /// A DirectionField object holds direction data between a target tile and all the tiles that are accessible to this target into a specified maximum distance range.  
    /// A DijkstraField object holds both direction and distance data between a target tile and all the tiles that are accessible to this target into a specified maximum distance range.
    /// 
    /// A DirectionGrid object holds direction data between a target tile and all the tiles that are accessible to this target, on the entire grid.  
    /// A DijkstraGrid object holds both direction and distance data between a target tile and all the tiles that are accessible to this target, on the entire grid.
    /// 
    /// A DirectionAtlas object holds DirectionGrid objects for each tile.  
    /// A DijkstraAtlas object holds DijkstraGrid objects for each tile.  
    /// 
    /// *Note that, obviously, any path calculation is valid as long as the user grid, walkable states (and weights for dijkstra objects) of the tiles, remains unchanged*
    /// </summary>
    public class Pathfinding
    {
        private static bool GetTile<T>(T[,] grid, int x, int y, out T tile) where T : ITile
        {
            if (x > -1 && y > -1 && x < GridUtils.GetHorizontalLength(grid) && y < GridUtils.GetVerticalLength(grid))
            {
                tile = GridUtils.GetTile(grid, x, y);
                return true;
            }
            tile = default;
            return false;
        }
        private static bool GetLeftNeighbour<T>(T[,] grid, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(grid, x - 1, y, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetRightNeighbour<T>(T[,] grid, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(grid, x + 1, y, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetBottomNeighbour<T>(T[,] grid, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(grid, x, y - 1, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetTopNeighbour<T>(T[,] grid, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(grid, x, y + 1, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetLeftBottomNeighbour<T>(T[,] grid, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(grid, x - 1, y - 1, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetLeftTopNeighbour<T>(T[,] grid, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(grid, x - 1, y + 1, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetRightBottomNeighbour<T>(T[,] grid, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(grid, x + 1, y - 1, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetRightTopNeighbour<T>(T[,] grid, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(grid, x + 1, y + 1, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static void GetTileOrthographicNeighbours<T>(ref List<T> nodes, T[,] grid, int x, int y) where T : ITile
        {
            T nei;

            bool leftWalkable = GetLeftNeighbour(grid, x, y, out nei);
            if (leftWalkable)
            {
                nodes.Add(nei);
            }
            bool rightWalkable = GetRightNeighbour(grid, x, y, out nei);
            if (rightWalkable)
            {
                nodes.Add(nei);
            }
            bool bottomWalkable = GetBottomNeighbour(grid, x, y, out nei);
            if (bottomWalkable)
            {
                nodes.Add(nei);
            }
            bool topWalkable = GetTopNeighbour(grid, x, y, out nei);
            if (topWalkable)
            {
                nodes.Add(nei);
            }
        }
        private static void GetTileNeighbours<T>(ref List<T> nodes, T[,] grid, int x, int y, DiagonalsPolicy diagonalsPolicy) where T : ITile
        {
            T nei;

            bool leftWalkable = GetLeftNeighbour(grid, x, y, out nei);
            if (leftWalkable)
            {
                nodes.Add(nei);
            }
            bool rightWalkable = GetRightNeighbour(grid, x, y, out nei);
            if (rightWalkable)
            {
                nodes.Add(nei);
            }
            bool bottomWalkable = GetBottomNeighbour(grid, x, y, out nei);
            if (bottomWalkable)
            {
                nodes.Add(nei);
            }
            bool topWalkable = GetTopNeighbour(grid, x, y, out nei);
            if (topWalkable)
            {
                nodes.Add(nei);
            }

            bool leftBottomWalkable = GetLeftBottomNeighbour(grid, x, y, out nei);
            if (leftBottomWalkable && IsDiagonalPolicyCompliant(diagonalsPolicy, leftWalkable, bottomWalkable))
            {
                nodes.Add(nei);
            }
            bool rightBottomWalkable = GetRightBottomNeighbour(grid, x, y, out nei);
            if (rightBottomWalkable && IsDiagonalPolicyCompliant(diagonalsPolicy, rightWalkable, bottomWalkable))
            {
                nodes.Add(nei);
            }
            bool leftTopWalkable = GetLeftTopNeighbour(grid, x, y, out nei);
            if (leftTopWalkable && IsDiagonalPolicyCompliant(diagonalsPolicy, leftWalkable, topWalkable))
            {
                nodes.Add(nei);
            }
            bool rightTopWalkable = GetRightTopNeighbour(grid, x, y, out nei);
            if (rightTopWalkable && IsDiagonalPolicyCompliant(diagonalsPolicy, rightWalkable, topWalkable))
            {
                nodes.Add(nei);
            }
        }
        private static bool IsDiagonalPolicyCompliant(DiagonalsPolicy policy, bool valueA, bool valueB)
        {
            switch (policy)
            {
                case DiagonalsPolicy.NONE:
                    return false;
                case DiagonalsPolicy.DIAGONAL_2FREE:
                    return valueA && valueB;
                case DiagonalsPolicy.DIAGONAL_1FREE:
                    return valueA || valueB;
                case DiagonalsPolicy.ALL_DIAGONALS:
                    return true;
                default:
                    return false;
            }
        }
        private static float FloatHeuristic(Vector2 a, Vector2 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
        /// <summary>
        /// Generates a DirectionPath that holds direction data for each tile on the path from the startTile to the targetTile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the path calculation</param>
        /// <param name="startTile">The start tile for the path calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the path calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the path calculation</param>
        /// <param name="includeStart">Include the start into the path array or not</param>
        /// <param name="includeTarget">Include the target into the path array or not</param>
        /// <returns>An array of tiles representing the path</returns>
        public static DirectionPath GenerateDirectionPath<T>(T[,] grid, T targetTile, T startTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f, bool includeStart = true, bool includeTarget = true) where T : IWeightedTile
        {
            if (targetTile == null || !targetTile.IsWalkable)
            {
                throw new Exception("Do not try to generate a DijkstraField with an unwalkable (or null) tile as the target");
            }
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int gridDimensions = new Vector2Int(width, height);
            Dictionary<int, (TileDirection, float)> accessibleTiles = new();
            int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
            accessibleTiles.Add(targetIndex, (TileDirection.SELF, 0f));
            PriorityQueue<T, float> frontier = new();
            frontier.Enqueue(targetTile, 0f);
            List<T> neighbourgs = new();
            T current;
            int neighborIndex;
            int currentIndex;
            bool isDiagonal;
            float newDistance;
            int visitedCount = 0;
            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();
                if (GridUtils.TileEquals(current, startTile))
                {
                    T t = current;
                    List<int> path = new();
                    if (includeStart)
                    {
                        int flatStartIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, t.X, t.Y);
                        path.Add(flatStartIndex);
                    }
                    while (!GridUtils.TileEquals(t, targetTile))
                    {
                        Vector2Int nextTileDirection = GridUtils.NextNodeDirectionToVector2Int(accessibleTiles[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), t.X, t.Y)].Item1);
                        Vector2Int nextTileCoords = new(t.X + nextTileDirection.x, t.Y + nextTileDirection.y);
                        T nextTile = GridUtils.GetTile(grid, nextTileCoords.x, nextTileCoords.y);
                        int flatIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, nextTileCoords.x, nextTileCoords.y);
                        if (!GridUtils.TileEquals(nextTile, targetTile) || includeTarget)
                        {
                            path.Add(flatIndex);
                        }
                        t = nextTile;
                    }
                    return new DirectionPath(path.ToArray());
                }

                if (diagonalsPolicy != DiagonalsPolicy.NONE)
                {
                    GetTileNeighbours(ref neighbourgs, grid, current.X, current.Y, diagonalsPolicy);
                }
                else
                {
                    GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                }
                foreach (T neiTile in neighbourgs)
                {
                    neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                    currentIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, current.X, current.Y);
                    isDiagonal = current.X != neiTile.X && current.Y != neiTile.Y;
                    newDistance = accessibleTiles[currentIndex].Item2 + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                    if (!accessibleTiles.ContainsKey(neighborIndex) || newDistance < accessibleTiles[neighborIndex].Item2)
                    {
                        visitedCount++;
                        if (!accessibleTiles.ContainsKey(neighborIndex))
                        {
                            accessibleTiles.Add(neighborIndex, (TileDirection.SELF, 0f));
                        }
                        accessibleTiles[neighborIndex] = (GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current), newDistance);
                        frontier.Enqueue(neiTile, newDistance + FloatHeuristic(new(startTile.X, startTile.Y), new(neiTile.X, neiTile.Y)));
                    }
                }
                neighbourgs.Clear();
            }
            return null;
        }
        /// <summary>
        /// Asynchronously generates a DirectionPath that holds direction data for each tile on the path from the startTile to the targetTile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the path calculation</param>
        /// <param name="startTile">The start tile for the path calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the path calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the path calculation</param>
        /// <param name="includeStart">Include the start into the path array or not</param>
        /// <param name="includeTarget">Include the target into the path array or not</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the generation</param>
        /// <returns>An array of tiles representing the path</returns>
        public static Task<DirectionPath> GenerateDirectionPathAsync<T>(T[,] grid, T targetTile, T startTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f, bool includeStart = true, bool includeTarget = true, CancellationToken cancelToken = default) where T : IWeightedTile
        {
            Task<DirectionPath> task = Task.Run(() =>
            {
                if (targetTile == null || !targetTile.IsWalkable)
                {
                    throw new Exception("Do not try to generate a DijkstraField with an unwalkable (or null) tile as the target");
                }
                int width = grid.GetLength(0);
                int height = grid.GetLength(1);
                Vector2Int gridDimensions = new Vector2Int(width, height);
                Dictionary<int, (TileDirection, float)> accessibleTiles = new();
                int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
                accessibleTiles.Add(targetIndex, (TileDirection.SELF, 0f));
                PriorityQueue<T, float> frontier = new();
                frontier.Enqueue(targetTile, 0f);
                List<T> neighbourgs = new();
                T current;
                int neighborIndex;
                int currentIndex;
                bool isDiagonal;
                float newDistance;
                int visitedCount = 0;
                while (frontier.Count > 0)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    current = frontier.Dequeue();
                    if (GridUtils.TileEquals(current, startTile))
                    {
                        T t = current;
                        List<int> path = new();
                        if (includeStart)
                        {
                            int flatStartIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, t.X, t.Y);
                            path.Add(flatStartIndex);
                        }
                        while (!GridUtils.TileEquals(t, targetTile))
                        {
                            Vector2Int nextTileDirection = GridUtils.NextNodeDirectionToVector2Int(accessibleTiles[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), t.X, t.Y)].Item1);
                            Vector2Int nextTileCoords = new(t.X + nextTileDirection.x, t.Y + nextTileDirection.y);
                            T nextTile = GridUtils.GetTile(grid, nextTileCoords.x, nextTileCoords.y);
                            int flatIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, nextTileCoords.x, nextTileCoords.y);
                            if (!GridUtils.TileEquals(nextTile, targetTile) || includeTarget)
                            {
                                path.Add(flatIndex);
                            }
                            t = nextTile;
                        }
                        return new DirectionPath(path.ToArray());
                    }

                    if (diagonalsPolicy != DiagonalsPolicy.NONE)
                    {
                        GetTileNeighbours(ref neighbourgs, grid, current.X, current.Y, diagonalsPolicy);
                    }
                    else
                    {
                        GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                    }
                    foreach (T neiTile in neighbourgs)
                    {
                        neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                        currentIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, current.X, current.Y);
                        isDiagonal = current.X != neiTile.X && current.Y != neiTile.Y;
                        newDistance = accessibleTiles[currentIndex].Item2 + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                        if (!accessibleTiles.ContainsKey(neighborIndex) || newDistance < accessibleTiles[neighborIndex].Item2)
                        {
                            visitedCount++;
                            if (!accessibleTiles.ContainsKey(neighborIndex))
                            {
                                accessibleTiles.Add(neighborIndex, (TileDirection.SELF, 0f));
                            }
                            accessibleTiles[neighborIndex] = (GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current), newDistance);
                            frontier.Enqueue(neiTile, newDistance + FloatHeuristic(new(startTile.X, startTile.Y), new(neiTile.X, neiTile.Y)));
                        }
                    }
                    neighbourgs.Clear();
                }
                return null;
            });
            return task;
        }
        /// <summary>
        /// Generates a DijkstraPath that holds direction and distance data for each tile on the path from the startTile to the targetTile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the path calculation</param>
        /// <param name="startTile">The start tile for the path calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the path calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the path calculation</param>
        /// <param name="includeStart">Include the start into the path array or not</param>
        /// <param name="includeTarget">Include the target into the path array or not</param>
        /// <returns>An array of tiles representing the path</returns>
        public static DijkstraPath GenerateDijkstraPath<T>(T[,] grid, T targetTile, T startTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f, bool includeStart = true, bool includeTarget = true) where T : IWeightedTile
        {
            if (targetTile == null || !targetTile.IsWalkable)
            {
                throw new Exception("Do not try to generate a DijkstraField with an unwalkable (or null) tile as the target");
            }
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int gridDimensions = new Vector2Int(width, height);
            Dictionary<int, (TileDirection, float)> accessibleTiles = new();
            int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
            accessibleTiles.Add(targetIndex, (TileDirection.SELF, 0f));
            PriorityQueue<T, float> frontier = new();
            frontier.Enqueue(targetTile, 0f);
            List<T> neighbourgs = new();
            T current;
            int neighborIndex;
            int currentIndex;
            bool isDiagonal;
            float newDistance;
            int visitedCount = 0;
            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();
                if (GridUtils.TileEquals(current, startTile))
                {
                    T t = current;
                    List<(int, float)> path = new();
                    if (includeStart)
                    {
                        int flatStartIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, t.X, t.Y);
                        path.Add((flatStartIndex, accessibleTiles[flatStartIndex].Item2));
                    }
                    while (!GridUtils.TileEquals(t, targetTile))
                    {
                        Vector2Int nextTileDirection = GridUtils.NextNodeDirectionToVector2Int(accessibleTiles[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), t.X, t.Y)].Item1);
                        Vector2Int nextTileCoords = new(t.X + nextTileDirection.x, t.Y + nextTileDirection.y);
                        T nextTile = GridUtils.GetTile(grid, nextTileCoords.x, nextTileCoords.y);
                        int flatIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, nextTileCoords.x, nextTileCoords.y);
                        if (!GridUtils.TileEquals(nextTile, targetTile) || includeTarget)
                        {
                            path.Add((flatIndex, accessibleTiles[flatIndex].Item2));
                        }
                        t = nextTile;
                    }
                    return new DijkstraPath(path.ToArray());
                }

                if (diagonalsPolicy != DiagonalsPolicy.NONE)
                {
                    GetTileNeighbours(ref neighbourgs, grid, current.X, current.Y, diagonalsPolicy);
                }
                else
                {
                    GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                }
                foreach (T neiTile in neighbourgs)
                {
                    neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                    currentIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, current.X, current.Y);
                    isDiagonal = current.X != neiTile.X && current.Y != neiTile.Y;
                    newDistance = accessibleTiles[currentIndex].Item2 + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                    if (!accessibleTiles.ContainsKey(neighborIndex) || newDistance < accessibleTiles[neighborIndex].Item2)
                    {
                        visitedCount++;
                        if (!accessibleTiles.ContainsKey(neighborIndex))
                        {
                            accessibleTiles.Add(neighborIndex, (TileDirection.SELF, 0f));
                        }
                        accessibleTiles[neighborIndex] = (GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current), newDistance);
                        frontier.Enqueue(neiTile, newDistance + FloatHeuristic(new(startTile.X, startTile.Y), new(neiTile.X, neiTile.Y)));
                    }
                }
                neighbourgs.Clear();
            }
            return null;
        }
        /// <summary>
        /// Asynchronously generates a DijkstraPath that holds direction and distance data for each tile on the path from the startTile to the targetTile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the path calculation</param>
        /// <param name="startTile">The start tile for the path calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the path calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the path calculation</param>
        /// <param name="includeStart">Include the start into the path array or not</param>
        /// <param name="includeTarget">Include the target into the path array or not</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the generation</param>
        /// <returns>An array of tiles representing the path</returns>
        public static Task<DijkstraPath> GenerateDijkstraPathAsync<T>(T[,] grid, T targetTile, T startTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f, bool includeStart = true, bool includeTarget = true, CancellationToken cancelToken = default) where T : IWeightedTile
        {
            Task<DijkstraPath> task = Task.Run(() =>
            {
                if (targetTile == null || !targetTile.IsWalkable)
                {
                    throw new Exception("Do not try to generate a DijkstraField with an unwalkable (or null) tile as the target");
                }
                int width = grid.GetLength(0);
                int height = grid.GetLength(1);
                Vector2Int gridDimensions = new Vector2Int(width, height);
                Dictionary<int, (TileDirection, float)> accessibleTiles = new();
                int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
                accessibleTiles.Add(targetIndex, (TileDirection.SELF, 0f));
                PriorityQueue<T, float> frontier = new();
                frontier.Enqueue(targetTile, 0f);
                List<T> neighbourgs = new();
                T current;
                int neighborIndex;
                int currentIndex;
                bool isDiagonal;
                float newDistance;
                int visitedCount = 0;
                while (frontier.Count > 0)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    current = frontier.Dequeue();
                    if (GridUtils.TileEquals(current, startTile))
                    {
                        T t = current;
                        List<(int, float)> path = new();
                        if (includeStart)
                        {
                            int flatStartIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, t.X, t.Y);
                            path.Add((flatStartIndex, accessibleTiles[flatStartIndex].Item2));
                        }
                        while (!GridUtils.TileEquals(t, targetTile))
                        {
                            Vector2Int nextTileDirection = GridUtils.NextNodeDirectionToVector2Int(accessibleTiles[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), t.X, t.Y)].Item1);
                            Vector2Int nextTileCoords = new(t.X + nextTileDirection.x, t.Y + nextTileDirection.y);
                            T nextTile = GridUtils.GetTile(grid, nextTileCoords.x, nextTileCoords.y);
                            int flatIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, nextTileCoords.x, nextTileCoords.y);
                            if (!GridUtils.TileEquals(nextTile, targetTile) || includeTarget)
                            {
                                path.Add((flatIndex, accessibleTiles[flatIndex].Item2));
                            }
                            t = nextTile;
                        }
                        return new DijkstraPath(path.ToArray());
                    }

                    if (diagonalsPolicy != DiagonalsPolicy.NONE)
                    {
                        GetTileNeighbours(ref neighbourgs, grid, current.X, current.Y, diagonalsPolicy);
                    }
                    else
                    {
                        GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                    }
                    foreach (T neiTile in neighbourgs)
                    {
                        neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                        currentIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, current.X, current.Y);
                        isDiagonal = current.X != neiTile.X && current.Y != neiTile.Y;
                        newDistance = accessibleTiles[currentIndex].Item2 + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                        if (!accessibleTiles.ContainsKey(neighborIndex) || newDistance < accessibleTiles[neighborIndex].Item2)
                        {
                            visitedCount++;
                            if (!accessibleTiles.ContainsKey(neighborIndex))
                            {
                                accessibleTiles.Add(neighborIndex, (TileDirection.SELF, 0f));
                            }
                            accessibleTiles[neighborIndex] = (GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current), newDistance);
                            frontier.Enqueue(neiTile, newDistance + FloatHeuristic(new(startTile.X, startTile.Y), new(neiTile.X, neiTile.Y)));
                        }
                    }
                    neighbourgs.Clear();
                }
                return null;
            });
            return task;
        }
        /// <summary>
        /// Generates a DirectionField that holds the direction data between a target tile and all the tiles that are accessible to this target into the specified maximum distance range.
        /// This allows you to run them more often because of the early exit due to the maximum distance parameter(note that more higher is the distance, more costly is the generation).  
        /// Once generated, these objects offers you a way to get the accessible tiles within a range, and paths to them, with almost no performance cost(ie: a strategy game where you want to check the tiles in range of your character)
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="maxDistance">The maximum distance used for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <returns>A DirectionField object</returns>
        public static DirectionField GenerateDirectionField<T>(T[,] grid, T targetTile, int maxDistance, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE) where T : ITile
        {
            if (maxDistance < 1)
            {
                throw new Exception("The MaxDistance parameter has to be superior to 0.");
            }

            if (targetTile == null || !targetTile.IsWalkable)
            {
                throw new Exception("Do not try to generate a DirectionField with an unwalkable (or null) tile as the target");
            }
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int gridDimensions = new Vector2Int(width, height);
            Dictionary<int, TileDirection> accessibleTiles = new();
            Dictionary<int, int> accessibleTilesDistances = new();
            int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
            accessibleTiles.Add(targetIndex, TileDirection.SELF);
            accessibleTilesDistances.Add(targetIndex, 0);
            Queue<T> frontier = new Queue<T>();
            frontier.Enqueue(targetTile);
            List<T> neighbourgs = new();
            T current = default;
            int neighborIndex = -1;
            int currentIndex = -1;
            int visitedCount = 0;
            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();
                if (diagonalsPolicy != DiagonalsPolicy.NONE)
                {
                    GetTileNeighbours(ref neighbourgs, grid, current.X, current.Y, diagonalsPolicy);
                }
                else
                {
                    GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                }
                foreach (T neiTile in neighbourgs)
                {
                    neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                    currentIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, current.X, current.Y);
                    int newDistance = accessibleTilesDistances[currentIndex] + 1;
                    if (!accessibleTiles.ContainsKey(neighborIndex) && newDistance <= maxDistance)
                    {
                        visitedCount++;
                        accessibleTiles.Add(neighborIndex, GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current));
                        accessibleTilesDistances.Add(neighborIndex, newDistance);
                        frontier.Enqueue(neiTile);
                    }
                }
                neighbourgs.Clear();
            }
            return new DirectionField(targetIndex, maxDistance, accessibleTiles);
        }
        /// <summary>
        /// Asynchronously generates a DirectionField that holds the direction data between a target tile and all the tiles that are accessible to this target into the specified maximum distance range.
        /// This allows you to run them more often because of the early exit due to the maximum distance parameter(note that more higher is the distance, more costly is the generation).  
        /// Once generated, these objects offers you a way to get the accessible tiles within a range, and paths to them, with almost no performance cost(ie: a strategy game where you want to check the tiles in range of your character)
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="maxDistance">The maximum distance used for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the generation</param>
        /// <returns>A DirectionField object</returns>
        public static Task<DirectionField> GenerateDirectionFieldAsync<T>(T[,] grid, T targetTile, int maxDistance, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, CancellationToken cancelToken = default) where T : ITile
        {
            if (maxDistance < 1)
            {
                throw new Exception("The MaxDistance parameter has to be superior to 0.");
            }
            Task<DirectionField> task = Task.Run(() =>
            {
                if (targetTile == null || !targetTile.IsWalkable)
                {
                    throw new Exception("Do not try to generate a DirectionField with an unwalkable (or null) tile as the target");
                }
                int width = grid.GetLength(0);
                int height = grid.GetLength(1);
                Vector2Int gridDimensions = new Vector2Int(width, height);
                Dictionary<int, TileDirection> accessibleTiles = new();
                Dictionary<int, int> accessibleTilesDistances = new();
                int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
                accessibleTiles.Add(targetIndex, TileDirection.SELF);
                accessibleTilesDistances.Add(targetIndex, 0);
                Queue<T> frontier = new Queue<T>();
                frontier.Enqueue(targetTile);
                List<T> neighbourgs = new();
                T current = default;
                int neighborIndex = -1;
                int currentIndex = -1;
                while (frontier.Count > 0)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    current = frontier.Dequeue();
                    if (diagonalsPolicy != DiagonalsPolicy.NONE)
                    {
                        GetTileNeighbours(ref neighbourgs, grid, current.X, current.Y, diagonalsPolicy);
                    }
                    else
                    {
                        GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                    }
                    foreach (T neiTile in neighbourgs)
                    {
                        neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                        currentIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, current.X, current.Y);
                        int newDistance = accessibleTilesDistances[currentIndex] + 1;
                        if (!accessibleTiles.ContainsKey(neighborIndex) && newDistance <= maxDistance)
                        {
                            accessibleTiles.Add(neighborIndex, GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current));
                            accessibleTilesDistances.Add(neighborIndex, newDistance);
                            frontier.Enqueue(neiTile);
                        }
                    }
                    neighbourgs.Clear();
                }
                return new DirectionField(targetIndex, maxDistance, accessibleTiles);
            });
            return task;
        }
        /// <summary>
        /// Generates a DijkstraField that holds the direction data between a target tile and all the tiles that are accessible to this target into the specified maximum distance range.
        /// This allows you to run them more often because of the early exit due to the maximum distance parameter(note that more higher is the distance, more costly is the generation).  
        /// Once generated, this object offers you a way to get the accessible tiles within a range, and paths to them, with almost no performance cost(ie: a strategy game where you want to check the tiles in range of your character)
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="maxDistance">The maximum distance used for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the paths calculation</param>
        /// <returns>A DirectionGrid object</returns>
        public static DijkstraField GenerateDijkstraField<T>(T[,] grid, T targetTile, float maxDistance, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f) where T : IWeightedTile
        {
            if (maxDistance < 1f)
            {
                throw new Exception("The MaxDistance parameter has to be superior or equal to 1f.");
            }
            if (targetTile == null || !targetTile.IsWalkable)
            {
                throw new Exception("Do not try to generate a DijkstraField with an unwalkable (or null) tile as the target");
            }
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int gridDimensions = new Vector2Int(width, height);
            Dictionary<int, (TileDirection, float)> accessibleTiles = new();
            int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
            accessibleTiles.Add(targetIndex, (TileDirection.SELF, 0f));
            PriorityQueue<T, float> frontier = new();
            frontier.Enqueue(targetTile, 0f);
            List<T> neighbourgs = new();
            T current;
            int neighborIndex;
            int currentIndex;
            bool isDiagonal;
            float newDistance;
            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();
                if (diagonalsPolicy != DiagonalsPolicy.NONE)
                {
                    GetTileNeighbours(ref neighbourgs, grid, current.X, current.Y, diagonalsPolicy);
                }
                else
                {
                    GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                }
                foreach (T neiTile in neighbourgs)
                {
                    neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                    currentIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, current.X, current.Y);
                    isDiagonal = current.X != neiTile.X && current.Y != neiTile.Y;
                    newDistance = accessibleTiles[currentIndex].Item2 + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                    if (newDistance <= maxDistance && (!accessibleTiles.ContainsKey(neighborIndex) || newDistance < accessibleTiles[neighborIndex].Item2))
                    {
                        if (!accessibleTiles.ContainsKey(neighborIndex))
                        {
                            accessibleTiles.Add(neighborIndex, (TileDirection.SELF, 0f));
                        }
                        accessibleTiles[neighborIndex] = (GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current), newDistance);
                        frontier.Enqueue(neiTile, newDistance);
                    }
                }
                neighbourgs.Clear();
            }
            return new DijkstraField(targetIndex, maxDistance, accessibleTiles);
        }
        /// <summary>
        /// Asynchronously generates a DijkstraField that holds the direction data between a target tile and all the tiles that are accessible to this target into the specified maximum distance range.
        /// This allows you to run them more often than DijkstraGrid because of the early exit due to the maximum distance parameter(note that more higher is the distance, more costly is the generation).  
        /// Once generated, this object offers you a way to get the accessible tiles within a range, and paths to them, with almost no performance cost(ie: a strategy game where you want to check the tiles in range of your character)
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="maxDistance">The maximum distance used for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the paths calculation</param>
        /// <param name="progress">An optional IProgress object to get the generation progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the generation</param>
        /// <returns>A DirectionGrid object</returns>
        public static Task<DijkstraField> GenerateDijkstraFieldAsync<T>(T[,] grid, T targetTile, float maxDistance, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f, CancellationToken cancelToken = default) where T : IWeightedTile
        {
            if (maxDistance < 1f)
            {
                throw new Exception("The MaxDistance parameter has to be superior or equal to 1f.");
            }
            Task<DijkstraField> task = Task.Run(() =>
            {
                if (targetTile == null || !targetTile.IsWalkable)
                {
                    throw new Exception("Do not try to generate a DijkstraField with an unwalkable (or null) tile as the target");
                }
                int width = grid.GetLength(0);
                int height = grid.GetLength(1);
                Vector2Int gridDimensions = new Vector2Int(width, height);
                Dictionary<int, (TileDirection, float)> accessibleTiles = new();
                int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
                accessibleTiles.Add(targetIndex, (TileDirection.SELF, 0f));
                PriorityQueue<T, float> frontier = new();
                frontier.Enqueue(targetTile, 0f);
                List<T> neighbourgs = new();
                T current;
                int neighborIndex;
                int currentIndex;
                bool isDiagonal;
                float newDistance;
                while (frontier.Count > 0)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    current = frontier.Dequeue();
                    if (diagonalsPolicy != DiagonalsPolicy.NONE)
                    {
                        GetTileNeighbours(ref neighbourgs, grid, current.X, current.Y, diagonalsPolicy);
                    }
                    else
                    {
                        GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                    }
                    foreach (T neiTile in neighbourgs)
                    {
                        neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                        currentIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, current.X, current.Y);
                        isDiagonal = current.X != neiTile.X && current.Y != neiTile.Y;
                        newDistance = accessibleTiles[currentIndex].Item2 + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                        if (newDistance <= maxDistance && (!accessibleTiles.ContainsKey(neighborIndex) || newDistance < accessibleTiles[neighborIndex].Item2))
                        {
                            if (!accessibleTiles.ContainsKey(neighborIndex))
                            {
                                accessibleTiles.Add(neighborIndex, (TileDirection.SELF, 0f));
                            }
                            accessibleTiles[neighborIndex] = (GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current), newDistance);
                            frontier.Enqueue(neiTile, newDistance);
                        }
                    }
                    neighbourgs.Clear();
                }
                return new DijkstraField(targetIndex, maxDistance, accessibleTiles);
            });
            return task;
        }
        /// <summary>
        /// Generates a DirectionGrid that holds direction data between a target tile and all the tiles that are accessible to this target.  
        /// Once generated, this object can contain all the paths you need (ie: a tower defense game with a village core where all enemies run to) and then use the paths with almost no performance cost.  
        /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <returns>A DirectionGrid object</returns>
        public static DirectionGrid GenerateDirectionGrid<T>(T[,] grid, T targetTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE) where T : ITile
        {
            if (targetTile == null || !targetTile.IsWalkable)
            {
                throw new Exception("Do not try to generate a DirectionGrid with an unwalkable (or null) tile as the target");
            }
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int gridDimensions = new Vector2Int(width, height);
            int totalSize = width * height;
            TileDirection[] directionGrid = new TileDirection[totalSize];
            bool[] visited = new bool[totalSize];
            int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
            visited[targetIndex] = true;
            directionGrid[targetIndex] = TileDirection.SELF;
            Queue<T> frontier = new Queue<T>();
            frontier.Enqueue(targetTile);
            List<T> neighbourgs = new();
            T current = default;
            int neighborIndex = -1;
            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();
                if (diagonalsPolicy != DiagonalsPolicy.NONE)
                {
                    GetTileNeighbours(ref neighbourgs, grid, current.X, current.Y, diagonalsPolicy);
                }
                else
                {
                    GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                }
                foreach (T neiTile in neighbourgs)
                {
                    neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                    if (!visited[neighborIndex])
                    {
                        visited[neighborIndex] = true;
                        directionGrid[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                        frontier.Enqueue(neiTile);
                    }
                }
                neighbourgs.Clear();
            }
            return new DirectionGrid(directionGrid, targetIndex);
        }
        /// <summary>
        /// Asynchronously generates a DirectionGrid that holds direction data between a target tile and all the tiles that are accessible to this target.  
        /// Once generated, this object can contain all the paths you need (ie: a tower defense game with a village core where all enemies run to) and then use the paths with almost no performance cost.  
        /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <param name="progress">An optional IProgress object to get the generation progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the generation</param>
        /// <returns>A DirectionGrid object</returns>
        public static Task<DirectionGrid> GenerateDirectionGridAsync<T>(T[,] grid, T targetTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : ITile
        {
            Task<DirectionGrid> task = Task.Run(() =>
            {
                if (targetTile == null || !targetTile.IsWalkable)
                {
                    throw new Exception("Do not try to generate a DirectionGrid with an unwalkable (or null) tile as the target");
                }
                int width = grid.GetLength(0);
                int height = grid.GetLength(1);
                Vector2Int gridDimensions = new Vector2Int(width, height);
                int totalSize = width * height;
                TileDirection[] directionGrid = new TileDirection[totalSize];
                bool[] visited = new bool[totalSize];
                int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
                visited[targetIndex] = true;
                directionGrid[targetIndex] = TileDirection.SELF;
                Queue<T> frontier = new Queue<T>();
                frontier.Enqueue(targetTile);
                List<T> neighbourgs = new();
                T current = default;
                int neighborIndex = -1;
                int visitedCount = 0;
                while (frontier.Count > 0)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress?.Report((float)visitedCount / totalSize);
                    current = frontier.Dequeue();
                    if (diagonalsPolicy != DiagonalsPolicy.NONE)
                    {
                        GetTileNeighbours(ref neighbourgs, grid, current.X, current.Y, diagonalsPolicy);
                    }
                    else
                    {
                        GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                    }
                    foreach (T neiTile in neighbourgs)
                    {
                        neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                        if (!visited[neighborIndex])
                        {
                            visitedCount++;
                            visited[neighborIndex] = true;
                            directionGrid[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                            frontier.Enqueue(neiTile);
                        }
                    }
                    neighbourgs.Clear();
                }
                return new DirectionGrid(directionGrid, targetIndex);
            });
            return task;
        }
        /// <summary>
        /// Generates a DijkstraGrid that holds both direction and distance data between a target tile and all the tiles that are accessible to this target.  
        /// Once generated, this object can contain all the paths and distance data that you need (ie: a tower defense game with a village core where all enemies run to, or a strategy game in which you would display the distance cost of the movement by hovering tiles with the cursor) and then use the paths with almost no performance cost.  
        /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the paths calculation</param>
        /// <returns>A DijkstraGrid object</returns>
        public static DijkstraGrid GenerateDijkstraGrid<T>(T[,] grid, T targetTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f) where T : IWeightedTile
        {
            if (targetTile == null || !targetTile.IsWalkable)
            {
                throw new Exception("Do not try to generate a DijkstraGrid with an unwalkable (or null) tile as the target");
            }
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int gridDimensions = new Vector2Int(width, height);
            int totalSize = width * height;
            TileDirection[] directionGrid = new TileDirection[totalSize];
            float[] distanceGrid = new float[totalSize];
            bool[] visited = new bool[totalSize];
            int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
            visited[targetIndex] = true;
            directionGrid[targetIndex] = TileDirection.SELF;
            distanceGrid[targetIndex] = 0f;
            PriorityQueue<T, float> frontier = new();
            frontier.Enqueue(targetTile, 0f);
            List<T> neighbourgs = new();
            T current;
            int neighborIndex;
            int currentIndex;
            bool isDiagonal;
            float newDistance;
            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();
                if (diagonalsPolicy != DiagonalsPolicy.NONE)
                {
                    GetTileNeighbours(ref neighbourgs, grid, current.X, current.Y, diagonalsPolicy);
                }
                else
                {
                    GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                }
                foreach (T neiTile in neighbourgs)
                {
                    neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                    currentIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, current.X, current.Y);
                    isDiagonal = current.X != neiTile.X && current.Y != neiTile.Y;
                    newDistance = distanceGrid[currentIndex] + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                    if (!visited[neighborIndex] || newDistance < distanceGrid[neighborIndex])
                    {
                        visited[neighborIndex] = true;
                        directionGrid[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                        distanceGrid[neighborIndex] = newDistance;
                        frontier.Enqueue(neiTile, newDistance);
                    }
                }
                neighbourgs.Clear();
            }
            return new DijkstraGrid(directionGrid, distanceGrid, targetIndex);
        }
        /// <summary>
        /// Asynchronously generates a DijkstraGrid that holds both direction and distance data between a target tile and all the tiles that are accessible to this target.  
        /// Once generated, this object can contain all the paths and distance data that you need (ie: a tower defense game with a village core where all enemies run to, or a strategy game in which you would display the distance cost of the movement by hovering tiles with the cursor) and then use the paths with almost no performance cost.  
        /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the paths calculation</param>
        /// <param name="progress">An optional IProgress object to get the generation progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the generation</param>
        /// <returns>A DirectionGrid object</returns>
        public static Task<DijkstraGrid> GenerateDijkstraGridAsync<T>(T[,] grid, T targetTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : IWeightedTile
        {
            Task<DijkstraGrid> task = Task.Run(() =>
            {
                if (targetTile == null || !targetTile.IsWalkable)
                {
                    throw new Exception("Do not try to generate a DijkstraGrid with an unwalkable (or null) tile as the target");
                }
                int width = grid.GetLength(0);
                int height = grid.GetLength(1);
                Vector2Int gridDimensions = new Vector2Int(width, height);
                int totalSize = width * height;
                TileDirection[] directionGrid = new TileDirection[totalSize];
                float[] distanceGrid = new float[totalSize];
                bool[] visited = new bool[totalSize];
                int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
                visited[targetIndex] = true;
                directionGrid[targetIndex] = TileDirection.SELF;
                distanceGrid[targetIndex] = 0f;
                PriorityQueue<T, float> frontier = new();
                frontier.Enqueue(targetTile, 0f);
                List<T> neighbourgs = new();
                T current;
                int neighborIndex;
                int currentIndex;
                bool isDiagonal;
                float newDistance;
                int visitedCount = 0;
                while (frontier.Count > 0)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress?.Report((float)visitedCount / totalSize);
                    current = frontier.Dequeue();
                    if (diagonalsPolicy != DiagonalsPolicy.NONE)
                    {
                        GetTileNeighbours(ref neighbourgs, grid, current.X, current.Y, diagonalsPolicy);
                    }
                    else
                    {
                        GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                    }
                    foreach (T neiTile in neighbourgs)
                    {
                        neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                        currentIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, current.X, current.Y);
                        isDiagonal = current.X != neiTile.X && current.Y != neiTile.Y;
                        newDistance = distanceGrid[currentIndex] + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                        if (!visited[neighborIndex] || newDistance < distanceGrid[neighborIndex])
                        {
                            visitedCount++;
                            visited[neighborIndex] = true;
                            directionGrid[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                            distanceGrid[neighborIndex] = newDistance;
                            frontier.Enqueue(neiTile, newDistance);
                        }
                    }
                    neighbourgs.Clear();
                }
                return new DijkstraGrid(directionGrid, distanceGrid, targetIndex);
            });
            return task;
        }
        /// <summary>
        /// Asynchronously generates a DirectionAtlas that holds DirectionGrid objects for each tile.  
        /// Once generated, this object contains all the paths between any tiles on the grid, with almost no performance cost.
        /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
        /// Be carefull as the memory usage can be huge depending on the grid size.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <returns>A DirectionGrid object</returns>
        public static DirectionAtlas GenerateDirectionAtlas<T>(T[,] grid, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE) where T : ITile
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int gridDimensions = new Vector2Int(width, height);
            int totalSize = width * height;
            DirectionGrid[] directionGrid = new DirectionGrid[totalSize];
            for (int i = 0; i < directionGrid.Length; i++)
            {
                Vector2Int coords = GridUtils.GetCoordinatesFromFlatIndex(gridDimensions, i);
                T tile = GridUtils.GetTile(grid, coords.x, coords.y);
                if (tile.IsWalkable)
                {
                    directionGrid[i] = GenerateDirectionGrid(grid, tile, diagonalsPolicy);
                }
            }
            return new DirectionAtlas(directionGrid);
        }
        /// <summary>
        /// Asynchronously generates a DirectionAtlas that holds DirectionGrid objects for each tile.  
        /// Once generated, this object contains all the paths between any tiles on the grid, with almost no performance cost.
        /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
        /// Be carefull as the memory usage can be huge depending on the grid size.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <param name="progress">An optional IProgress object to get the generation progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the generation</param>
        /// <returns>A DirectionGrid object</returns>
        public static Task<DirectionAtlas> GenerateDirectionAtlasAsync<T>(T[,] grid, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : ITile
        {
            Task<DirectionAtlas> task = Task.Run(() =>
            {
                int width = grid.GetLength(0);
                int height = grid.GetLength(1);
                Vector2Int gridDimensions = new Vector2Int(width, height);
                int totalSize = width * height;
                DirectionGrid[] directionGrid = new DirectionGrid[totalSize];
                for (int i = 0; i < directionGrid.Length; i++)
                {
                    Vector2Int coords = GridUtils.GetCoordinatesFromFlatIndex(gridDimensions, i);
                    T tile = GridUtils.GetTile(grid, coords.x, coords.y);
                    if (tile.IsWalkable)
                    {
                        directionGrid[i] = GenerateDirectionGrid(grid, tile, diagonalsPolicy);
                    }
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress?.Report((float)i / directionGrid.Length);
                }
                return new DirectionAtlas(directionGrid);
            });
            return task;
        }
        /// <summary>
        /// Asynchronously generates a DijkstraAtlas that holds DijkstraGrid objects for each tile.  
        /// Once generated, this object contains all the paths and distances between any tiles on the grid, with almost no performance cost.
        /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
        /// Be carefull as the memory usage can be huge depending on the grid size.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the paths calculation</param>
        /// <param name="progress">An optional IProgress object to get the generation progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the generation</param>
        /// <returns>A DirectionGrid object</returns>
        public static DijkstraAtlas GenerateDijkstraAtlas<T>(T[,] grid, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f) where T : IWeightedTile
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int gridDimensions = new Vector2Int(width, height);
            int totalSize = width * height;
            DijkstraGrid[] dijkstraGrid = new DijkstraGrid[totalSize];
            for (int i = 0; i < dijkstraGrid.Length; i++)
            {
                Vector2Int coords = GridUtils.GetCoordinatesFromFlatIndex(gridDimensions, i);
                T tile = GridUtils.GetTile(grid, coords.x, coords.y);
                if (tile.IsWalkable)
                {
                    dijkstraGrid[i] = GenerateDijkstraGrid(grid, tile, diagonalsPolicy);
                }
            }
            return new DijkstraAtlas(dijkstraGrid);
        }
        /// Asynchronously generates a DijkstraAtlas that holds DijkstraGrid objects for each tile.  
        /// Once generated, this object contains all the paths and distances between any tiles on the grid, with almost no performance cost.
        /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
        /// Be carefull as the memory usage can be huge depending on the grid size.
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the paths calculation</param>
        /// <param name="progress">An optional IProgress object to get the generation progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the generation</param>
        /// <returns>A DirectionGrid object</returns>
        public static Task<DijkstraAtlas> GenerateDijkstraAtlasAsync<T>(T[,] grid, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : IWeightedTile
        {
            Task<DijkstraAtlas> task = Task.Run(() =>
            {
                int width = grid.GetLength(0);
                int height = grid.GetLength(1);
                Vector2Int gridDimensions = new Vector2Int(width, height);
                int totalSize = width * height;
                DijkstraGrid[] dijkstraGrid = new DijkstraGrid[totalSize];
                for (int i = 0; i < dijkstraGrid.Length; i++)
                {
                    Vector2Int coords = GridUtils.GetCoordinatesFromFlatIndex(gridDimensions, i);
                    T tile = GridUtils.GetTile(grid, coords.x, coords.y);
                    if (tile.IsWalkable)
                    {
                        dijkstraGrid[i] = GenerateDijkstraGrid(grid, tile, diagonalsPolicy);
                    }
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress?.Report((float)i / dijkstraGrid.Length);
                }
                return new DijkstraAtlas(dijkstraGrid);
            });
            return task;
        }
    }
}
