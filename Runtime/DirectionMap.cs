using System;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using static UnityEngine.GraphicsBuffer;
/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    /// <summary>
    /// A DirectionMap will hold all the pre-calculated direction data between a target tile and all the tiles that are accessible to this target.  
    /// Once generated, this object can contain all the paths you need (ie: a tower defense game with a village core where all enemies run to) and then use the paths with almost no performance cost.  
    /// There are also serialization methods to bake or save these objects to files and load them later with the deserialization methods.
    /// </summary>
    /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
    public class DirectionMap : DirectionBase
    {
        internal DirectionMap(NextTileDirection[] directionMap, int target) : base(directionMap, target) { }
        /// <summary>
        /// Returns the DirectionMap serialized as a byte array.
        /// </summary>
        /// <returns>A byte array representing the serialized DirectionMap</returns>
        public byte[] ToByteArray()
        {
            int bytesCount = sizeof(int) + sizeof(int) + sizeof(byte) * _directionMap.Length;
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
            }
            return bytes;
        }
        /// <summary>
        /// Returns the DirectionMap serialized as a byte array.
        /// </summary>
        /// <param name="progress">An optional IProgress object to get the serialization progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the serialization</param>
        /// <returns>A byte array representing the serialized DirectionMap</returns>
        public Task<byte[]> ToByteArrayAsync(IProgress<float> progress = null, CancellationToken cancelToken = default)
        {
            Task<byte[]> task = Task.Run(() =>
            {
                int bytesCount = sizeof(int) + sizeof(int) + sizeof(byte) * _directionMap.Length;
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
                }
                return bytes;
            });
            return task;
        }
        /// <summary>
        /// Returns a DirectionMap from a byte array that has been serialized with the ToByteArray method.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="bytes">The serialized byte array</param>
        /// <returns>The deserialized DirectionMap</returns>
        public static DirectionMap FromByteArray<T>(T[,] grid, byte[] bytes) where T : ITile
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
            for (int i = 0; i < count; i++)
            {
                directionMap[i] = (NextTileDirection)bytes[byteIndex];
                byteIndex++;
            }
            return new DirectionMap(directionMap, target);
        }
        /// <summary>
        /// Returns a DirectionMap from a byte array that has been serialized with the ToByteArray method.
        /// </summary>
        /// <param name="grid">The user grid</param>
        /// <param name="bytes">The serialized byte array</param>
        /// <param name="progress">An optional IProgress object to get the deserialization progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the deserialization</param>
        /// <returns>The deserialized DirectionMap</returns>
        public static Task<DirectionMap> FromByteArrayAsync<T>(T[,] grid, byte[] bytes, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : ITile
        {
            Task<DirectionMap> task = Task.Run(() =>
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
                for (int i = 0; i < count; i++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress.Report((float)i / count);
                    directionMap[i] = (NextTileDirection)bytes[byteIndex];
                    byteIndex++;
                }
                return new DirectionMap(directionMap, target);
            });
            return task;
        }
    }
}
