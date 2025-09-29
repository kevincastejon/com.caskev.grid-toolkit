using UnityEngine;

/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    public class DijkstraField : DijkstraBase
    {
        private float _maxDistance;
        private int[] _accessibleTilesFlatIndexes;
        public float MaxDistance => _maxDistance;
        public int AccessibleTilesCount => _accessibleTilesFlatIndexes.Length;
        internal DijkstraField(float maxDistance, int[] accessibleTilesFlatIndexes, NextTileDirection[] directionMap, float[] distanceMap, int target) : base(directionMap, distanceMap, target)
        {
            _maxDistance = maxDistance;
            _accessibleTilesFlatIndexes = accessibleTilesFlatIndexes;
        }
        public T GetAccessibleTile<T>(T[,] grid, int index) where T : IWeightedTile
        {
            int flatIndex = _accessibleTilesFlatIndexes[index];
            Vector2Int coords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), flatIndex);
            return grid[coords.y, coords.x];
        }
    }
}
