using UnityEngine;
/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    /// <summary>
    /// Some utilitary methods
    /// </summary>
    public static class GridUtils
    {
        internal static Vector2Int NextNodeDirectionToVector2Int(NextTileDirection dir)
        {
            switch (dir)
            {
                case NextTileDirection.LEFT:
                    return Vector2Int.left;
                case NextTileDirection.RIGHT:
                    return Vector2Int.right;
                case NextTileDirection.DOWN:
                    return Vector2Int.down;
                case NextTileDirection.UP:
                    return Vector2Int.up;
                case NextTileDirection.UP_LEFT:
                    return new(-1, 1);
                case NextTileDirection.UP_RIGHT:
                    return new(1, 1);
                case NextTileDirection.DOWN_LEFT:
                    return new(-1, -1);
                case NextTileDirection.DOWN_RIGHT:
                    return new(1, -1);
                case NextTileDirection.NONE:
                case NextTileDirection.SELF:
                default:
                    return Vector2Int.zero;
            }
        }
        internal static Vector2Int GetCoordinatesFromFlatIndex(Vector2Int gridDimensions, int flatIndex)
        {
            return new Vector2Int(flatIndex % gridDimensions.y, flatIndex / gridDimensions.y);
        }
        internal static int GetFlatIndexFromCoordinates(Vector2Int gridDimensions, int x, int y)
        {
            return y * gridDimensions.y + x;
        }
        /// <summary>
        /// Gets the direction between two adjacent tiles.
        /// </summary>
        /// <param name="tileA">The first tile</param>
        /// <param name="tileB">The second tile</param>
        /// <returns>The direction from the first tile to the second tile.</returns>
        public static NextTileDirection GetDirectionBetweenAdjacentTiles(ITile tileA, ITile tileB)
        {
            int dx = tileB.X - tileA.X;
            int dy = tileB.Y - tileA.Y;

            if (dx == 0 && dy == 0) return NextTileDirection.SELF;
            if (dx == 0 && dy == -1) return NextTileDirection.DOWN;
            if (dx == 0 && dy == 1) return NextTileDirection.UP;
            if (dx == -1 && dy == 0) return NextTileDirection.LEFT;
            if (dx == 1 && dy == 0) return NextTileDirection.RIGHT;
            if (dx == -1 && dy == -1) return NextTileDirection.DOWN_LEFT;
            if (dx == 1 && dy == -1) return NextTileDirection.DOWN_RIGHT;
            if (dx == -1 && dy == 1) return NextTileDirection.UP_LEFT;
            if (dx == 1 && dy == 1) return NextTileDirection.UP_RIGHT;

            return NextTileDirection.NONE;
        }
        /// <summary>
        /// Compare two tiles to check if they share the same coordinates values
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="tileA">The first tile</param>
        /// <param name="tileB">The second tile</param>
        /// <returns>A boolean value</returns>
        public static bool TileEquals<T>(T tileA, T tileB) where T : ITile
        {
            return tileA.X == tileB.X && tileA.Y == tileB.Y;
        }
        /// <summary>
        /// Return clamped coordinates into the grid
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="grid">A two-dimensional array</param>
        /// <param name="x">Horizontal coordinate to clamp</param>
        /// <param name="y">Vertical coordinate to clamp</param>
        /// <returns>A Vector2Int representing the clamped coordinates</returns>
        public static Vector2Int ClampCoordsIntoGrid<T>(T[,] grid, int x, int y)
        {
            return new Vector2Int(Mathf.Clamp(x, 0, GetHorizontalLength(grid) - 1), Mathf.Clamp(y, 0, GetVerticalLength(grid) - 1));
        }
        /// <summary>
        /// Check if specific coordinates are into the grid range
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="grid">A two-dimensional array</param>
        /// <param name="x">Horizontal coordinate to check</param>
        /// <param name="y">Vertical coordinate to check</param>
        /// <returns>A boolean value</returns>
        public static bool AreCoordsIntoGrid<T>(T[,] grid, int x, int y)
        {
            return x >= 0 && x < GetHorizontalLength(grid) && y >= 0 && y < GetVerticalLength(grid);
        }
        /// <summary>
        /// Returns a tile from the grid at specific coordinates
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="grid">A two-dimensional array</param>
        /// <param name="x">Horizontal coordinate of the tile</param>
        /// <param name="y">Vertical coordinate of the tile</param>
        /// <returns>A tile</returns>
        public static T GetTile<T>(T[,] grid, int x, int y)
        {
            return grid[y, x];
        }
        /// <summary>
        /// Returns the horizontal length of a grid
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="grid">A two-dimensional array</param>
        /// <returns>The horizontal length of a grid</returns>
        public static int GetHorizontalLength<T>(T[,] grid)
        {
            return grid.GetLength(1);
        }
        /// <summary>
        /// Returns the vertical length of a grid
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="grid">A two-dimensional array</param>
        /// <returns>The vertical length of a grid</returns>
        public static int GetVerticalLength<T>(T[,] grid)
        {
            return grid.GetLength(0);
        }
    }
}
