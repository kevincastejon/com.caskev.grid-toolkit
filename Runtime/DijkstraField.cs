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
    public class DijkstraField : DijkstraBase
    {
        private float _maxDistance;
        private int[] _accessibleTilesFlatIndexes;
        /// <summary>
        /// Gets the maximum allowable distance for the operation.
        /// </summary>
        public float MaxDistance => _maxDistance;
        /// <summary>
        /// Gets the total number of accessible tiles.
        /// </summary>
        public int AccessibleTilesCount => _accessibleTilesFlatIndexes.Length;
        internal DijkstraField(float maxDistance, int[] accessibleTilesFlatIndexes, NextTileDirection[] directionGrid, float[] distanceMap, int target) : base(directionGrid, distanceMap, target)
        {
            _maxDistance = maxDistance;
            _accessibleTilesFlatIndexes = accessibleTilesFlatIndexes;
        }
        /// <summary>
        /// Retrieves an accessible tile from the specified grid based on the given index.
        /// </summary>
        /// <param name="grid">A two-dimensional array representing the grid of tiles.</param>
        /// <param name="index">The zero-based index of the accessible tile in the accessible tiles list <see cref="AccessibleTilesCount"/>.</param>
        /// <returns>The tile of type <typeparamref name="T"/> located at the specified index in the accessible tiles list.</returns>
        public T GetAccessibleTile<T>(T[,] grid, int index) where T : IWeightedTile
        {
            int flatIndex = _accessibleTilesFlatIndexes[index];
            Vector2Int coords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), flatIndex);
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
            for (int i = 0; i < _accessibleTilesFlatIndexes.Length; i++)
            {
                accessibleTiles[i] = GetAccessibleTile(grid, i);
            }
            return accessibleTiles;
        }
    }
}
