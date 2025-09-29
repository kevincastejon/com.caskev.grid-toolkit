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
    /// You can generate a DijkstraMap object that holds pre-calculated paths data.
    /// This way of doing pathfinding is useful for some usages(like Tower Defenses and more) because it calculates once all the paths between one tile, called the "target", and all the accessible tiles from it.
    /// To generate the DijkstraMap object, use the GenerateDijkstraMap method that needs the* grid* and the target tile from which to calculate the paths, as parameters.
    /// <i>Note that, obviously, any path calculation is valid as long as the user grid, and walkable states of the tiles, remains unchanged</i>
    /// </summary>
    /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
    public class DijkstraMap : DijkstraBase
    {
        internal DijkstraMap(NextTileDirection[] directionMap, float[] distanceMap, int target) : base(directionMap, distanceMap, target) { }
        /// <summary>
        /// Returns the DijkstraMap serialized as a byte array.
        /// </summary>
        /// <returns>A byte array representing the serialized DijkstraMap</returns>
        public byte[] ToByteArray()
        {
            int bytesCount = sizeof(int) + sizeof(int) + ((sizeof(byte) + sizeof(float)) * _directionMap.Length);
            byte[] bytes = new byte[bytesCount];
            int byteIndex = 0;
            BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), _target);
            byteIndex += sizeof(int);
            BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), _directionMap.Length);
            byteIndex += sizeof(int);
            for (int i = 0; i < _directionMap.Length; i++)
            {
                bytes[byteIndex] = (byte)_directionMap[i];
                byteIndex++;
                BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), BitConverter.SingleToInt32Bits(_distanceMap[i]));
                byteIndex += sizeof(float);
            }
            return bytes;
        }
        /// <summary>
        /// Returns the DijkstraMap serialized as a byte array.
        /// </summary>
        /// <param name="progress">An optional IProgress object to get the serialization progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the serialization</param>
        /// <returns>A byte array representing the serialized DijkstraMap</returns>
        public Task<byte[]> ToByteArrayAsync(IProgress<float> progress = null, CancellationToken cancelToken = default)
        {
            Task<byte[]> task = Task.Run(() =>
            {
                int bytesCount = sizeof(int) + sizeof(int) + ((sizeof(byte) + sizeof(float)) * _directionMap.Length);
                byte[] bytes = new byte[bytesCount];
                int byteIndex = 0;
                BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), _target);
                byteIndex += sizeof(int);
                BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), _directionMap.Length);
                byteIndex += sizeof(int);
                for (int i = 0; i < _directionMap.Length; i++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress.Report((float)i / _directionMap.Length);
                    bytes[byteIndex] = (byte)_directionMap[i];
                    byteIndex++;
                    BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(byteIndex), BitConverter.SingleToInt32Bits(_distanceMap[i]));
                    byteIndex += sizeof(float);
                }
                return bytes;
            });
            return task;
        }
        /// <summary>
        /// Returns a DijkstraMap from a byte array that has been serialized with the ToByteArray method.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="bytes">The serialized byte array</param>
        /// <returns>The deserialized DijkstraMap</returns>
        public static DijkstraMap FromByteArray<T>(T[,] grid, byte[] bytes) where T : IWeightedTile
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
            NextTileDirection[] directionMap = new NextTileDirection[count];
            float[] distanceMap = new float[count];
            for (int i = 0; i < count; i++)
            {
                directionMap[i] = (NextTileDirection)bytes[byteIndex];
                byteIndex++;
                distanceMap[i] = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex)));
                byteIndex += sizeof(float);
            }
            return new DijkstraMap(directionMap, distanceMap, target);
        }
        /// <summary>
        /// Returns a DijkstraMap from a byte array that has been serialized with the ToByteArray method.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="bytes">The serialized byte array</param>
        /// <param name="progress">An optional IProgress object to get the deserialization progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the deserialization</param>
        /// <returns>The deserialized DijkstraMap</returns>
        public static Task<DijkstraMap> FromByteArrayAsync<T>(T[,] grid, byte[] bytes, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : IWeightedTile
        {
            Task<DijkstraMap> task = Task.Run(() =>
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
                NextTileDirection[] directionMap = new NextTileDirection[count];
                float[] distanceMap = new float[count];
                for (int i = 0; i < count; i++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress.Report((float)i / count);
                    directionMap[i] = (NextTileDirection)bytes[byteIndex];
                    byteIndex++;
                    distanceMap[i] = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(byteIndex)));
                    byteIndex += sizeof(float);
                }
                return new DijkstraMap(directionMap, distanceMap, target);
            });
            return task;
        }
    }
}
