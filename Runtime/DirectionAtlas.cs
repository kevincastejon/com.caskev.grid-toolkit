using Caskev.GridToolkit;
using System;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
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
    public bool GetNextTileFromTile<T>(T[,] grid, T startTile, T destinationTile, out T nextTile) where T : ITile
    {
        if (startTile == null || !startTile.IsWalkable || destinationTile == null || !destinationTile.IsWalkable)
        {
            throw new Exception("Do not call this method with non-walkable (or null) tiles");
        }
        DirectionGrid directionGrid = _directionAtlas[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), destinationTile.X, destinationTile.Y)];
        if (!directionGrid.IsTileAccessible(grid, startTile))
        {
            nextTile = default;
            return false;
        }
        nextTile = directionGrid.GetNextTileFromTile(grid, startTile);
        return true;
    }
    /// <summary>
    /// Get the next tile on the path between a start tile and a destination tile.
    /// </summary>
    /// <param name="grid">The user grid</param>
    /// <param name="startTile">The start tile</param>
    /// <param name="destinationTile">The destination tile</param>
    /// <returns>A Vector2Int direction</returns>
    public NextTileDirection GetNextTileDirectionFromTile<T>(T[,] grid, T startTile, T destinationTile) where T : ITile
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
        int bytesCount = sizeof(int) + _directionAtlas.Length * (sizeof(byte) * _directionAtlas.Length);
        byte[] bytes = new byte[bytesCount];
        int byteIndex = 0;
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), _directionAtlas.Length);
        byteIndex += sizeof(int);
        for (int i = 0; i < _directionAtlas.Length; i++)
        {
            for (int j = 0; j < _directionAtlas.Length; j++)
            {
                bytes[byteIndex] = (byte)_directionAtlas[i]._directionGrid[j];
                byteIndex++;
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
            int bytesCount = sizeof(int) + _directionAtlas.Length * (sizeof(byte) * _directionAtlas.Length);
            byte[] bytes = new byte[bytesCount];
            int byteIndex = 0;
            BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), _directionAtlas.Length);
            byteIndex += sizeof(int);
            for (int i = 0; i < _directionAtlas.Length; i++)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return null;
                }
                progress.Report((float)i / _directionAtlas.Length);
                for (int j = 0; j < _directionAtlas.Length; j++)
                {
                    bytes[byteIndex] = (byte)_directionAtlas[i]._directionGrid[j];
                    byteIndex++;
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
        int count = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex));
        byteIndex += sizeof(int);
        DirectionGrid[] directionAtlas = new DirectionGrid[count];
        for (int i = 0; i < count; i++)
        {
            NextTileDirection[] directionGrid = new NextTileDirection[count];
            for (int j = 0; j < count; j++)
            {
                directionGrid[i] = (NextTileDirection)bytes[byteIndex];
                byteIndex++;
            }
            directionAtlas[i] = new DirectionGrid(directionGrid, i);
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
            int count = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex));
            byteIndex += sizeof(int);
            DirectionGrid[] directionAtlas = new DirectionGrid[count];
            for (int i = 0; i < count; i++)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return null;
                }
                progress.Report((float)i / count);
                NextTileDirection[] directionGrid = new NextTileDirection[count];
                for (int j = 0; j < count; j++)
                {
                    directionGrid[i] = (NextTileDirection)bytes[byteIndex];
                    byteIndex++;
                }
                directionAtlas[i] = new DirectionGrid(directionGrid, i);
            }
            return new DirectionAtlas(directionAtlas);
        });
        return task;
    }
}
