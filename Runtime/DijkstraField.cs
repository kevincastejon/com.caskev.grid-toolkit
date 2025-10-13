using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    /// <summary>
    /// A DijkstraField holds the direction data between a target tile and all the tiles that are accessible to this target into the specified maximum distance range.
    /// This allows you to run them more often because of the early exit due to the maximum distance parameter(note that more higher is the distance, more costly is the generation).  
    /// Once generated, this object offers you a way to get the accessible tiles within a range, and paths to them, with almost no performance cost(ie: a strategy game where you want to check the tiles in range of your character)
    /// </summary>
    public class DijkstraField
    {
        private readonly int _target;
        private float _maxDistance;
        internal Dictionary<int, (TileDirection, float)> _accessibleTiles;
        /// <summary>
        /// The maximum distance used to generate this DirectionField
        /// </summary>
        public float MaxDistance => _maxDistance;
        /// <summary>
        /// Gets the total number of accessible tiles.
        /// </summary>
        public int AccessibleTilesCount => _accessibleTiles.Count;

        internal DijkstraField(int target, float maxDistance, Dictionary<int, (TileDirection, float)> accessibleTiles)
        {
            _target = target;
            _maxDistance = maxDistance;
            _accessibleTiles = accessibleTiles;
        }
        /// <summary>
        /// Is the tile is accessible from the target.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">The tile to check</param>
        /// <returns>A boolean value</returns>
        public bool IsTileAccessible<T>(T[,] grid, T tile) where T : ITile
        {
            if (tile == null)
            {
                return false;
            }
            return _accessibleTiles.ContainsKey(GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y));
        }
        /// <summary>
        /// Returns the target tile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <returns></returns>
        public T GetTargetTile<T>(T[,] grid) where T : ITile
        {
            Vector2Int targetCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _target);
            return GridUtils.GetTile(grid, targetCoords.x, targetCoords.y);
        }
        /// <summary>
        /// Get the next tile on the path between a tile and the target.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <returns>A tile object</returns>
        public T GetNextTileFromTile<T>(T[,] grid, T tile) where T : ITile
        {
            if (!IsTileAccessible(grid, tile))
            {
                throw new Exception("Do not call this method with an inaccessible tile");
            }
            return GetNextTile(grid, tile);
        }
        /// <summary>
        /// Get the next tile on the path between the target and a tile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">The tile</param>
        /// <returns>A Vector2Int direction</returns>
        public TileDirection GetNextTileDirectionFromTile<T>(T[,] grid, T tile) where T : ITile
        {
            if (!IsTileAccessible(grid, tile))
            {
                throw new Exception("Do not call this method with an inaccessible tile");
            }
            return _accessibleTiles[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y)].Item1;
        }
        /// <summary>
        /// Get all the tiles on the path from a tile to the target.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeTarget">Include the target tile into the resulting array or not</param>
        /// <returns>An array of tiles</returns>
        public T[] GetPathToTarget<T>(T[,] grid, T startTile, bool includeStart = true, bool includeTarget = true) where T : ITile
        {
            if (!IsTileAccessible(grid, startTile))
            {
                throw new Exception("Do not call this method with an inaccessible tile");
            }

            Vector2Int targetCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _target);
            T target = GridUtils.GetTile(grid, targetCoords.x, targetCoords.y);

            T tile = includeStart ? startTile : GetNextTile(grid, startTile);
            bool targetReached = GridUtils.TileEquals(tile, target);
            if (!includeTarget && targetReached)
            {
                return new T[0];
            }
            List<T> tiles = new List<T>() { tile };
            while (!targetReached)
            {
                tile = GetNextTile(grid, tile);
                targetReached = GridUtils.TileEquals(tile, target);
                if (includeTarget || !targetReached)
                {
                    tiles.Add(tile);
                }
            }
            return tiles.ToArray();
        }
        /// <summary>
        /// Get all the tiles on the path from the target to a tile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="includeDestination">Include the destination tile into the resulting array or not. Default is true</param>
        /// <param name="includeTarget">Include the target tile into the resulting array or not</param>
        /// <returns>An array of tiles</returns>
        public T[] GetPathFromTarget<T>(T[,] grid, T destinationTile, bool includeDestination = true, bool includeTarget = true) where T : ITile
        {
            T[] path = GetPathToTarget(grid, destinationTile, includeDestination, includeTarget);
            T[] reversedPath = new T[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                reversedPath[i] = path[path.Length - 1 - i];
            }
            return reversedPath;
        }
        /// <summary>
        /// Gets the next tile from the specified tile along the path to the target tile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">Any tile that is accessible to the target tile</param>
        /// <returns></returns>
        protected T GetNextTile<T>(T[,] grid, T tile) where T : ITile
        {
            if (!IsTileAccessible(grid, tile))
            {
                throw new Exception("Do not call this method with an inaccessible tile");
            }
            Vector2Int nextTileDirection = GridUtils.NextNodeDirectionToVector2Int(_accessibleTiles[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y)].Item1);
            Vector2Int nextTileCoords = new(tile.X + nextTileDirection.x, tile.Y + nextTileDirection.y);
            return GridUtils.GetTile(grid, nextTileCoords.x, nextTileCoords.y);
        }
        /// <summary>
        /// Use this method to iterate though the accessible tiles. <see cref="AccessibleTilesCount"/>.
        /// </summary>
        /// <typeparam name="T">The type of the tile, which must implement the <see cref="ITile"/> interface.</typeparam>
        /// <param name="grid">A two-dimensional array representing the grid of tiles.</param>
        /// <param name="index">The zero-based index of the accessible tile in the accessible tiles list <see cref="AccessibleTilesCount"/>.</param>
        /// <returns>The tile of type <typeparamref name="T"/> located at the specified index in the accessible tiles list.</returns>
        public T GetAccessibleTile<T>(T[,] grid, int index) where T : ITile
        {
            Vector2Int coords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _accessibleTiles.ElementAt(index).Key);
            return grid[coords.y, coords.x];
        }
        /// <summary>
        /// Returns the tiles that accessible to the target.
        /// </summary>
        /// <param name="grid">A two-dimensional array representing the grid of tiles.</param>
        /// <returns>The tiles that accessible to the target.</returns>
        public T[] GetAccessibleTiles<T>(T[,] grid) where T : IWeightedTile
        {
            T[] accessibleTiles = new T[AccessibleTilesCount];
            for (int i = 0; i < _accessibleTiles.Count; i++)
            {
                accessibleTiles[i] = GetAccessibleTile(grid, i);
            }
            return accessibleTiles;
        }
        /// <summary>
        /// Returns the tiles that accessible to the target.
        /// </summary>
        /// <param name="grid">A two-dimensional array representing the grid of tiles.</param>
        /// <returns>The tiles that accessible to the target.</returns>
        public void GetAccessibleTilesNoAlloc<T>(T[,] grid, T[] accessibleTiles) where T : IWeightedTile
        {
            for (int i = 0; i < accessibleTiles.Length; i++)
            {
                if (_accessibleTiles.Count <= i)
                {
                    return;
                }
                accessibleTiles[i] = GetAccessibleTile(grid, i);
            }
        }
        /// <summary>
        /// Get the distance from the specified tile to the target.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">The tile from which to get the distance to the target tile</param>
        /// <returns>The float distance from the specified tile to the target tile</returns>
        public float GetDistanceToTarget<T>(T[,] grid, T tile) where T : IWeightedTile
        {
            if (!IsTileAccessible(grid, tile))
            {
                throw new Exception("Do not call this method with an inaccessible tile");
            }
            int tileFlatIndex = GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y);
            return _accessibleTiles[tileFlatIndex].Item2;
        }
    }
}
