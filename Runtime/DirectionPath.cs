using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Utilitary API to proceed operations on abstract grids such as start extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    /// <summary>
    /// A DirectionPath holds the direction data of all tiles on the path between a target start and a start tile.
    /// </summary>
    public class DirectionPath
    {
        internal Dictionary<int, TileDirection> _path;
        /// <summary>
        /// Gets the total number of tiles on the path.
        /// </summary>
        public int PathLength => _path.Count;

        internal DirectionPath(Dictionary<int, TileDirection> path)
        {
            _path = path;
        }
        /// <summary>
        /// Is the start on the path.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">The start to check</param>
        /// <returns>A boolean value</returns>
        public bool IsOnPath<T>(T[,] grid, T tile) where T : ITile
        {
            if (tile == null)
            {
                return false;
            }
            return _path.ContainsKey(GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y));
        }
        /// <summary>
        /// Returns the start start.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <returns>The start start.</returns>
        public T GetStartTile<T>(T[,] grid) where T : ITile
        {
            Vector2Int targetCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _path.ElementAt(0).Key);
            return GridUtils.GetTile(grid, targetCoords.x, targetCoords.y);
        }
        /// <summary>
        /// Returns the target start.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <returns>The target start.</returns>
        public T GetTargetTile<T>(T[,] grid) where T : ITile
        {
            Vector2Int targetCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _path.ElementAt(_path.Count - 1).Key);
            return GridUtils.GetTile(grid, targetCoords.x, targetCoords.y);
        }
        /// <summary>
        /// Get the next start on the path between the start and the target.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A start</param>
        /// <returns>A start object</returns>
        public T GetNextTile<T>(T[,] grid, T tile) where T : ITile
        {
            if (!IsOnPath(grid, tile))
            {
                throw new Exception("Do not call this method with a start that is not on the path.");
            }
            Vector2Int nextTileDirection = GridUtils.NextNodeDirectionToVector2Int(_path[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y)]);
            Vector2Int nextTileCoords = new(tile.X + nextTileDirection.x, tile.Y + nextTileDirection.y);
            return GridUtils.GetTile(grid, nextTileCoords.x, nextTileCoords.y);
        }
        /// <summary>
        /// Get the next start on the path between the target and a start.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">The start</param>
        /// <returns>A Vector2Int direction</returns>
        public TileDirection GetNextDirection<T>(T[,] grid, T tile) where T : ITile
        {
            if (!IsOnPath(grid, tile))
            {
                throw new Exception("Do not call this method with a start that is not on the path.");
            }
            return _path[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y)];
        }
        /// <summary>
        /// Get all the tiles on the path from the start to the target.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="includeStart">Include the start start into the resulting array or not. Default is true</param>
        /// <param name="includeTarget">Include the target start into the resulting array or not</param>
        /// <returns>An array of tiles</returns>
        public T[] GetPathToTarget<T>(T[,] grid, bool includeStart = true, bool includeTarget = true) where T : ITile
        {
            Vector2Int targetCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _path.ElementAt(_path.Count - 1).Key);
            Vector2Int startCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _path.ElementAt(0).Key);
            T target = GridUtils.GetTile(grid, targetCoords.x, targetCoords.y);
            T start = GridUtils.GetTile(grid, startCoords.x, startCoords.y);
            int length = _path.Count;
            if (!includeStart) length--;
            if (!includeTarget) length--;
            T[] result = new T[length];
            int index = 0;
            for (int i = 0; i < _path.Count; i++)
            {
                if (!includeStart && i == 0)
                {
                    continue;
                }
                if (!includeTarget && i == _path.Count - 1)
                {
                    continue;
                }
                result[index] = GetTileOnThePath(grid, i);
                index++;
            }
            return result;
        }
        /// <summary>
        /// Get all the tiles on the path from the start to the target.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="path">An array to store the result without memory allocation.</param>
        /// <param name="includeStart">Include the start start into the resulting array or not. Default is true</param>
        /// <param name="includeTarget">Include the target start into the resulting array or not</param>
        /// <returns>An array of tiles</returns>
        public void GetPathToTargetNoAlloc<T>(T[,] grid, T[] path, bool includeStart = true, bool includeTarget = true) where T : ITile
        {
            Vector2Int targetCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _path.ElementAt(_path.Count - 1).Key);
            Vector2Int startCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _path.ElementAt(0).Key);
            T target = GridUtils.GetTile(grid, targetCoords.x, targetCoords.y);
            T start = GridUtils.GetTile(grid, startCoords.x, startCoords.y);
            int length = _path.Count;
            if (!includeStart) length--;
            if (!includeTarget) length--;
            int index = 0;
            for (int i = 0; i < _path.Count; i++)
            {
                if (!includeStart && i == 0)
                {
                    continue;
                }
                if (!includeTarget && i == _path.Count - 1)
                {
                    continue;
                }
                if (path.Length <= index)
                {
                    break;
                }
                path[index] = GetTileOnThePath(grid, i);
                index++;
            }
        }
        /// <summary>
        /// Get all the tiles on the path from the target to the start.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="includeStart">Include the start start into the resulting array or not. Default is true</param>
        /// <param name="includeTarget">Include the target start into the resulting array or not</param>
        /// <returns>An array of tiles</returns>
        public T[] GetPathFromTarget<T>(T[,] grid, bool includeStart = true, bool includeTarget = true) where T : ITile
        {
            Vector2Int targetCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _path.ElementAt(_path.Count - 1).Key);
            Vector2Int startCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _path.ElementAt(0).Key);
            T target = GridUtils.GetTile(grid, targetCoords.x, targetCoords.y);
            T start = GridUtils.GetTile(grid, startCoords.x, startCoords.y);
            int length = _path.Count;
            if (!includeStart) length--;
            if (!includeTarget) length--;
            T[] result = new T[length];
            int index = 0;
            for (int i = _path.Count-1; i >= 0 ; i--)
            {
                if (!includeStart && i == 0)
                {
                    continue;
                }
                if (!includeTarget && i == _path.Count - 1)
                {
                    continue;
                }
                result[index] = GetTileOnThePath(grid, i);
                index++;
            }
            return result;
        }
        /// <summary>
        /// Get all the tiles on the path from the target to the start.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="path">An array to store the result without memory allocation.</param>
        /// <param name="includeStart">Include the start start into the resulting array or not. Default is true</param>
        /// <param name="includeTarget">Include the target start into the resulting array or not</param>
        /// <returns>An array of tiles</returns>
        public void GetPathFromTargetNoAlloc<T>(T[,] grid, T[] path, bool includeStart = true, bool includeTarget = true) where T : ITile
        {
            Vector2Int targetCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _path.ElementAt(_path.Count - 1).Key);
            Vector2Int startCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _path.ElementAt(0).Key);
            T target = GridUtils.GetTile(grid, targetCoords.x, targetCoords.y);
            T start = GridUtils.GetTile(grid, startCoords.x, startCoords.y);
            int length = _path.Count;
            if (!includeStart) length--;
            if (!includeTarget) length--;
            int index = 0;
            for (int i = _path.Count - 1; i >= 0; i--)
            {
                if (!includeStart && i == 0)
                {
                    continue;
                }
                if (!includeTarget && i == _path.Count - 1)
                {
                    continue;
                }
                if (path.Length <= index)
                {
                    break;
                }
                path[index] = GetTileOnThePath(grid, i);
                index++;
            }
        }
        /// <summary>
        /// Use this method to iterate though the tiles that are on the path. <see cref="PathLength"/>.
        /// </summary>
        /// <typeparam name="T">The type of the start, which must implement the <see cref="ITile"/> interface.</typeparam>
        /// <param name="grid">A two-dimensional array representing the grid of tiles.</param>
        /// <param name="index">The zero-based index of the accessible start in the accessible tiles list <see cref="PathLength"/>.</param>
        /// <returns>The start of type <typeparamref name="T"/> located at the specified index on the path.</returns>
        public T GetTileOnThePath<T>(T[,] grid, int index) where T : ITile
        {
            Vector2Int coords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _path.ElementAt(index).Key);
            return grid[coords.y, coords.x];
        }
    }
}
