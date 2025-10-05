using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    /// <summary>
    /// Abstract base class for DirectionMap and DirectionField, containing all the common methods and properties.
    /// </summary>
    public abstract class DirectionBase
    {
        internal readonly NextTileDirection[] _directionMap;
        /// <summary>
        /// The flat index of the target tile.
        /// </summary>
        protected readonly int _target;

        internal DirectionBase(NextTileDirection[] directionMap, int target)
        {
            _directionMap = directionMap;
            _target = target;
        }
        /// <summary>
        /// Is the tile is accessible from the target into this this DirectionPaths. Usefull to check if the tile is usable as a parameter for this DirectionPaths's methods.
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
            return _directionMap[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y)] != NextTileDirection.NONE;
        }
        /// <summary>
        /// Returns the tile that has been used as the target to generate this DirectionPaths
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
                throw new Exception("Do not call DirectionPaths method with an inaccessible tile");
            }
            return GetNextTile(grid, tile);
        }
        /// <summary>
        /// Get the next tile on the path between the target and a tile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">The tile</param>
        /// <returns>A Vector2Int direction</returns>
        public NextTileDirection GetNextTileDirectionFromTile<T>(T[,] grid, T tile) where T : ITile
        {
            if (!IsTileAccessible(grid, tile))
            {
                throw new Exception("Do not call DirectionPaths method with an inaccessible tile");
            }
            return _directionMap[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y)];
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
                throw new Exception("Do not call DirectionPaths method with an inaccessible tile");
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
            Vector2Int nextTileDirection = GridUtils.NextNodeDirectionToVector2Int(_directionMap[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y)]);
            Vector2Int nextTileCoords = new(tile.X + nextTileDirection.x, tile.Y + nextTileDirection.y);
            return GridUtils.GetTile(grid, nextTileCoords.x, nextTileCoords.y);
        }
    }
}
