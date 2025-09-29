using UnityEngine;

/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    public class DirectionField : DirectionBase
    {
        private int _maxDistance;
        private int[] _accessibleTilesFlatIndexes;

        public int MaxDistance => _maxDistance;
        public int AccessibleTilesCount => _accessibleTilesFlatIndexes.Length;

        internal DirectionField(int maxDistance, int[] accessibleTilesFlatIndexes, NextTileDirection[] directionMap, int target) : base(directionMap, target)
        {
            _maxDistance = maxDistance;
            _accessibleTilesFlatIndexes = accessibleTilesFlatIndexes;
        }
        public T GetAccessibleTile<T>(T[,] grid, int index) where T : ITile
        {
            int flatIndex = _accessibleTilesFlatIndexes[index];
            Vector2Int coords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), flatIndex);
            return grid[coords.y, coords.x];
        }
    }
}
