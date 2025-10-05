using System;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    /// <summary>
    /// A DijkstraGrid holds both direction and distance data between a target tile and all the tiles that are accessible to this target.  
    /// Once generated, this object can contain all the paths and distance data that you need (ie: a tower defense game with a village core where all enemies run to, or a strategy game in which you would display the distance cost of the movement by hovering tiles with the cursor) and then use the paths with almost no performance cost.  
    /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
    /// </summary>
    public class DijkstraGrid : DijkstraBase
    {
        internal DijkstraGrid(NextTileDirection[] directionGrid, float[] distanceMap, int target) : base(directionGrid, distanceMap, target) { }
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
                BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), BitConverter.SingleToInt32Bits(_distanceMap[i]));
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
                    BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), BitConverter.SingleToInt32Bits(_distanceMap[i]));
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
            NextTileDirection[] directionGrid = new NextTileDirection[count];
            float[] distanceMap = new float[count];
            for (int i = 0; i < count; i++)
            {
                directionGrid[i] = (NextTileDirection)bytes[byteIndex];
                byteIndex++;
                distanceMap[i] = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex)));
                byteIndex += sizeof(float);
            }
            return new DijkstraGrid(directionGrid, distanceMap, target);
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
                NextTileDirection[] directionGrid = new NextTileDirection[count];
                float[] distanceMap = new float[count];
                for (int i = 0; i < count; i++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress.Report((float)i / count);
                    directionGrid[i] = (NextTileDirection)bytes[byteIndex];
                    byteIndex++;
                    distanceMap[i] = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex)));
                    byteIndex += sizeof(float);
                }
                return new DijkstraGrid(directionGrid, distanceMap, target);
            });
            return task;
        }
    }
}
