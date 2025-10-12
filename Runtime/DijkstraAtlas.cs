using System;
using System.Buffers.Binary;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Caskev.GridToolkit
{
    /// <summary>
    /// Asynchronously generates a DijkstraAtlas that holds DijkstraGrid objects for each tile.  
    /// Once generated, this object contains all the paths and distances between any tiles on the grid, with almost no performance cost.
    /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
    /// Be carefull as the memory usage can be huge depending on the grid size.
    /// </summary>
    public class DijkstraAtlas
    {
        internal readonly DijkstraGrid[] _dijkstraAtlas;

        internal DijkstraAtlas(DijkstraGrid[] dijkstraAtlas)
        {
            _dijkstraAtlas = dijkstraAtlas;
        }

        /// <summary>
        /// Is there a path between two tiles.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <returns>A boolean value</returns>
        public bool HasPath<T>(T[,] grid, T startTile, T destinationTile) where T : IWeightedTile
        {
            if (startTile == null || !startTile.IsWalkable || destinationTile == null || !destinationTile.IsWalkable)
            {
                return false;
            }
            DijkstraGrid dijkstraGrid = _dijkstraAtlas[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), destinationTile.X, destinationTile.Y)];
            return dijkstraGrid.IsTileAccessible(grid, startTile);
        }
        /// <summary>
        /// Get the next tile on the path between a start tile and a destination tile.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <returns>A tile object</returns>
        public T GetNextTileFromTile<T>(T[,] grid, T startTile, T destinationTile) where T : IWeightedTile
        {
            if (startTile == null || !startTile.IsWalkable || destinationTile == null || !destinationTile.IsWalkable)
            {
                throw new Exception("Do not call this method with non-walkable (or null) tiles");
            }
            DijkstraGrid dijkstraGrid = _dijkstraAtlas[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), destinationTile.X, destinationTile.Y)];
            if (!dijkstraGrid.IsTileAccessible(grid, startTile))
            {
                return default;
            }
            return dijkstraGrid.GetNextTileFromTile(grid, startTile);
        }
        /// <summary>
        /// Get the next tile on the path between a start tile and a destination tile.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <returns>A Vector2Int direction</returns>
        public NextTileDirection GetNextTileDirectionFromTile<T>(T[,] grid, T startTile, T destinationTile) where T : IWeightedTile
        {
            if (startTile == null || !startTile.IsWalkable || destinationTile == null || !destinationTile.IsWalkable)
            {
                throw new Exception("Do not call this method with non-walkable (or null) tiles");
            }
            DijkstraGrid dijkstraGrid = _dijkstraAtlas[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), destinationTile.X, destinationTile.Y)];
            return dijkstraGrid.GetNextTileDirectionFromTile(grid, startTile);
        }
        /// <summary>
        /// Get the distance between two tiles.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <returns>The distance between the two tiles.</returns>
        public float GetDistanceBetweenTiles<T>(T[,] grid, T startTile, T destinationTile) where T : IWeightedTile
        {
            if (startTile == null || !startTile.IsWalkable || destinationTile == null || !destinationTile.IsWalkable)
            {
                throw new Exception("Do not call this method with non-walkable (or null) tiles");
            }
            DijkstraGrid dijkstraGrid = _dijkstraAtlas[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), destinationTile.X, destinationTile.Y)];
            return dijkstraGrid.GetDistanceToTarget(grid, startTile);
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
        public T[] GetPath<T>(T[,] grid, T startTile, T destinationTile, bool includeStart = true, bool includeDestination = true) where T : IWeightedTile
        {
            if (startTile == null || !startTile.IsWalkable || destinationTile == null || !destinationTile.IsWalkable)
            {
                throw new Exception("Do not call this method with non-walkable (or null) tiles");
            }
            DijkstraGrid dijkstraGrid = _dijkstraAtlas[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), destinationTile.X, destinationTile.Y)];
            if (!dijkstraGrid.IsTileAccessible(grid, startTile))
            {
                return new T[0];
            }
            return dijkstraGrid.GetPathToTarget(grid, startTile, includeStart, includeDestination);
        }
        /// <summary>
        /// Serializes a DijkstraAtlas to a byte array. 
        /// </summary>
        /// <returns>The serialized DijkstraAtlas.</returns>
        public byte[] ToByteArray()
        {
            int bytesCount = _dijkstraAtlas.Length * (_dijkstraAtlas.Count(x => x != null) * (sizeof(float) + 1)) + _dijkstraAtlas.Length;
            byte[] bytes = new byte[bytesCount];
            int byteIndex = 0;
            for (int i = 0; i < _dijkstraAtlas.Length; i++)
            {
                if (_dijkstraAtlas[i] == null)
                {
                    bytes[byteIndex] = 0;
                    byteIndex++;
                    continue;
                }
                else
                {
                    bytes[byteIndex] = 1;
                    byteIndex++;
                    for (int j = 0; j < _dijkstraAtlas.Length; j++)
                    {
                        bytes[byteIndex] = (byte)_dijkstraAtlas[i]._directionGrid[j];
                        byteIndex++;
                        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), BitConverter.SingleToInt32Bits(_dijkstraAtlas[i]._distanceGrid[j]));
                        byteIndex += sizeof(float);
                    }
                }
            }
            return bytes;
        }
        /// <summary>
        /// Asynchronously serializes a DijkstraAtlas to a byte array. 
        /// </summary>
        /// <param name="progress">An optional IProgress object to get the deserialization progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the serialization</param>
        /// <returns>The serialized DijkstraAtlas.</returns>
        public Task<byte[]> ToByteArrayAsync<T>(IProgress<float> progress = null, CancellationToken cancelToken = default) where T : ITile
        {
            Task<byte[]> task = Task.Run(() =>
            {
                int bytesCount = _dijkstraAtlas.Length * (_dijkstraAtlas.Count(x => x != null) * (sizeof(float) + 1)) + _dijkstraAtlas.Length;
                byte[] bytes = new byte[bytesCount];
                int byteIndex = 0;
                for (int i = 0; i < _dijkstraAtlas.Length; i++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress.Report((float)i / _dijkstraAtlas.Length);
                    if (_dijkstraAtlas[i] == null)
                    {
                        bytes[byteIndex] = 0;
                        byteIndex++;
                        continue;
                    }
                    else
                    {
                        bytes[byteIndex] = 1;
                        byteIndex++;
                        for (int j = 0; j < _dijkstraAtlas.Length; j++)
                        {
                            bytes[byteIndex] = (byte)_dijkstraAtlas[i]._directionGrid[j];
                            byteIndex++;
                            BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), BitConverter.SingleToInt32Bits(_dijkstraAtlas[i]._distanceGrid[j]));
                            byteIndex += sizeof(float);
                        }
                    }
                }
                return bytes;
            });
            return task;
        }
        /// <summary>
        /// Deserializes a DijkstraAtlas from a byte array. 
        /// </summary>
        /// <param name="grid">The user grid.</param>
        /// <param name="bytes">The serialized DijkstraAtlas.</param>
        /// <returns>The deserialized DijkstraAtlas.</returns>
        public static DijkstraAtlas FromByteArray<T>(T[,] grid, byte[] bytes) where T : ITile
        {
            if (grid == null)
            {
                throw new ArgumentException("The grid cannot be null");
            }
            int byteIndex = 0;
            DijkstraGrid[] dijkstraAtlas = new DijkstraGrid[grid.Length];
            for (int i = 0; i < grid.Length; i++)
            {
                bool isWalkable = bytes[byteIndex] == 1;
                byteIndex++;
                if (!isWalkable)
                {
                    dijkstraAtlas[i] = null;
                    continue;
                }
                else
                {
                    NextTileDirection[] directionGrid = new NextTileDirection[grid.Length];
                    float[] distanceGrid = new float[grid.Length];
                    for (int j = 0; j < grid.Length; j++)
                    {
                        directionGrid[j] = (NextTileDirection)bytes[byteIndex];
                        byteIndex++;
                        distanceGrid[i] = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex)));
                        byteIndex += sizeof(float);
                    }
                    dijkstraAtlas[i] = new DijkstraGrid(directionGrid, distanceGrid, i);
                }
            }
            return new DijkstraAtlas(dijkstraAtlas);
        }
        /// <summary>
        /// Asynchronously deserializes a DijkstraAtlas from a byte array. 
        /// </summary>
        /// <param name="grid">The user grid.</param>
        /// <param name="bytes">The serialized DijkstraAtlas.</param>
        /// <param name="progress">An optional IProgress object to get the deserialization progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the deserialization</param>
        /// <returns>The deserialized DijkstraAtlas.</returns>
        public static Task<DijkstraAtlas> FromByteArrayAsync<T>(T[,] grid, byte[] bytes, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : ITile
        {
            Task<DijkstraAtlas> task = Task.Run(() =>
            {
                if (grid == null)
                {
                    throw new ArgumentException("The grid cannot be null");
                }
                int byteIndex = 0;
                DijkstraGrid[] dijkstraAtlas = new DijkstraGrid[grid.Length];
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
                        dijkstraAtlas[i] = null;
                        continue;
                    }
                    else
                    {
                        NextTileDirection[] directionGrid = new NextTileDirection[grid.Length];
                        float[] distanceGrid = new float[grid.Length];
                        for (int j = 0; j < grid.Length; j++)
                        {
                            directionGrid[j] = (NextTileDirection)bytes[byteIndex];
                            byteIndex++;
                            distanceGrid[i] = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex)));
                            byteIndex += sizeof(float);
                        }
                        dijkstraAtlas[i] = new DijkstraGrid(directionGrid, distanceGrid, i);
                    }
                }
                return new DijkstraAtlas(dijkstraAtlas);
            });
            return task;
        }
    }
}
