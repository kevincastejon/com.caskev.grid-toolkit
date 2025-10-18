using System;
using System.Buffers.Binary;
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
    /// A DijkstraGrid object holds both direction and distance data between a target tile and all the tiles that are accessible to this target, on the entire grid.
    /// </summary>
    public class DijkstraGrid
    {
        internal readonly TileDirection[] _directionGrid;
        internal readonly float[] _distanceGrid;
        private readonly int _target;
        internal DijkstraGrid(TileDirection[] directionGrid, float[] distanceGrid, int target)
        {
            _directionGrid = directionGrid;
            _distanceGrid = distanceGrid;
            _target = target;
        }
        /// <summary>
        /// Is the tile is accessible from the target.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">The tile to check</param>
        /// <returns>A boolean value</returns>
        public bool IsTileAccessible<T>(T[,] grid, T tile) where T : IWeightedTile
        {
            if (tile == null)
            {
                return false;
            }
            return _directionGrid[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y)] != TileDirection.NONE;
        }
        /// <summary>
        /// Returns the target tile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <returns></returns>
        public T GetTargetTile<T>(T[,] grid) where T : IWeightedTile
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
        public T GetNextTile<T>(T[,] grid, T tile) where T : IWeightedTile
        {
            if (!IsTileAccessible(grid, tile))
            {
                throw new Exception("Do not call this method with an inaccessible tile");
            }
            return GetNextTileUnsafe(grid, tile);
        }
        /// <summary>
        /// Get the next tile on the path between the target and a tile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">The tile</param>
        /// <returns>A Vector2Int direction</returns>
        public TileDirection GetNextDirection<T>(T[,] grid, T tile) where T : IWeightedTile
        {
            if (!IsTileAccessible(grid, tile))
            {
                throw new Exception("Do not call this method with an inaccessible tile");
            }
            return _directionGrid[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y)];
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
            return _distanceGrid[tileFlatIndex];
        }
        /// <summary>
        /// Get all the tiles on the path from a tile to the target.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeTarget">Include the target tile into the resulting array or not</param>
        /// <returns>An array of tiles</returns>
        public T[] GetPathToTarget<T>(T[,] grid, T startTile, bool includeStart = true, bool includeTarget = true) where T : IWeightedTile
        {
            if (!IsTileAccessible(grid, startTile))
            {
                throw new Exception("Do not call this method with an inaccessible tile");
            }

            Vector2Int targetCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _target);
            T target = GridUtils.GetTile(grid, targetCoords.x, targetCoords.y);

            T tile = includeStart ? startTile : GetNextTileUnsafe(grid, startTile);
            bool targetReached = GridUtils.TileEquals(tile, target);
            if (!includeTarget && targetReached)
            {
                return new T[0];
            }
            List<T> tiles = new List<T>() { tile };
            while (!targetReached)
            {
                tile = GetNextTileUnsafe(grid, tile);
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
        public T[] GetPathFromTarget<T>(T[,] grid, T destinationTile, bool includeDestination = true, bool includeTarget = true) where T : IWeightedTile
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
        /// Returns the DijkstraGrid serialized as a byte array.
        /// </summary>
        /// <returns>A byte array representing the serialized DijkstraGrid</returns>
        public byte[] ToByteArray()
        {
            int bytesCount = sizeof(int) + sizeof(int) + ((sizeof(byte) + sizeof(float)) * _directionGrid.Length);
            byte[] bytes = new byte[bytesCount];
            int byteIndex = 0;
            BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), _target);
            byteIndex += sizeof(int);
            BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), _directionGrid.Length);
            byteIndex += sizeof(int);
            for (int i = 0; i < _directionGrid.Length; i++)
            {
                bytes[byteIndex] = (byte)_directionGrid[i];
                byteIndex++;
                BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), BitConverter.SingleToInt32Bits(_distanceGrid[i]));
                byteIndex += sizeof(float);
            }
            return bytes;
        }
        /// <summary>
        /// Returns the DijkstraGrid serialized as a byte array.
        /// </summary>
        /// <param name="progress">An optional IProgress object to get the serialization progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the serialization</param>
        /// <returns>A byte array representing the serialized DijkstraGrid</returns>
        public Task<byte[]> ToByteArrayAsync(IProgress<float> progress = null, CancellationToken cancelToken = default)
        {
            Task<byte[]> task = Task.Run(() =>
            {
                int bytesCount = sizeof(int) + sizeof(int) + ((sizeof(byte) + sizeof(float)) * _directionGrid.Length);
                byte[] bytes = new byte[bytesCount];
                int byteIndex = 0;
                BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), _target);
                byteIndex += sizeof(int);
                BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), _directionGrid.Length);
                byteIndex += sizeof(int);
                for (int i = 0; i < _directionGrid.Length; i++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress.Report((float)i / _directionGrid.Length);
                    bytes[byteIndex] = (byte)_directionGrid[i];
                    byteIndex++;
                    BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), BitConverter.SingleToInt32Bits(_distanceGrid[i]));
                    byteIndex += sizeof(float);
                }
                return bytes;
            });
            return task;
        }
        /// <summary>
        /// Returns a DijkstraGrid from a byte array that has been serialized with the ToByteArray method.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="bytes">The serialized byte array</param>
        /// <returns>The deserialized DijkstraGrid</returns>
        public static DijkstraGrid FromByteArray<T>(T[,] grid, byte[] bytes) where T : IWeightedTile
        {
            if (grid == null)
            {
                throw new ArgumentException("The grid cannot be null");
            }
            int byteIndex = 0;
            int target = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex));
            byteIndex += sizeof(int);
            int count = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex));
            byteIndex += sizeof(int);
            TileDirection[] directionGrid = new TileDirection[count];
            float[] distanceGrid = new float[count];
            for (int i = 0; i < count; i++)
            {
                directionGrid[i] = (TileDirection)bytes[byteIndex];
                byteIndex++;
                distanceGrid[i] = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex)));
                byteIndex += sizeof(float);
            }
            return new DijkstraGrid(directionGrid, distanceGrid, target);
        }
        /// <summary>
        /// Returns a DijkstraGrid from a byte array that has been serialized with the ToByteArray method.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="bytes">The serialized byte array</param>
        /// <param name="progress">An optional IProgress object to get the deserialization progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the deserialization</param>
        /// <returns>The deserialized DijkstraGrid</returns>
        public static Task<DijkstraGrid> FromByteArrayAsync<T>(T[,] grid, byte[] bytes, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : IWeightedTile
        {
            Task<DijkstraGrid> task = Task.Run(() =>
            {
                if (grid == null)
                {
                    throw new ArgumentException("The grid cannot be null");
                }
                int byteIndex = 0;
                int target = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex));
                byteIndex += sizeof(int);
                int count = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex));
                byteIndex += sizeof(int);
                TileDirection[] directionGrid = new TileDirection[count];
                float[] distanceGrid = new float[count];
                for (int i = 0; i < count; i++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress.Report((float)i / count);
                    directionGrid[i] = (TileDirection)bytes[byteIndex];
                    byteIndex++;
                    distanceGrid[i] = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex)));
                    byteIndex += sizeof(float);
                }
                return new DijkstraGrid(directionGrid, distanceGrid, target);
            });
            return task;
        }
        /// <summary>
        /// Gets the next tile from the specified tile along the path to the target tile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">Any tile that is accessible to the target tile</param>
        /// <returns></returns>
        private T GetNextTileUnsafe<T>(T[,] grid, T tile) where T : IWeightedTile
        {
            Vector2Int nextTileDirection = GridUtils.NextNodeDirectionToVector2Int(_directionGrid[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y)]);
            Vector2Int nextTileCoords = new(tile.X + nextTileDirection.x, tile.Y + nextTileDirection.y);
            return GridUtils.GetTile(grid, nextTileCoords.x, nextTileCoords.y);
        }
    }
}
