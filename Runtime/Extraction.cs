using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    /// <summary>
    /// Allows you to extract tiles on a grid.<br>Provides shape extraction (rectangles, circles, cones and lines) and neighbors extraction with a lot of parameters.
    /// </summary>
    public class Extraction
    {
        private static T[] ExtractRectangle<T>(T[,] grid, T center, Vector2Int rectangleSize, bool includeCenter, bool includeWalls) where T : ITile
        {
            Vector2Int min = GridUtils.ClampCoordsIntoGrid(grid, center.X + -rectangleSize.x, center.Y - rectangleSize.y);
            Vector2Int max = GridUtils.ClampCoordsIntoGrid(grid, center.X + rectangleSize.x, center.Y + rectangleSize.y);
            List<T> list = new();
            for (int i = min.y; i <= max.y; i++)
            {
                for (int j = min.x; j <= max.x; j++)
                {
                    T tile = GridUtils.GetTile(grid, j, i);
                    if (tile != null && (includeWalls || tile.IsWalkable) && (includeCenter || !GridUtils.TileEquals(tile, center)))
                    {
                        list.Add(tile);
                    }
                }
            }
            return list.ToArray();
        }
        private static T[] ExtractRectangleOutline<T>(T[,] grid, T center, Vector2Int rectangleSize, bool includeWalls) where T : ITile
        {
            Vector2Int min = new(center.X - rectangleSize.x, center.Y - rectangleSize.y);
            Vector2Int max = new(center.X + rectangleSize.x, center.Y + rectangleSize.y);
            List<T> list = new();
            for (int j = min.x; j <= max.x; j++)
            {
                if (GridUtils.AreCoordsIntoGrid(grid, j, min.y))
                {
                    T bottomTile = GridUtils.GetTile(grid, j, min.y);
                    if (bottomTile != null && (includeWalls || bottomTile.IsWalkable))
                    {
                        list.Add(bottomTile);
                    }
                }
                if (GridUtils.AreCoordsIntoGrid(grid, j, max.y))
                {
                    T topTile = GridUtils.GetTile(grid, j, max.y);
                    if (topTile != null && (includeWalls || topTile.IsWalkable))
                    {
                        list.Add(topTile);
                    }
                }
            }
            for (int i = min.y + 1; i <= max.y - 1; i++)
            {
                if (GridUtils.AreCoordsIntoGrid(grid, min.x, i))
                {
                    T leftTile = GridUtils.GetTile(grid, min.x, i);
                    if (leftTile != null && (includeWalls || leftTile.IsWalkable))
                    {
                        list.Add(leftTile);
                    }
                }
                if (GridUtils.AreCoordsIntoGrid(grid, max.x, i))
                {
                    T rightTile = GridUtils.GetTile(grid, max.x, i);
                    if (rightTile != null && (includeWalls || rightTile.IsWalkable))
                    {
                        list.Add(rightTile);
                    }
                }
            }
            return list.ToArray();
        }
        private static T[] ExtractCircleArcFilled<T>(T[,] grid, T center, int radius, bool includeCenter, bool includeWalls, float openingAngle, Vector2 direction) where T : ITile
        {
            int x = 0;
            int y = -radius;
            int F_M = 1 - radius;
            int d_e = 3;
            int d_ne = -(radius << 1) + 5;
            HashSet<T> points = new();
            GetLineMirrors(grid, center, ref points, x, y, openingAngle, direction, includeCenter, includeWalls);
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
                GetLineMirrors(grid, center, ref points, x, y, openingAngle, direction, includeCenter, includeWalls);
            }
            return points.ToArray();
        }
        private static T[] ExtractCircleArcOutline<T>(T[,] grid, T center, int radius, bool includeWalls, float openingAngle, Vector2 direction) where T : ITile
        {
            int x = 0;
            int y = -radius;
            int F_M = 1 - radius;
            int d_e = 3;
            int d_ne = -(radius << 1) + 5;
            T[] firstLineMirror = GetTileMirrors(grid, center, x, y, openingAngle, direction, includeWalls);
            HashSet<T> points = new();
            for (int i = 0; i < firstLineMirror.Length; i++)
            {
                points.Add(firstLineMirror[i]);
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
                T[] mirrorLine = GetTileMirrors(grid, center, x, y, openingAngle, direction, includeWalls);
                for (int i = 0; i < mirrorLine.Length; i++)
                {
                    points.Add(mirrorLine[i]);
                }
            }
            return points.ToArray();
        }
        private static T[] GetTileMirrors<T>(T[,] grid, T centerTile, int x, int y, float openingAngle, Vector2 direction, bool includeWalls) where T : ITile
        {
            List<T> neis = new List<T>();
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X + x, centerTile.Y + y))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X + x, centerTile.Y + y);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X - x, centerTile.Y + y))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X - x, centerTile.Y + y);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X + x, centerTile.Y - y))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X + x, centerTile.Y - y);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X - x, centerTile.Y - y))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X - x, centerTile.Y - y);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X + y, centerTile.Y + x))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X + y, centerTile.Y + x);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X - y, centerTile.Y + x))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X - y, centerTile.Y + x);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X + y, centerTile.Y - x))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X + y, centerTile.Y - x);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X - y, centerTile.Y - x))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X - y, centerTile.Y - x);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            return neis.ToArray();
        }
        private static void GetLineMirrors<T>(T[,] grid, T centerTile, ref HashSet<T> tiles, int x, int y, float openingAngle, Vector2 direction, bool includeCenter, bool includeWalls) where T : ITile
        {
            Vector2Int posLeft = GridUtils.ClampCoordsIntoGrid(grid, x >= 0 ? centerTile.X - x : centerTile.X + x, centerTile.Y + y);
            Vector2Int posRight = GridUtils.ClampCoordsIntoGrid(grid, x >= 0 ? centerTile.X + x : centerTile.X - x, centerTile.Y + y);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(grid, i, posLeft.y);
                if ((includeWalls || nei.IsWalkable) && (includeCenter || !GridUtils.TileEquals(nei, centerTile)) && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    tiles.Add(nei);
                }
            }
            posLeft = GridUtils.ClampCoordsIntoGrid(grid, x >= 0 ? centerTile.X - x : centerTile.X + x, centerTile.Y - y);
            posRight = GridUtils.ClampCoordsIntoGrid(grid, x >= 0 ? centerTile.X + x : centerTile.X - x, centerTile.Y - y);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(grid, i, posLeft.y);
                if ((includeWalls || nei.IsWalkable) && (includeCenter || !GridUtils.TileEquals(nei, centerTile)) && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    tiles.Add(nei);
                }
            }
            posLeft = GridUtils.ClampCoordsIntoGrid(grid, y >= 0 ? centerTile.X - y : centerTile.X + y, centerTile.Y + x);
            posRight = GridUtils.ClampCoordsIntoGrid(grid, y >= 0 ? centerTile.X + y : centerTile.X - y, centerTile.Y + x);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(grid, i, posLeft.y);
                if ((includeWalls || nei.IsWalkable) && (includeCenter || !GridUtils.TileEquals(nei, centerTile)) && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    tiles.Add(nei);
                }
            }
            posLeft = GridUtils.ClampCoordsIntoGrid(grid, y >= 0 ? centerTile.X - y : centerTile.X + y, centerTile.Y - x);
            posRight = GridUtils.ClampCoordsIntoGrid(grid, y >= 0 ? centerTile.X + y : centerTile.X - y, centerTile.Y - x);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(grid, i, posLeft.y);
                if ((includeWalls || nei.IsWalkable) && (includeCenter || !GridUtils.TileEquals(nei, centerTile)) && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    tiles.Add(nei);
                }
            }
        }
        private static bool IsInARectangle<T>(T[,] grid, T tile, T center, Vector2Int rectangleSize) where T : ITile
        {
            Vector2Int min = GridUtils.ClampCoordsIntoGrid(grid, center.X + -rectangleSize.x, center.Y - rectangleSize.y);
            Vector2Int max = GridUtils.ClampCoordsIntoGrid(grid, center.X + rectangleSize.x, center.Y + rectangleSize.y);
            return tile.X >= min.x && tile.X <= max.x && tile.Y >= min.y && tile.Y <= max.y;
        }
        private static bool IsOnRectangleOutline<T>(T[,] grid, T tile, T center, Vector2Int rectangleSize) where T : ITile
        {
            Vector2Int min = GridUtils.ClampCoordsIntoGrid(grid, center.X + -rectangleSize.x, center.Y - rectangleSize.y);
            Vector2Int max = GridUtils.ClampCoordsIntoGrid(grid, center.X + rectangleSize.x, center.Y + rectangleSize.y);
            return (tile.X == min.x && tile.Y <= max.y && tile.Y >= min.y) || (tile.X == max.x && tile.Y <= max.y && tile.Y >= min.y) || (tile.Y == min.y && tile.X <= max.x && tile.X >= min.x) || (tile.Y == max.y && tile.X <= max.x && tile.X >= min.x);
        }
        private static bool IsOnCircleArcFilled<T>(T[,] grid, T tile, T center, int radius, float openingAngle, Vector2 direction) where T : ITile
        {
            int x = 0;
            int y = -radius;
            int F_M = 1 - radius;
            int d_e = 3;
            int d_ne = -(radius << 1) + 5;
            if (IsOnLineMirrors(grid, tile, center, x, y, openingAngle, direction))
            {
                return true;
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
                if (IsOnLineMirrors(grid, tile, center, x, y, openingAngle, direction))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsOnCircleArcOutline<T>(T[,] grid, T tile, T center, int radius, float openingAngle, Vector2 direction) where T : ITile
        {
            int x = 0;
            int y = -radius;
            int F_M = 1 - radius;
            int d_e = 3;
            int d_ne = -(radius << 1) + 5;
            if (IsOneOfTileMirrors(grid, tile, center, x, y, openingAngle, direction))
            {
                return true;
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
                if (IsOneOfTileMirrors(grid, tile, center, x, y, openingAngle, direction))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsOneOfTileMirrors<T>(T[,] grid, T tile, T centerTile, int x, int y, float openingAngle, Vector2 direction) where T : ITile
        {
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X + x, centerTile.Y + y))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X + x, centerTile.Y + y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X - x, centerTile.Y + y))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X - x, centerTile.Y + y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X + x, centerTile.Y - y))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X + x, centerTile.Y - y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X - x, centerTile.Y - y))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X - x, centerTile.Y - y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X + y, centerTile.Y + x))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X + y, centerTile.Y + x);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X - y, centerTile.Y + x))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X - y, centerTile.Y + x);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X + y, centerTile.Y - x))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X + y, centerTile.Y - x);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, centerTile.X - y, centerTile.Y - x))
            {
                T nei = GridUtils.GetTile(grid, centerTile.X - y, centerTile.Y - x);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsOnLineMirrors<T>(T[,] grid, T tile, T centerTile, int x, int y, float openingAngle, Vector2 direction) where T : ITile
        {
            Vector2Int posLeft = GridUtils.ClampCoordsIntoGrid(grid, x >= 0 ? centerTile.X - x : centerTile.X + x, centerTile.Y + y);
            Vector2Int posRight = GridUtils.ClampCoordsIntoGrid(grid, x >= 0 ? centerTile.X + x : centerTile.X - x, centerTile.Y + y);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(grid, i, posLeft.y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            posLeft = GridUtils.ClampCoordsIntoGrid(grid, x >= 0 ? centerTile.X - x : centerTile.X + x, centerTile.Y - y);
            posRight = GridUtils.ClampCoordsIntoGrid(grid, x >= 0 ? centerTile.X + x : centerTile.X - x, centerTile.Y - y);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(grid, i, posLeft.y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            posLeft = GridUtils.ClampCoordsIntoGrid(grid, y >= 0 ? centerTile.X - y : centerTile.X + y, centerTile.Y + x);
            posRight = GridUtils.ClampCoordsIntoGrid(grid, y >= 0 ? centerTile.X + y : centerTile.X - y, centerTile.Y + x);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(grid, i, posLeft.y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            posLeft = GridUtils.ClampCoordsIntoGrid(grid, y >= 0 ? centerTile.X - y : centerTile.X + y, centerTile.Y - x);
            posRight = GridUtils.ClampCoordsIntoGrid(grid, y >= 0 ? centerTile.X + y : centerTile.X - y, centerTile.Y - x);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(grid, i, posLeft.y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            return false;
        }
        internal static bool IsIntoAngle(int tileAX, int tileAY, int tileBX, int tileBY, float openingAngle, Vector2 direction)
        {
            Vector2 realDirection = (new Vector2(tileBX, tileBY) - new Vector2(tileAX, tileAY)).normalized;
            float angleDiff = Vector2.Angle(realDirection, direction.normalized);
            return angleDiff <= openingAngle / 2;
        }

        /// <summary>
        /// Get tiles in a rectangle around a center tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="center">The center tile</param>
        /// <param name="rectangleExtends">The Vector2Int representing the extends of the rectangle from the center</param>
        /// <param name="includeCenter">Include the center tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesInARectangle<T>(T[,] grid, T center, Vector2Int rectangleExtends, bool includeCenter = true, bool includeWalls = true) where T : ITile
        {
            rectangleExtends.x = rectangleExtends.x < 1 ? 1 : rectangleExtends.x;
            rectangleExtends.y = rectangleExtends.y < 1 ? 1 : rectangleExtends.y;
            return ExtractRectangle(grid, center, rectangleExtends, includeCenter, includeWalls);
        }
        /// <summary>
        /// Get tiles on a rectangle outline around a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="center">The center tile</param>
        /// <param name="rectangleExtends">The Vector2Int representing the extends of the rectangle from the center</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesOnARectangleOutline<T>(T[,] grid, T center, Vector2Int rectangleExtends, bool includeWalls = true) where T : ITile
        {
            rectangleExtends.x = rectangleExtends.x < 1 ? 1 : rectangleExtends.x;
            rectangleExtends.y = rectangleExtends.y < 1 ? 1 : rectangleExtends.y;
            return ExtractRectangleOutline(grid, center, rectangleExtends, includeWalls);
        }

        /// <summary>
        /// Get tiles in a circle around a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="center">The center tile</param>
        /// <param name="radius">The circle radius</param>
        /// <param name="includeCenter">Include the center tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesInACircle<T>(T[,] grid, T center, int radius, bool includeCenter = true, bool includeWalls = true) where T : ITile
        {
            radius = radius < 1 ? 1 : radius;
            return ExtractCircleArcFilled(grid, center, radius, includeCenter, includeWalls, 360f, Vector2.right);
        }
        /// <summary>
        /// Get tiles on a circle outline around a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="center">The center tile</param>
        /// <param name="radius">The circle radius</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesOnACircleOutline<T>(T[,] grid, T center, int radius, bool includeWalls = true) where T : ITile
        {
            radius = radius < 1 ? 1 : radius;
            return ExtractCircleArcOutline(grid, center, radius, includeWalls, 360f, Vector2.right);
        }

        /// <summary>
        /// Get tiles in a cone starting from a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="start">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesInACone<T>(T[,] grid, T start, T destinationTile, float openingAngle, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            return GetTilesInACone(grid, start, new Vector2Int(destinationTile.X, destinationTile.Y), openingAngle, includeStart, includeWalls);
        }
        /// <summary>
        /// Get tiles in a cone starting from a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="start">The start tile</param>
        /// <param name="length">The cone length</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="directionAngle">The cone direction angle in degrees. 0 represents a direction pointing to the right in 2D coordinates</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesInACone<T>(T[,] grid, T start, int length, float openingAngle, float directionAngle, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            float radians = directionAngle * Mathf.Deg2Rad;
            float dx = Mathf.Cos(radians);
            float dy = Mathf.Sin(radians);
            return GetTilesInACone(grid, start, length, openingAngle, new Vector2(dx, dy), includeStart, includeWalls);
        }
        /// <summary>
        /// Get tiles in a cone starting from a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="start">The start tile</param>
        /// <param name="length">The cone length</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="direction">The Vector2 representing the cone direction. Note that an 'empty' Vector2 (Vector2.zero) will be treated as Vector2.right</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesInACone<T>(T[,] grid, T start, int length, float openingAngle, Vector2 direction, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            direction.Normalize();
            direction = direction == Vector2.zero ? Vector2.right : direction;
            return ExtractCircleArcFilled(grid, start, length, includeStart, includeWalls, openingAngle, direction);
        }
        /// <summary>
        /// Get tiles in a cone starting from a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="start">The start tile</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesInACone<T>(T[,] grid, T start, Vector2Int endPosition, float openingAngle, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            Vector2 direction = endPosition - new Vector2(start.X, start.Y);
            return ExtractCircleArcFilled(grid, start, Mathf.CeilToInt(direction.magnitude), includeStart, includeWalls, openingAngle, direction);
        }

        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesOnALine<T>(T[,] grid, T startTile, T destinationTile, bool allowDiagonals = true, bool favorVertical = false, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            Vector2Int endPos = new Vector2Int(destinationTile.X, destinationTile.Y);
            return GetTilesOnALine(grid, startTile, endPos, allowDiagonals, favorVertical, includeStart, includeWalls);
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesOnALine<T>(T[,] grid, T startTile, int length, float directionAngle, bool allowDiagonals = true, bool favorVertical = false, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(startTile.X + Mathf.Cos(directionAngle * Mathf.Deg2Rad) * length), Mathf.RoundToInt(startTile.Y + Mathf.Sin(directionAngle * Mathf.Deg2Rad) * length));
            return GetTilesOnALine(grid, startTile, endPos, allowDiagonals, favorVertical, includeStart, includeWalls);
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesOnALine<T>(T[,] grid, T startTile, int length, Vector2 direction, bool allowDiagonals = true, bool favorVertical = false, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPos = Vector2Int.RoundToInt(new Vector2(startTile.X, startTile.Y) + (direction.normalized * length));
            return GetTilesOnALine(grid, startTile, endPos, allowDiagonals, favorVertical, includeStart, includeWalls);
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesOnALine<T>(T[,] grid, T startTile, Vector2Int endPosition, bool allowDiagonals = true, bool favorVertical = false, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            HashSet<T> hashSet = new HashSet<T>();
            Raycasting.Raycast(grid, startTile, endPosition, allowDiagonals, favorVertical, includeStart, false, includeWalls, out bool isClear, ref hashSet);
            return hashSet.ToArray();
        }

        /// <summary>
        /// Get neighbour of a tile if it exists
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="neighbourDirectionAngle">The neighbour direction angle in degrees [0-360]. 0 represents a direction pointing to the right in 2D coordinates</param>
        /// <param name="neighbour">The neighbour of a tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>Returns true if the neighbour exists, false otherwise</returns>
        public static bool GetTileNeighbour<T>(T[,] grid, T tile, float neighbourDirectionAngle, out T neighbour, bool includeWalls = true) where T : ITile
        {
            float radians = neighbourDirectionAngle * Mathf.Deg2Rad;

            float dx = Mathf.Cos(radians);
            float dy = Mathf.Sin(radians);

            Vector2Int direction = new Vector2Int(
                Mathf.RoundToInt(dx),
                Mathf.RoundToInt(dy)
            );
            return GetTileNeighbour(grid, tile, direction, out neighbour, includeWalls);
        }
        /// <summary>
        /// Get neighbour of a tile if it exists
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="neighbourDirection">The direction from the tile to the desired neighbour</param>
        /// <param name="neighbour">The neighbour of a tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>Returns true if the neighbour exists, false otherwise</returns>
        public static bool GetTileNeighbour<T>(T[,] grid, T tile, Vector2Int neighbourDirection, out T neighbour, bool includeWalls = true) where T : ITile
        {
            int x = neighbourDirection.x > 0 ? tile.X + 1 : (neighbourDirection.x < 0 ? tile.X - 1 : tile.X);
            int y = neighbourDirection.y > 0 ? tile.Y + 1 : (neighbourDirection.y < 0 ? tile.Y - 1 : tile.Y);

            if (GridUtils.AreCoordsIntoGrid(grid, x, y))
            {
                T nei = GridUtils.GetTile(grid, x, y);
                if (includeWalls || nei.IsWalkable)
                {
                    neighbour = nei;
                    return true;
                }
            }
            neighbour = default;
            return false;
        }
        /// <summary>
        /// Get the eight neighbours of a tile when they exist
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTileNeighbours<T>(T[,] grid, T tile, bool includeWalls) where T : ITile
        {
            List<T> neis = new List<T>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    if (GridUtils.AreCoordsIntoGrid(grid, tile.X + i, tile.Y + j))
                    {
                        T nei = GridUtils.GetTile(grid, tile.X + i, tile.Y + j);
                        if (includeWalls || nei.IsWalkable)
                        {
                            neis.Add(nei);
                        }
                    }
                }
            }
            return neis.ToArray();
        }
        /// <summary>
        /// Get the four orthogonals neighbours of a tile when they exist
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTileOrthogonalsNeighbours<T>(T[,] grid, T tile, bool includeWalls) where T : ITile
        {
            List<T> neis = new List<T>();
            if (GridUtils.AreCoordsIntoGrid(grid, tile.X - 1, tile.Y + 0))
            {
                T nei = GridUtils.GetTile(grid, tile.X - 1, tile.Y + 0);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, tile.X + 1, tile.Y + 0))
            {
                T nei = GridUtils.GetTile(grid, tile.X + 1, tile.Y + 0);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, tile.X + 0, tile.Y - 1))
            {
                T nei = GridUtils.GetTile(grid, tile.X + 0, tile.Y - 1);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, tile.X + 0, tile.Y + 1))
            {
                T nei = GridUtils.GetTile(grid, tile.X + 0, tile.Y + 1);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            return neis.ToArray();
        }
        /// <summary>
        /// Get the four diagonals neighbours of a tile when they exist
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTileDiagonalsNeighbours<T>(T[,] grid, T tile, bool includeWalls) where T : ITile
        {
            List<T> neis = new List<T>();

            if (GridUtils.AreCoordsIntoGrid(grid, tile.X - 1, tile.Y - 1))
            {
                T nei = GridUtils.GetTile(grid, tile.X - 1, tile.Y - 1);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, tile.X - 1, tile.Y + 1))
            {
                T nei = GridUtils.GetTile(grid, tile.X - 1, tile.Y + 1);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, tile.X + 1, tile.Y - 1))
            {
                T nei = GridUtils.GetTile(grid, tile.X + 1, tile.Y - 1);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(grid, tile.X + 1, tile.Y + 1))
            {
                T nei = GridUtils.GetTile(grid, tile.X + 1, tile.Y + 1);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            return neis.ToArray();
        }

        /// <summary>
        /// Is this tile in a rectangle or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="rectangleExtends">The Vector2Int representing the extends of the rectangle from the center</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileInARectangle<T>(T[,] grid, T tile, T center, Vector2Int rectangleExtends) where T : ITile
        {
            return IsInARectangle(grid, tile, center, rectangleExtends);
        }
        /// <summary>
        /// Is this tile on a rectangle outline or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="rectangleExtends">The Vector2Int representing the extends of the rectangle from the center</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileOnARectangleOutline<T>(T[,] grid, T tile, T center, Vector2Int rectangleExtends) where T : ITile
        {
            return IsOnRectangleOutline(grid, tile, center, rectangleExtends);
        }

        /// <summary>
        /// Is this tile in a circle or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="radius">The circle radius</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileInACircle<T>(T[,] grid, T tile, T center, int radius) where T : ITile
        {
            return IsOnCircleArcFilled(grid, tile, center, radius, 360f, Vector2.right);
        }
        /// <summary>
        /// Is this tile on a circle outline or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="radius">The circle radius</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileOnACircleOutline<T>(T[,] grid, T tile, T center, int radius) where T : ITile
        {
            return IsOnCircleArcOutline(grid, tile, center, radius, 360f, Vector2.right);
        }

        /// <summary>
        /// Is this tile on a cone or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileInACone<T>(T[,] grid, T tile, T center, T destinationTile, float openingAngle) where T : ITile
        {
            return IsTileInACone(grid, tile, center, new Vector2Int(destinationTile.X, destinationTile.Y), openingAngle);
        }
        /// <summary>
        /// Is this tile on a cone or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileInACone<T>(T[,] grid, T tile, T center, Vector2Int endPosition, float openingAngle) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            Vector2 direction = endPosition - new Vector2(center.X, center.Y);
            return IsOnCircleArcFilled(grid, tile, center, Mathf.CeilToInt(direction.magnitude), openingAngle, direction);
        }
        /// <summary>
        /// Is this tile on a cone or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="length">The length of the cone</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="directionAngle">The cone direction angle in degrees. 0 represents a direction pointing to the right in 2D coordinates</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileInACone<T>(T[,] grid, T tile, T center, int length, float openingAngle, float directionAngle) where T : ITile
        {
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            float radians = directionAngle * Mathf.Deg2Rad;
            float dx = Mathf.Cos(radians);
            float dy = Mathf.Sin(radians);
            return IsOnCircleArcFilled(grid, tile, center, length, openingAngle, new Vector2(dx, dy));
        }
        /// <summary>
        /// Is this tile on a cone or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="length">The length of the cone</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="direction">The Vector2 representing the cone direction. Note that an 'empty' Vector2 (Vector2.zero) will be treated as Vector2.right</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileInACone<T>(T[,] grid, T tile, T center, int length, float openingAngle, Vector2 direction) where T : ITile
        {
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            direction.Normalize();
            direction = direction == Vector2.zero ? Vector2.right : direction;
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            return IsOnCircleArcFilled(grid, tile, center, length, openingAngle, direction);
        }

        /// <summary>
        /// Is a tile on a line
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="start">The start tile of the line</param>
        /// <param name="destinationTile">The destination tile of the line</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileOnALine<T>(T[,] grid, T tile, T start, T destinationTile, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            Vector2Int endPosition = new Vector2Int(destinationTile.X, destinationTile.Y);
            return IsTileOnALine(grid, start, tile, endPosition, allowDiagonals, favorVertical);
        }
        /// <summary>
        /// Is a tile on a line
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="start">The start tile of the line</param>
        /// <param name="length">The length of the line</param>
        /// <param name="directionAngle">The cone direction angle in degrees. 0 represents a direction pointing to the right in 2D coordinates</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileOnALine<T>(T[,] grid, T tile, T start, int length, float directionAngle, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPosition = new Vector2Int(Mathf.RoundToInt(start.X + Mathf.Cos(directionAngle * Mathf.Deg2Rad) * length), Mathf.RoundToInt(start.Y + Mathf.Sin(directionAngle * Mathf.Deg2Rad) * length));
            return IsTileOnALine(grid, start, tile, endPosition, allowDiagonals, favorVertical);
        }
        /// <summary>
        /// Is a tile on a line
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="start">The center tile of the rectangle</param>
        /// <param name="length">The length of the line</param>
        /// <param name="direction">The Vector2 representing the cone direction. Note that an 'empty' Vector2 (Vector2.zero) will be treated as Vector2.right</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileOnALine<T>(T[,] grid, T tile, T start, int length, Vector2 direction, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            float magnitude = new Vector2Int(grid.GetLength(0), grid.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPosition = Vector2Int.RoundToInt(new Vector2(start.X, start.Y) + (direction.normalized * length));
            return IsTileOnALine(grid, start, tile, endPosition, allowDiagonals, favorVertical);
        }
        /// <summary>
        /// Is a tile on a line
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="start">The start tile of the line</param>
        /// <param name="endPosition">The line destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileOnALine<T>(T[,] grid, T start, T tile, Vector2Int endPosition, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            return Raycasting.IsTileOnALine(grid, start, tile, endPosition, allowDiagonals, favorVertical, false);
        }

        /// <summary>
        /// Is a tile the neighbour of another tile with the given direction.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="neighbour">The tile to check as a neighbour</param>
        /// <param name="center">A tile</param>
        /// <param name="neighbourDirectionAngle">The cone direction angle in degrees  [0-360]. 0 represents a direction pointing to the right in 2D coordinates</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileNeighbor<T>(T neighbour, T center, float neighbourDirectionAngle, bool includeWalls = true) where T : ITile
        {
            float radians = neighbourDirectionAngle * Mathf.Deg2Rad;

            float dx = Mathf.Cos(radians);
            float dy = Mathf.Sin(radians);

            Vector2Int direction = new Vector2Int(
                Mathf.RoundToInt(dx),
                Mathf.RoundToInt(dy)
            );
            return IsTileNeighbor(neighbour, center, direction, includeWalls);
        }
        /// <summary>
        /// Is a tile the neighbour of another tile with the given direction.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="neighbour">The tile to check as a neighbour</param>
        /// <param name="center">A tile</param>
        /// <param name="neighbourDirection">The position of the expected neighbour from the tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileNeighbor<T>(T neighbour, T center, Vector2Int neighbourDirection, bool includeWalls = true) where T : ITile
        {
            if (!includeWalls && (neighbour == null || !neighbour.IsWalkable))
            {
                return false;
            }
            int x = neighbourDirection.x > 0 ? center.X + 1 : (neighbourDirection.x < 0 ? center.X - 1 : center.X);
            int y = neighbourDirection.y > 0 ? center.Y + 1 : (neighbourDirection.y < 0 ? center.Y - 1 : center.Y);
            return neighbour.X == x && neighbour.Y == y;
        }
        /// <summary>
        /// Is a tile an orthogonal neighbour of another tile.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="neighbour">The tile to check as a neighbour</param>
        /// <param name="center">A tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileOrthogonalNeighbor<T>(T neighbour, T center, bool includeWalls = true) where T : ITile
        {
            if (!includeWalls && (neighbour == null || !neighbour.IsWalkable))
            {
                return false;
            }
            return (center.X == neighbour.X && (center.Y == neighbour.Y + 1 || center.Y == neighbour.Y - 1)) || center.Y == neighbour.Y && (center.X == neighbour.X + 1 || center.X == neighbour.X - 1);
        }
        /// <summary>
        /// Is a tile an diagonal neighbour of another tile.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="neighbour">The tile to check as a neighbour</param>
        /// <param name="center">A tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileDiagonalNeighbor<T>(T neighbour, T center, bool includeWalls = true) where T : ITile
        {
            if (!includeWalls && (neighbour == null || !neighbour.IsWalkable))
            {
                return false;
            }
            return Mathf.Abs(neighbour.X - center.X) == 1 && Mathf.Abs(neighbour.Y - center.Y) == 1;
        }
        /// <summary>
        /// Is a tile any neighbour of another tile.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="neighbour">The tile to check as a neighbour</param>
        /// <param name="center">A tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileAnyNeighbor<T>(T neighbour, T center, bool includeWalls = true) where T : ITile
        {
            if (!includeWalls && (neighbour == null || !neighbour.IsWalkable))
            {
                return false;
            }
            return IsTileOrthogonalNeighbor(center, neighbour) || IsTileDiagonalNeighbor(center, neighbour);
        }
    }
}
