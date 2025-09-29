using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    /// <summary>
    /// Allows you to cast lines of sight and cones of vision on a grid
    /// </summary>
    public class Raycasting
    {
        internal static bool IsTileOnALine<T>(T[,] grid, T startTile, T tile, Vector2Int endPosition, bool allowDiagonals, bool favorVertical, bool breakOnWalls) where T : ITile
        {
            if (GridUtils.TileEquals(startTile, tile))
            {
                return true;
            }
            Vector2Int p0 = new Vector2Int(startTile.X, startTile.Y);
            Vector2Int p1 = endPosition;
            int dx = p1.x - p0.x;
            int dy = p1.y - p0.y;
            int nx = Mathf.Abs(dx);
            int ny = Mathf.Abs(dy);
            int sign_x = dx > 0 ? 1 : -1, sign_y = dy > 0 ? 1 : -1;

            Vector2Int p = new Vector2Int(p0.x, p0.y);
            for (int ix = 0, iy = 0; ix < nx || iy < ny;)
            {
                int decision = (1 + 2 * ix) * ny - (1 + 2 * iy) * nx;
                if (!allowDiagonals && decision == 0)
                {
                    decision = favorVertical ? 1 : -1;
                }
                if (decision == 0)
                {
                    // next step is diagonal
                    p.x += sign_x;
                    p.y += sign_y;
                    ix++;
                    iy++;
                }
                else if (decision < 0)
                {
                    // next step is horizontal
                    p.x += sign_x;
                    ix++;
                }
                else
                {
                    // next step is vertical
                    p.y += sign_y;
                    iy++;
                }
                bool isNextTileIntoGrid = GridUtils.AreCoordsIntoGrid(grid, p.x, p.y);
                if (!isNextTileIntoGrid)
                {
                    break;
                }
                T nextTile = GridUtils.GetTile(grid, p.x, p.y);
                bool isNextTileWalkable = nextTile != null && nextTile.IsWalkable;
                if (breakOnWalls && (nextTile == null || !nextTile.IsWalkable))
                {
                    break;
                }
                if (nextTile == null)
                {
                    continue;
                }
                if (GridUtils.TileEquals(tile, nextTile))
                {
                    return true;
                }
            }
            return false;
        }
        internal static void Raycast<T>(T[,] grid, T startTile, Vector2Int endPosition, bool allowDiagonals, bool favorVertical, bool includeStart, bool breakOnWalls, bool includeWalls, out bool isClear, ref HashSet<T> results) where T : ITile
        {
            Vector2Int p0 = new Vector2Int(startTile.X, startTile.Y);
            Vector2Int p1 = endPosition;
            int dx = p1.x - p0.x;
            int dy = p1.y - p0.y;
            int nx = Mathf.Abs(dx);
            int ny = Mathf.Abs(dy);
            int sign_x = dx > 0 ? 1 : -1, sign_y = dy > 0 ? 1 : -1;

            Vector2Int p = new Vector2Int(p0.x, p0.y);
            if (includeStart)
            {
                results.Add(GridUtils.GetTile(grid, p.x, p.y));
            }
            isClear = true;
            for (int ix = 0, iy = 0; ix < nx || iy < ny;)
            {
                int decision = (1 + 2 * ix) * ny - (1 + 2 * iy) * nx;
                if (!allowDiagonals && decision == 0)
                {
                    decision = favorVertical ? 1 : -1;
                }
                if (decision == 0)
                {
                    // next step is diagonal
                    p.x += sign_x;
                    p.y += sign_y;
                    ix++;
                    iy++;
                }
                else if (decision < 0)
                {
                    // next step is horizontal
                    p.x += sign_x;
                    ix++;
                }
                else
                {
                    // next step is vertical
                    p.y += sign_y;
                    iy++;
                }
                bool isNextTileIntoGrid = GridUtils.AreCoordsIntoGrid(grid, p.x, p.y);
                if (!isNextTileIntoGrid)
                {
                    break;
                }
                T tile = GridUtils.GetTile(grid, p.x, p.y);
                bool isNextTileWalkable = tile != null && tile.IsWalkable;
                if (!isNextTileWalkable)
                {
                    isClear = false;
                }
                if (breakOnWalls && (tile == null || !tile.IsWalkable))
                {
                    break;
                }
                if (tile == null || !includeWalls && !tile.IsWalkable)
                {
                    continue;
                }
                results.Add(GridUtils.GetTile(grid, p.x, p.y));
            }
        }
        private static void ConeCast<T>(T[,] grid, T center, int radius, float openingAngle, Vector2 direction, ref bool isClear, bool includeStart, ref HashSet<T> resultList) where T : ITile
        {
            bool lineClear = true;
            direction.Normalize();
            int x = 0;
            int y = -radius;
            int F_M = 1 - radius;
            int d_e = 3;
            int d_ne = -(radius << 1) + 5;
            RaycastToMirrorPositions(grid, center, x, y, openingAngle, direction, ref lineClear, includeStart, ref resultList);
            if (!lineClear)
            {
                isClear = false;
            }
            while (x < -y)
            {
                if (F_M <= 0)
                {
                    F_M += d_e;
                }
                else
                {
                    F_M += d_ne;
                    d_ne += 2;
                    y += 1;
                }
                d_e += 2;
                d_ne += 2;
                x += 1;
                RaycastToMirrorPositions(grid, center, x, y, openingAngle, direction, ref isClear, includeStart, ref resultList);
                if (!lineClear)
                {
                    isClear = false;
                }
            }
        }
        private static void RaycastToMirrorPositions<T>(T[,] grid, T centerTile, int x, int y, float openingAngle, Vector2 direction, ref bool isClear, bool includeStart, ref HashSet<T> resultList) where T : ITile
        {
            bool lineClear = true;
            Vector2Int nei = new Vector2Int(centerTile.X + x, centerTile.Y + y);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(grid, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
            }
            if (!lineClear)
            {
                isClear = false;
            }
            nei = new Vector2Int(centerTile.X - x, centerTile.Y + y);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(grid, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
            }
            if (!lineClear)
            {
                isClear = false;
            }
            nei = new Vector2Int(centerTile.X + x, centerTile.Y - y);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(grid, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
                if (!lineClear)
                {
                    isClear = false;
                }
            }
            nei = new Vector2Int(centerTile.X - x, centerTile.Y - y);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(grid, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
                if (!lineClear)
                {
                    isClear = false;
                }
            }
            nei = new Vector2Int(centerTile.X + y, centerTile.Y + x);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(grid, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
                if (!lineClear)
                {
                    isClear = false;
                }
            }
            nei = new Vector2Int(centerTile.X - y, centerTile.Y + x);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(grid, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
                if (!lineClear)
                {
                    isClear = false;
                }
            }
            nei = new Vector2Int(centerTile.X + y, centerTile.Y - x);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(grid, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
                if (!lineClear)
                {
                    isClear = false;
                }
            }
            nei = new Vector2Int(centerTile.X - y, centerTile.Y - x);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(grid, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
                if (!lineClear)
                {
                    isClear = false;
                }
            }
        }

        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsLineOfSightClear<T>(T[,] grid, T startTile, T destinationTile, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            GetLineOfSight(grid, out bool isClear, startTile, destinationTile, allowDiagonals, favorVertical, false);
            return isClear;
        }
        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsLineOfSightClear<T>(T[,] grid, T startTile, int length, float directionAngle, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            GetLineOfSight(grid, out bool isClear, startTile, length, directionAngle, allowDiagonals, favorVertical, false);
            return isClear;
        }
        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsLineOfSightClear<T>(T[,] grid, T startTile, int length, Vector2 direction, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            GetLineOfSight(grid, out bool isClear, startTile, length, direction, allowDiagonals, favorVertical, false);
            return isClear;
        }
        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsLineOfSightClear<T>(T[,] grid, T startTile, Vector2Int endPosition, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            GetLineOfSight(grid, out bool isClear, startTile, endPosition, allowDiagonals, favorVertical, false);
            return isClear;
        }

        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <returns>A boolean value</returns>
        public static bool IsConeOfVisionClear<T>(T[,] grid, T startTile, float openingAngle, T destinationTile) where T : ITile
        {
            GetConeOfVision(grid, out bool clear, startTile, openingAngle, destinationTile, true);
            return clear;
        }
        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the cone</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <returns>A boolean value</returns>
        public static bool IsConeOfVisionClear<T>(T[,] grid, T startTile, int length, float openingAngle, float directionAngle) where T : ITile
        {
            GetConeOfVision(grid, out bool clear, startTile, length, openingAngle, directionAngle, true);
            return clear;
        }
        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the cone</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <returns>A boolean value</returns>
        public static bool IsConeOfVisionClear<T>(T[,] grid, T startTile, int length, float openingAngle, Vector2 direction) where T : ITile
        {
            GetConeOfVision(grid, out bool clear, startTile, length, openingAngle, direction, true);
            return clear;
        }
        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <returns>A boolean value</returns>
        public static bool IsConeOfVisionClear<T>(T[,] grid, T startTile, float openingAngle, Vector2Int endPosition) where T : ITile
        {
            GetConeOfVision(grid, out bool clear, startTile, openingAngle, endPosition, true);
            return clear;
        }

        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>A boolean value</returns>
        public static T[] GetLineOfSight<T>(T[,] grid, T startTile, T destinationTile, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            Vector2Int endPos = new Vector2Int(destinationTile.X, destinationTile.Y);
            return GetLineOfSight(grid, startTile, endPos, allowDiagonals, favorVertical, includeStart);
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetLineOfSight<T>(T[,] grid, T startTile, int length, float directionAngle, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(startTile.X + Mathf.Cos(directionAngle * Mathf.Deg2Rad) * length), Mathf.RoundToInt(startTile.Y + Mathf.Sin(directionAngle * Mathf.Deg2Rad) * length));
            return GetLineOfSight(grid, startTile, endPos, allowDiagonals, favorVertical, includeStart);
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetLineOfSight<T>(T[,] grid, T startTile, int length, Vector2 direction, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPos = Vector2Int.RoundToInt(new Vector2(startTile.X, startTile.Y) + (direction.normalized * length));
            return GetLineOfSight(grid, startTile, endPos, allowDiagonals, favorVertical, includeStart);
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetLineOfSight<T>(T[,] grid, T startTile, Vector2Int endPosition, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            HashSet<T> hashSet = new HashSet<T>();
            Raycast(grid, startTile, endPosition, allowDiagonals, favorVertical, includeStart, true, false, out bool isClear, ref hashSet);
            return hashSet.ToArray();
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>A boolean value</returns>
        public static T[] GetLineOfSight<T>(T[,] grid, out bool isClear, T startTile, T destinationTile, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            Vector2Int endPos = new Vector2Int(destinationTile.X, destinationTile.Y);
            return GetLineOfSight(grid, out isClear, startTile, endPos, allowDiagonals, favorVertical, includeStart);
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetLineOfSight<T>(T[,] grid, out bool isClear, T startTile, int length, float directionAngle, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(startTile.X + Mathf.Cos(directionAngle * Mathf.Deg2Rad) * length), Mathf.RoundToInt(startTile.Y + Mathf.Sin(directionAngle * Mathf.Deg2Rad) * length));
            return GetLineOfSight(grid, out isClear, startTile, endPos, allowDiagonals, favorVertical, includeStart);
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetLineOfSight<T>(T[,] grid, out bool isClear, T startTile, int length, Vector2 direction, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPos = Vector2Int.RoundToInt(new Vector2(startTile.X, startTile.Y) + (direction.normalized * length));
            return GetLineOfSight(grid, out isClear, startTile, endPos, allowDiagonals, favorVertical, includeStart);
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetLineOfSight<T>(T[,] grid, out bool isClear, T startTile, Vector2Int endPosition, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            HashSet<T> hashSet = new HashSet<T>();
            Raycast(grid, startTile, endPosition, allowDiagonals, favorVertical, includeStart, true, false, out isClear, ref hashSet);
            return hashSet.ToArray();
        }

        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="destinationTile">The destination tile at the end of the cone</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] grid, T startTile, float openingAngle, T destinationTile, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            Vector2Int startPos = new Vector2Int(startTile.X, startTile.Y);
            Vector2Int endPos = new Vector2Int(destinationTile.X, destinationTile.Y);
            Vector2 direction = endPos - startPos;
            HashSet<T> lines = new HashSet<T>();
            bool isClear = true;
            ConeCast(grid, startTile, Mathf.CeilToInt(direction.magnitude), openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] grid, T startTile, int length, float openingAngle, float directionAngle, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int startPos = new Vector2Int(startTile.X, startTile.Y);
            Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(startTile.X + Mathf.Cos(directionAngle * Mathf.Deg2Rad) * length), Mathf.RoundToInt(startTile.Y + Mathf.Sin(directionAngle * Mathf.Deg2Rad) * length));
            Vector2 direction = endPos - startPos;
            HashSet<T> lines = new HashSet<T>();
            bool isClear = true;
            ConeCast(grid, startTile, length, openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] grid, T startTile, int length, float openingAngle, Vector2 direction, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            HashSet<T> lines = new HashSet<T>();
            bool isClear = true;
            ConeCast(grid, startTile, length, openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] grid, T startTile, float openingAngle, Vector2Int endPosition, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            Vector2Int startPos = new Vector2Int(startTile.X, startTile.Y);
            Vector2 direction = endPosition - startPos;
            HashSet<T> lines = new HashSet<T>();
            bool isClear = true;
            ConeCast(grid, startTile, Mathf.FloorToInt(direction.magnitude), openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="destinationTile">The destination tile at the end of the cone</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] grid, out bool isClear, T startTile, float openingAngle, T destinationTile, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            Vector2Int startPos = new Vector2Int(startTile.X, startTile.Y);
            Vector2Int endPos = new Vector2Int(destinationTile.X, destinationTile.Y);
            int radius = Mathf.CeilToInt(Vector2Int.Distance(startPos, endPos));
            Vector2 direction = endPos - startPos;
            HashSet<T> lines = new HashSet<T>();
            isClear = true;
            ConeCast(grid, startTile, Mathf.FloorToInt(direction.magnitude), openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] grid, out bool isClear, T startTile, int length, float openingAngle, float directionAngle, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int startPos = new Vector2Int(startTile.X, startTile.Y);
            Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(startTile.X + Mathf.Cos(directionAngle * Mathf.Deg2Rad) * length), Mathf.RoundToInt(startTile.Y + Mathf.Sin(directionAngle * Mathf.Deg2Rad) * length));
            Vector2 direction = endPos - startPos;
            HashSet<T> lines = new HashSet<T>();
            isClear = true;
            ConeCast(grid, startTile, length, openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] grid, out bool isClear, T startTile, int length, float openingAngle, Vector2 direction, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            HashSet<T> lines = new HashSet<T>();
            isClear = true;
            ConeCast(grid, startTile, length, openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] grid, out bool isClear, T startTile, float openingAngle, Vector2Int endPosition, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            Vector2Int startPos = new Vector2Int(startTile.X, startTile.Y);
            Vector2 direction = endPosition - startPos;
            HashSet<T> lines = new HashSet<T>();
            isClear = true;
            ConeCast(grid, startTile, Mathf.CeilToInt(direction.magnitude), openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
    }
}
