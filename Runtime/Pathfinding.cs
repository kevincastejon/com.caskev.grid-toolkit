using Codice.Utils;
using PriorityQueueUnityPort;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;
/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    /// <summary>
    /// Allows you to calculate paths between tiles.  
    /// This API offers several ways to do pathfinding, depending on your needs.
    /// 
    /// You can generate objects that can be seen as layers of data on top of your grid.  
    /// A DirectionMap will hold all the pre-calculated direction data between a target tile and all the tiles that are accessible to this target.  
    /// A Dijkstra map will hold both direction and distance data.  
    /// Once generated, these objects can contain all the paths you need (ie: a tower defense game with a village core where all enemies have to run to) and then use the paths with almost no performance cost.  
    /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
    /// 
    /// These two objects covers the entire grid, but you can also generate a DirectionField or a DijkstraField that will hold the same kind of data but only for tiles that are within a specified distance from the target tile.  
    /// This allows you to run them more often because of the early exit due to the maximum distance parameter (note that more higher is the distance, more costly is the generation).  
    /// Once generated, these objects offers you a way to get the accessible tiles within a range, and paths to them, with almost no performance cost (ie: a strategy game where you want to check the tiles in range of your character)
    /// 
    /// If you only need a single path between two specific tiles, you can also generate that unique path. But you should keep the other options in mind as it can be way more effective to generate all paths at once rather than generating a unique path again and again.
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
        /// Generates a DirectionMap that holds direction data between a target tile and all the tiles that are accessible to this target.  
        /// Once generated, this object can contain all the paths you need (ie: a tower defense game with a village core where all enemies run to) and then use the paths with almost no performance cost.  
        /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <returns>A DirectionMap object</returns>
        public static DirectionMap GenerateDirectionMap<T>(T[,] grid, T targetTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE) where T : ITile
        {
            if (targetTile == null || !targetTile.IsWalkable)
            {
                throw new Exception("Do not try to generate a DirectionMap with an unwalkable (or null) tile as the target");
            }
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int gridDimensions = new Vector2Int(width, height);
            int totalSize = width * height;
            NextTileDirection[] directionMap = new NextTileDirection[totalSize];
            bool[] visited = new bool[totalSize];
            int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
            visited[targetIndex] = true;
            directionMap[targetIndex] = NextTileDirection.SELF;
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
                        directionMap[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                        frontier.Enqueue(neiTile);
                    }
                }
                neighbourgs.Clear();
            }
            return new DirectionMap(directionMap, targetIndex);
        }
        /// <summary>
        /// Asynchronously generates a DirectionMap that holds direction data between a target tile and all the tiles that are accessible to this target.  
        /// Once generated, this object can contain all the paths you need (ie: a tower defense game with a village core where all enemies run to) and then use the paths with almost no performance cost.  
        /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <param name="progress">An optional IProgress object to get the generation progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the generation</param>
        /// <returns>A DirectionMap object</returns>
        public static Task<DirectionMap> GenerateDirectionMapAsync<T>(T[,] grid, T targetTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : ITile
        {
            Task<DirectionMap> task = Task.Run(() =>
            {
                if (targetTile == null || !targetTile.IsWalkable)
                {
                    throw new Exception("Do not try to generate a DirectionMap with an unwalkable (or null) tile as the target");
                }
                int width = grid.GetLength(0);
                int height = grid.GetLength(1);
                Vector2Int gridDimensions = new Vector2Int(width, height);
                int totalSize = width * height;
                NextTileDirection[] directionMap = new NextTileDirection[totalSize];
                bool[] visited = new bool[totalSize];
                int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
                visited[targetIndex] = true;
                directionMap[targetIndex] = NextTileDirection.SELF;
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
                            directionMap[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                            frontier.Enqueue(neiTile);
                        }
                    }
                    neighbourgs.Clear();
                }
                return new DirectionMap(directionMap, targetIndex);
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
            int totalSize = width * height;
            NextTileDirection[] directionMap = new NextTileDirection[totalSize];
            int[] distanceMap = new int[totalSize];
            bool[] visited = new bool[totalSize];
            List<int> accessibleTilesFlatIndexes = new();
            int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
            visited[targetIndex] = true;
            directionMap[targetIndex] = NextTileDirection.SELF;
            distanceMap[targetIndex] = 0;
            accessibleTilesFlatIndexes.Add(targetIndex);
            Queue<T> frontier = new Queue<T>();
            frontier.Enqueue(targetTile);
            List<T> neighbourgs = new();
            T current = default;
            int neighborIndex = -1;
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
                    int newDistance = distanceMap[GridUtils.GetFlatIndexFromCoordinates(gridDimensions, current.X, current.Y)] + 1;
                    if (!visited[neighborIndex] && newDistance <= maxDistance)
                    {
                        visitedCount++;
                        accessibleTilesFlatIndexes.Add(neighborIndex);
                        visited[neighborIndex] = true;
                        directionMap[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                        distanceMap[neighborIndex] = newDistance;
                        frontier.Enqueue(neiTile);
                    }
                }
                neighbourgs.Clear();
            }
            return new DirectionField(maxDistance, accessibleTilesFlatIndexes.ToArray(), directionMap, targetIndex);
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
        /// <param name="progress">An optional IProgress object to get the generation progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the generation</param>
        /// <returns>A DirectionField object</returns>
        public static Task<DirectionField> GenerateDirectionFieldAsync<T>(T[,] grid, T targetTile, int maxDistance, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : ITile
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
                int totalSize = width * height;
                NextTileDirection[] directionMap = new NextTileDirection[totalSize];
                int[] distanceMap = new int[totalSize];
                bool[] visited = new bool[totalSize];
                List<int> accessibleTilesFlatIndexes = new();
                int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
                visited[targetIndex] = true;
                directionMap[targetIndex] = NextTileDirection.SELF;
                distanceMap[targetIndex] = 0;
                accessibleTilesFlatIndexes.Add(targetIndex);
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
                        int newDistance = distanceMap[GridUtils.GetFlatIndexFromCoordinates(gridDimensions, current.X, current.Y)] + 1;
                        if (!visited[neighborIndex] && newDistance <= maxDistance)
                        {
                            visitedCount++;
                            accessibleTilesFlatIndexes.Add(neighborIndex);
                            visited[neighborIndex] = true;
                            directionMap[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                            distanceMap[neighborIndex] = newDistance;
                            frontier.Enqueue(neiTile);
                        }
                    }
                    neighbourgs.Clear();
                }
                return new DirectionField(maxDistance, accessibleTilesFlatIndexes.ToArray(), directionMap, targetIndex);
            });
            return task;
        }
        /// <summary>
        /// Generates a DijkstraMap that holds both direction and distance data between a target tile and all the tiles that are accessible to this target.  
        /// Once generated, this object can contain all the paths and distance data that you need (ie: a tower defense game with a village core where all enemies run to, or a strategy game in which you would display the distance cost of the movement by hovering tiles with the cursor) and then use the paths with almost no performance cost.  
        /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the paths calculation</param>
        /// <returns>A DijkstraMap object</returns>
        public static DijkstraMap GenerateDijkstraMap<T>(T[,] grid, T targetTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f) where T : IWeightedTile
        {
            if (targetTile == null || !targetTile.IsWalkable)
            {
                throw new Exception("Do not try to generate a DijkstraMap with an unwalkable (or null) tile as the target");
            }
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int gridDimensions = new Vector2Int(width, height);
            int totalSize = width * height;
            NextTileDirection[] directionMap = new NextTileDirection[totalSize];
            float[] distanceMap = new float[totalSize];
            bool[] visited = new bool[totalSize];
            int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
            visited[targetIndex] = true;
            directionMap[targetIndex] = NextTileDirection.SELF;
            distanceMap[targetIndex] = 0f;
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
                    newDistance = distanceMap[currentIndex] + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                    if (!visited[neighborIndex] || newDistance < distanceMap[neighborIndex])
                    {
                        visited[neighborIndex] = true;
                        directionMap[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                        distanceMap[neighborIndex] = newDistance;
                        frontier.Enqueue(neiTile, newDistance);
                    }
                }
                neighbourgs.Clear();
            }
            return new DijkstraMap(directionMap, distanceMap, targetIndex);
        }
        /// <summary>
        /// Asynchronously generates a DijkstraMap that holds both direction and distance data between a target tile and all the tiles that are accessible to this target.  
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
        /// <returns>A DirectionMap object</returns>
        public static Task<DijkstraMap> GenerateDijkstraMapAsync<T>(T[,] grid, T targetTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : IWeightedTile
        {
            Task<DijkstraMap> task = Task.Run(() =>
            {
                if (targetTile == null || !targetTile.IsWalkable)
                {
                    throw new Exception("Do not try to generate a DijkstraMap with an unwalkable (or null) tile as the target");
                }
                int width = grid.GetLength(0);
                int height = grid.GetLength(1);
                Vector2Int gridDimensions = new Vector2Int(width, height);
                int totalSize = width * height;
                NextTileDirection[] directionMap = new NextTileDirection[totalSize];
                float[] distanceMap = new float[totalSize];
                bool[] visited = new bool[totalSize];
                int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
                visited[targetIndex] = true;
                directionMap[targetIndex] = NextTileDirection.SELF;
                distanceMap[targetIndex] = 0f;
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
                        newDistance = distanceMap[currentIndex] + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                        if (!visited[neighborIndex] || newDistance < distanceMap[neighborIndex])
                        {
                            visitedCount++;
                            visited[neighborIndex] = true;
                            directionMap[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                            distanceMap[neighborIndex] = newDistance;
                            frontier.Enqueue(neiTile, newDistance);
                        }
                    }
                    neighbourgs.Clear();
                }
                return new DijkstraMap(directionMap, distanceMap, targetIndex);
            });
            return task;
        }
        /// <summary>
        /// Generates a DijkstraField holds the direction data between a target tile and all the tiles that are accessible to this target into the specified maximum distance range.
        /// This allows you to run them more often because of the early exit due to the maximum distance parameter(note that more higher is the distance, more costly is the generation).  
        /// Once generated, this object offers you a way to get the accessible tiles within a range, and paths to them, with almost no performance cost(ie: a strategy game where you want to check the tiles in range of your character)
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="maxDistance">The maximum distance used for the paths calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the paths calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the paths calculation</param>
        /// <returns>A DirectionMap object</returns>
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
            int totalSize = width * height;
            NextTileDirection[] directionMap = new NextTileDirection[totalSize];
            float[] distanceMap = new float[totalSize];
            bool[] visited = new bool[totalSize];
            List<int> accessibleTilesFlatIndexes = new();
            int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
            visited[targetIndex] = true;
            directionMap[targetIndex] = NextTileDirection.SELF;
            distanceMap[targetIndex] = 0f;
            accessibleTilesFlatIndexes.Add(targetIndex);
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
                    newDistance = distanceMap[currentIndex] + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                    if (newDistance <= maxDistance && (!visited[neighborIndex] || newDistance < distanceMap[neighborIndex]))
                    {
                        visitedCount++;
                        if (!visited[neighborIndex])
                        {
                            accessibleTilesFlatIndexes.Add(neighborIndex);
                        }
                        visited[neighborIndex] = true;
                        directionMap[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                        distanceMap[neighborIndex] = newDistance;
                        frontier.Enqueue(neiTile, newDistance);
                    }
                }
                neighbourgs.Clear();
            }
            return new DijkstraField(maxDistance, accessibleTilesFlatIndexes.ToArray(), directionMap, distanceMap, targetIndex);
        }
        /// <summary>
        /// Asynchronously generates a DijkstraField holds the direction data between a target tile and all the tiles that are accessible to this target into the specified maximum distance range.
        /// This allows you to run them more often than DijkstraMap because of the early exit due to the maximum distance parameter(note that more higher is the distance, more costly is the generation).  
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
        /// <returns>A DirectionMap object</returns>
        public static Task<DijkstraField> GenerateDijkstraFieldAsync<T>(T[,] grid, T targetTile, float maxDistance, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : IWeightedTile
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
                int totalSize = width * height;
                NextTileDirection[] directionMap = new NextTileDirection[totalSize];
                float[] distanceMap = new float[totalSize];
                bool[] visited = new bool[totalSize];
                List<int> accessibleTilesFlatIndexes = new();
                int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
                visited[targetIndex] = true;
                directionMap[targetIndex] = NextTileDirection.SELF;
                distanceMap[targetIndex] = 0f;
                accessibleTilesFlatIndexes.Add(targetIndex);
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
                        newDistance = distanceMap[currentIndex] + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                        if (newDistance <= maxDistance && (!visited[neighborIndex] || newDistance < distanceMap[neighborIndex]))
                        {
                            visitedCount++;
                            if (!visited[neighborIndex])
                            {
                                accessibleTilesFlatIndexes.Add(neighborIndex);
                            }
                            visited[neighborIndex] = true;
                            directionMap[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                            distanceMap[neighborIndex] = newDistance;
                            frontier.Enqueue(neiTile, newDistance);
                        }
                    }
                    neighbourgs.Clear();
                }
                return new DijkstraField(maxDistance, accessibleTilesFlatIndexes.ToArray(), directionMap, distanceMap, targetIndex);
            });
            return task;
        }
        /// <summary>
        /// Generates a unique path from the startTile to the targetTile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the path calculation</param>
        /// <param name="startTile">The start tile for the path calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the path calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the path calculation</param>
        /// <returns>An array of tiles representing the path</returns>
        public static T[] GenerateUniquePath<T>(T[,] grid, T targetTile, T startTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f) where T : IWeightedTile
        {
            if (targetTile == null || !targetTile.IsWalkable)
            {
                throw new Exception("Do not try to generate a DijkstraField with an unwalkable (or null) tile as the target");
            }
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int gridDimensions = new Vector2Int(width, height);
            int totalSize = width * height;
            NextTileDirection[] directionMap = new NextTileDirection[totalSize];
            float[] distanceMap = new float[totalSize];
            bool[] visited = new bool[totalSize];
            List<int> accessibleTilesFlatIndexes = new();
            int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
            visited[targetIndex] = true;
            directionMap[targetIndex] = NextTileDirection.SELF;
            distanceMap[targetIndex] = 0f;
            accessibleTilesFlatIndexes.Add(targetIndex);
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
                    List<T> path = new List<T>() { t };
                    while (!GridUtils.TileEquals(t, targetTile))
                    {
                        Vector2Int nextTileDirection = GridUtils.NextNodeDirectionToVector2Int(directionMap[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), t.X, t.Y)]);
                        Vector2Int nextTileCoords = new(t.X + nextTileDirection.x, t.Y + nextTileDirection.y);
                        T nextTile = GridUtils.GetTile(grid, nextTileCoords.x, nextTileCoords.y);
                        path.Add(nextTile);
                        t = nextTile;
                    }
                    return path.ToArray();
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
                    newDistance = distanceMap[currentIndex] + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                    if (!visited[neighborIndex] || newDistance < distanceMap[neighborIndex])
                    {
                        visitedCount++;
                        if (!visited[neighborIndex])
                        {
                            accessibleTilesFlatIndexes.Add(neighborIndex);
                        }
                        visited[neighborIndex] = true;
                        directionMap[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                        distanceMap[neighborIndex] = newDistance;
                        frontier.Enqueue(neiTile, newDistance + FloatHeuristic(new(startTile.X, startTile.Y), new(neiTile.X, neiTile.Y)));
                    }
                }
                neighbourgs.Clear();
            }
            return null;
        }
        /// <summary>
        /// Asynchronously generates a unique path from the startTile to the targetTile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the path calculation</param>
        /// <param name="startTile">The start tile for the path calculation</param>
        /// <param name="diagonalsPolicy">The diagonal movements policy for the path calculation</param>
        /// <param name="diagonalsWeight">The diagonal movements cost for the path calculation</param>
        /// <returns>An array of tiles representing the path</returns>
        public static Task<T[]> GenerateUniquePathAsync<T>(T[,] grid, T targetTile, T startTile, DiagonalsPolicy diagonalsPolicy = DiagonalsPolicy.NONE, float diagonalsWeight = 1.414f, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : IWeightedTile
        {
            Task<T[]> task = Task.Run(() =>
            {
                if (targetTile == null || !targetTile.IsWalkable)
                {
                    throw new Exception("Do not try to generate a DijkstraField with an unwalkable (or null) tile as the target");
                }
                int width = grid.GetLength(0);
                int height = grid.GetLength(1);
                Vector2Int gridDimensions = new Vector2Int(width, height);
                int totalSize = width * height;
                NextTileDirection[] directionMap = new NextTileDirection[totalSize];
                float[] distanceMap = new float[totalSize];
                bool[] visited = new bool[totalSize];
                List<int> accessibleTilesFlatIndexes = new();
                int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
                visited[targetIndex] = true;
                directionMap[targetIndex] = NextTileDirection.SELF;
                distanceMap[targetIndex] = 0f;
                accessibleTilesFlatIndexes.Add(targetIndex);
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
                    if (GridUtils.TileEquals(current, startTile))
                    {
                        T t = current;
                        List<T> path = new List<T>() { t };
                        while (!GridUtils.TileEquals(t, targetTile))
                        {
                            Vector2Int nextTileDirection = GridUtils.NextNodeDirectionToVector2Int(directionMap[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), t.X, t.Y)]);
                            Vector2Int nextTileCoords = new(t.X + nextTileDirection.x, t.Y + nextTileDirection.y);
                            T nextTile = GridUtils.GetTile(grid, nextTileCoords.x, nextTileCoords.y);
                            path.Add(nextTile);
                            t = nextTile;
                        }
                        return path.ToArray();
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
                        newDistance = distanceMap[currentIndex] + neiTile.Weight * (isDiagonal ? diagonalsWeight : 1f);
                        if (!visited[neighborIndex] || newDistance < distanceMap[neighborIndex])
                        {
                            visitedCount++;
                            if (!visited[neighborIndex])
                            {
                                accessibleTilesFlatIndexes.Add(neighborIndex);
                            }
                            visited[neighborIndex] = true;
                            directionMap[neighborIndex] = GridUtils.GetDirectionBetweenAdjacentTiles(neiTile, current);
                            distanceMap[neighborIndex] = newDistance;
                            frontier.Enqueue(neiTile, newDistance + FloatHeuristic(new(startTile.X, startTile.Y), new(neiTile.X, neiTile.Y)));
                        }
                    }
                    neighbourgs.Clear();
                }
                return null;
            });
            return task;
        }
    }
}
