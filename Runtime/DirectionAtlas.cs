using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Caskev.GridToolkit
{
    /// <summary>
    /// Asynchronously generates a DirectionAtlas that holds DirectionGrid objects for each tile.  
    /// Once generated, this object contains all the paths between any tiles on the grid, with almost no performance cost.
    /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
    /// Be carefull as the memory usage can be huge depending on the grid size.
    /// </summary>
    public class DirectionAtlas
    {
        internal readonly DirectionGrid[] _directionAtlas;

        internal DirectionAtlas(DirectionGrid[] directionAtlas)
        {
            _directionAtlas = directionAtlas;
        }

        /// <summary>
        /// Is there a path between two tiles.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <returns>A boolean value</returns>
        public bool HasPath<T>(T[,] grid, T startTile, T destinationTile) where T : ITile
        {
            if (startTile == null || !startTile.IsWalkable || destinationTile == null || !destinationTile.IsWalkable)
            {
                return false;
            }
            DirectionGrid directionGrid = _directionAtlas[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), destinationTile.X, destinationTile.Y)];
            return directionGrid.IsTileAccessible(grid, startTile);
        }
        /// <summary>
        /// Get the next tile on the path between a start tile and a destination tile.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <returns>A tile object</returns>
        public T GetNextTileFromTile<T>(T[,] grid, T startTile, T destinationTile) where T : ITile
        {
            if (startTile == null || !startTile.IsWalkable || destinationTile == null || !destinationTile.IsWalkable)
            {
                throw new Exception("Do not call this method with non-walkable (or null) tiles");
            }
            DirectionGrid directionGrid = _directionAtlas[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), destinationTile.X, destinationTile.Y)];
            if (!directionGrid.IsTileAccessible(grid, startTile))
            {
                return default;
            }
            return directionGrid.GetNextTileFromTile(grid, startTile);
        }
        /// <summary>
        /// Get the next tile on the path between a start tile and a destination tile.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <returns>A Vector2Int direction</returns>
        public TileDirection GetNextTileDirectionFromTile<T>(T[,] grid, T startTile, T destinationTile) where T : ITile
        {
            if (startTile == null || !startTile.IsWalkable || destinationTile == null || !destinationTile.IsWalkable)
            {
                throw new Exception("Do not call this method with non-walkable (or null) tiles");
            }
            DirectionGrid directionGrid = _directionAtlas[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), destinationTile.X, destinationTile.Y)];
            return directionGrid.GetNextTileDirectionFromTile(grid, startTile);
        }
        /// <summary>
        /// Get all the tiles on the path from a start tile to a destination tile. If there is no path between the two tiles then an empty array will be returned.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeDestination">Include the target tile into the resulting array or not</param>
        /// <returns>An array of tiles</returns>
        public T[] GetPath<T>(T[,] grid, T startTile, T destinationTile, bool includeStart = true, bool includeDestination = true) where T : ITile
        {
            if (startTile == null || !startTile.IsWalkable || destinationTile == null || !destinationTile.IsWalkable)
            {
                throw new Exception("Do not call this method with non-walkable (or null) tiles");
            }
            DirectionGrid directionGrid = _directionAtlas[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), destinationTile.X, destinationTile.Y)];
            if (!directionGrid.IsTileAccessible(grid, startTile))
            {
                return new T[0];
            }
            return directionGrid.GetPathToTarget(grid, startTile, includeStart, includeDestination);
        }
        /// <summary>
        /// Serializes a DirectionAtlas to a byte array. 
        /// </summary>
        /// <returns>The serialized DirectionAtlas.</returns>
        public byte[] ToByteArray()
        {
            int bytesCount = _directionAtlas.Length * _directionAtlas.Count(x => x != null) + _directionAtlas.Length;
            byte[] bytes = new byte[bytesCount];
            int byteIndex = 0;
            for (int i = 0; i < _directionAtlas.Length; i++)
            {
                if (_directionAtlas[i] == null)
                {
                    bytes[byteIndex] = 0;
                    byteIndex++;
                    continue;
                }
                else
                {
                    bytes[byteIndex] = 1;
                    byteIndex++;
                    for (int j = 0; j < _directionAtlas.Length; j++)
                    {
                        bytes[byteIndex] = (byte)_directionAtlas[i]._directionGrid[j];
                        byteIndex++;
                    }
                }
            }
            return bytes;
        }
        /// <summary>
        /// Asynchronously serializes a DirectionAtlas to a byte array. 
        /// </summary>
        /// <param name="progress">An optional IProgress object to get the deserialization progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the serialization</param>
        /// <returns>The serialized DirectionAtlas.</returns>
        public Task<byte[]> ToByteArrayAsync<T>(IProgress<float> progress = null, CancellationToken cancelToken = default) where T : ITile
        {
            Task<byte[]> task = Task.Run(() =>
            {
                int bytesCount = _directionAtlas.Length * _directionAtlas.Count(x => x != null) + _directionAtlas.Length;
                byte[] bytes = new byte[bytesCount];
                int byteIndex = 0;
                for (int i = 0; i < _directionAtlas.Length; i++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress.Report((float)i / _directionAtlas.Length);
                    if (_directionAtlas[i] == null)
                    {
                        bytes[byteIndex] = 0;
                        byteIndex++;
                        continue;
                    }
                    else
                    {
                        bytes[byteIndex] = 1;
                        byteIndex++;
                        for (int j = 0; j < _directionAtlas.Length; j++)
                        {
                            bytes[byteIndex] = (byte)_directionAtlas[i]._directionGrid[j];
                            byteIndex++;
                        }
                    }
                }
                return bytes;
            });
            return task;
        }
        /// <summary>
        /// Deserializes a DirectionAtlas from a byte array. 
        /// </summary>
        /// <param name="grid">The user grid.</param>
        /// <param name="bytes">The serialized DirectionAtlas.</param>
        /// <returns>The deserialized DirectionAtlas.</returns>
        public static DirectionAtlas FromByteArray<T>(T[,] grid, byte[] bytes) where T : ITile
        {
            if (grid == null)
            {
                throw new ArgumentException("The grid cannot be null");
            }
            int byteIndex = 0;
            DirectionGrid[] directionAtlas = new DirectionGrid[grid.Length];
            for (int i = 0; i < grid.Length; i++)
            {
                bool isWalkable = bytes[byteIndex] == 1;
                byteIndex++;
                if (!isWalkable)
                {
                    directionAtlas[i] = null;
                    continue;
                }
                else
                {
                    TileDirection[] directionGrid = new TileDirection[grid.Length];
                    for (int j = 0; j < grid.Length; j++)
                    {
                        directionGrid[j] = (TileDirection)bytes[byteIndex];
                        byteIndex++;
                    }
                    directionAtlas[i] = new DirectionGrid(directionGrid, i);
                }
            }
            return new DirectionAtlas(directionAtlas);
        }
        /// <summary>
        /// Asynchronously deserializes a DirectionAtlas from a byte array. 
        /// </summary>
        /// <param name="grid">The user grid.</param>
        /// <param name="bytes">The serialized DirectionAtlas.</param>
        /// <param name="progress">An optional IProgress object to get the deserialization progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the deserialization</param>
        /// <returns>The deserialized DirectionAtlas.</returns>
        public static Task<DirectionAtlas> FromByteArrayAsync<T>(T[,] grid, byte[] bytes, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : ITile
        {
            Task<DirectionAtlas> task = Task.Run(() =>
            {
                if (grid == null)
                {
                    throw new ArgumentException("The grid cannot be null");
                }
                int byteIndex = 0;
                DirectionGrid[] directionAtlas = new DirectionGrid[grid.Length];
                for (int i = 0; i < grid.Length; i++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress.Report((float)i / grid.Length);
                    bool isWalkable = bytes[byteIndex] == 1;
                    byteIndex++;
                    if (!isWalkable)
                    {
                        directionAtlas[i] = null;
                        continue;
                    }
                    else
                    {
                        TileDirection[] directionGrid = new TileDirection[grid.Length];
                        for (int j = 0; j < grid.Length; j++)
                        {
                            directionGrid[j] = (TileDirection)bytes[byteIndex];
                            byteIndex++;
                        }
                        directionAtlas[i] = new DirectionGrid(directionGrid, i);
                    }
                }
                return new DirectionAtlas(directionAtlas);
            });
            return task;
        }
    }
}
