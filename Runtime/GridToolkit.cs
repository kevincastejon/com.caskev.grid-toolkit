using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Utilitary API to proceed operations on abstract grids such as tile extraction, raycasting, and pathfinding.
/// </summary>
namespace Caskev.GridToolkit
{
    public enum NextTileDirection : byte
    {
        NONE,
        SELF,
        LEFT,
        RIGHT,
        DOWN,
        UP,
        UP_LEFT,
        UP_RIGHT,
        DOWN_LEFT,
        DOWN_RIGHT,
    }
    /// <summary>
    /// An interface that the user-defined tile object has to implement in order to work with most of this library's methods
    /// </summary>
    public interface ITile
    {
        /// <summary>
        /// Is the tile walkable (or "transparent" for line of sight and cone of vision methods)
        /// </summary>
        public bool IsWalkable
        {
            get;
        }
        /// <summary>
        /// The tile horizontal coordinate
        /// </summary>
        public int X
        {
            get;
        }
        /// <summary>
        /// The tile vertical coordinate
        /// </summary>
        public int Y
        {
            get;
        }
    }
    /// <summary>
    /// Allows you to extract tiles on a grid.<br>Provides shape extraction (rectangles, circles, cones and lines) and neighbors extraction with a lot of parameters.
    /// </summary>
    public class Extraction
    {
        private static T[] ExtractRectangle<T>(T[,] map, T center, Vector2Int rectangleSize, bool includeCenter, bool includeWalls) where T : ITile
        {
            Vector2Int min = GridUtils.ClampCoordsIntoGrid(map, center.X + -rectangleSize.x, center.Y - rectangleSize.y);
            Vector2Int max = GridUtils.ClampCoordsIntoGrid(map, center.X + rectangleSize.x, center.Y + rectangleSize.y);
            List<T> list = new();
            for (int i = min.y; i <= max.y; i++)
            {
                for (int j = min.x; j <= max.x; j++)
                {
                    T tile = GridUtils.GetTile(map, j, i);
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
        private static T[] ExtractCircleArcFilled<T>(T[,] map, T center, int radius, bool includeCenter, bool includeWalls, float openingAngle, Vector2 direction) where T : ITile
        {
            int x = 0;
            int y = -radius;
            int F_M = 1 - radius;
            int d_e = 3;
            int d_ne = -(radius << 1) + 5;
            HashSet<T> points = new();
            GetLineMirrors(map, center, ref points, x, y, openingAngle, direction, includeCenter, includeWalls);
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
                GetLineMirrors(map, center, ref points, x, y, openingAngle, direction, includeCenter, includeWalls);
            }
            return points.ToArray();
        }
        private static T[] ExtractCircleArcOutline<T>(T[,] map, T center, int radius, bool includeWalls, float openingAngle, Vector2 direction) where T : ITile
        {
            int x = 0;
            int y = -radius;
            int F_M = 1 - radius;
            int d_e = 3;
            int d_ne = -(radius << 1) + 5;
            T[] firstLineMirror = GetTileMirrors(map, center, x, y, openingAngle, direction, includeWalls);
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
                T[] mirrorLine = GetTileMirrors(map, center, x, y, openingAngle, direction, includeWalls);
                for (int i = 0; i < mirrorLine.Length; i++)
                {
                    points.Add(mirrorLine[i]);
                }
            }
            return points.ToArray();
        }
        private static T[] GetTileMirrors<T>(T[,] map, T centerTile, int x, int y, float openingAngle, Vector2 direction, bool includeWalls) where T : ITile
        {
            List<T> neis = new List<T>();
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X + x, centerTile.Y + y))
            {
                T nei = GridUtils.GetTile(map, centerTile.X + x, centerTile.Y + y);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X - x, centerTile.Y + y))
            {
                T nei = GridUtils.GetTile(map, centerTile.X - x, centerTile.Y + y);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X + x, centerTile.Y - y))
            {
                T nei = GridUtils.GetTile(map, centerTile.X + x, centerTile.Y - y);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X - x, centerTile.Y - y))
            {
                T nei = GridUtils.GetTile(map, centerTile.X - x, centerTile.Y - y);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X + y, centerTile.Y + x))
            {
                T nei = GridUtils.GetTile(map, centerTile.X + y, centerTile.Y + x);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X - y, centerTile.Y + x))
            {
                T nei = GridUtils.GetTile(map, centerTile.X - y, centerTile.Y + x);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X + y, centerTile.Y - x))
            {
                T nei = GridUtils.GetTile(map, centerTile.X + y, centerTile.Y - x);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X - y, centerTile.Y - x))
            {
                T nei = GridUtils.GetTile(map, centerTile.X - y, centerTile.Y - x);
                if (includeWalls || nei.IsWalkable && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    neis.Add(nei);
                }
            }
            return neis.ToArray();
        }
        private static void GetLineMirrors<T>(T[,] map, T centerTile, ref HashSet<T> tiles, int x, int y, float openingAngle, Vector2 direction, bool includeCenter, bool includeWalls) where T : ITile
        {
            Vector2Int posLeft = GridUtils.ClampCoordsIntoGrid(map, x >= 0 ? centerTile.X - x : centerTile.X + x, centerTile.Y + y);
            Vector2Int posRight = GridUtils.ClampCoordsIntoGrid(map, x >= 0 ? centerTile.X + x : centerTile.X - x, centerTile.Y + y);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(map, i, posLeft.y);
                if ((includeWalls || nei.IsWalkable) && (includeCenter || !GridUtils.TileEquals(nei, centerTile)) && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    tiles.Add(nei);
                }
            }
            posLeft = GridUtils.ClampCoordsIntoGrid(map, x >= 0 ? centerTile.X - x : centerTile.X + x, centerTile.Y - y);
            posRight = GridUtils.ClampCoordsIntoGrid(map, x >= 0 ? centerTile.X + x : centerTile.X - x, centerTile.Y - y);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(map, i, posLeft.y);
                if ((includeWalls || nei.IsWalkable) && (includeCenter || !GridUtils.TileEquals(nei, centerTile)) && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    tiles.Add(nei);
                }
            }
            posLeft = GridUtils.ClampCoordsIntoGrid(map, y >= 0 ? centerTile.X - y : centerTile.X + y, centerTile.Y + x);
            posRight = GridUtils.ClampCoordsIntoGrid(map, y >= 0 ? centerTile.X + y : centerTile.X - y, centerTile.Y + x);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(map, i, posLeft.y);
                if ((includeWalls || nei.IsWalkable) && (includeCenter || !GridUtils.TileEquals(nei, centerTile)) && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    tiles.Add(nei);
                }
            }
            posLeft = GridUtils.ClampCoordsIntoGrid(map, y >= 0 ? centerTile.X - y : centerTile.X + y, centerTile.Y - x);
            posRight = GridUtils.ClampCoordsIntoGrid(map, y >= 0 ? centerTile.X + y : centerTile.X - y, centerTile.Y - x);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(map, i, posLeft.y);
                if ((includeWalls || nei.IsWalkable) && (includeCenter || !GridUtils.TileEquals(nei, centerTile)) && IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction))
                {
                    tiles.Add(nei);
                }
            }
        }
        private static bool IsInARectangle<T>(T[,] map, T tile, T center, Vector2Int rectangleSize) where T : ITile
        {
            Vector2Int min = GridUtils.ClampCoordsIntoGrid(map, center.X + -rectangleSize.x, center.Y - rectangleSize.y);
            Vector2Int max = GridUtils.ClampCoordsIntoGrid(map, center.X + rectangleSize.x, center.Y + rectangleSize.y);
            return tile.X >= min.x && tile.X <= max.x && tile.Y >= min.y && tile.Y <= max.y;
        }
        private static bool IsOnRectangleOutline<T>(T[,] map, T tile, T center, Vector2Int rectangleSize) where T : ITile
        {
            Vector2Int min = GridUtils.ClampCoordsIntoGrid(map, center.X + -rectangleSize.x, center.Y - rectangleSize.y);
            Vector2Int max = GridUtils.ClampCoordsIntoGrid(map, center.X + rectangleSize.x, center.Y + rectangleSize.y);
            return (tile.X == min.x && tile.Y <= max.y && tile.Y >= min.y) || (tile.X == max.x && tile.Y <= max.y && tile.Y >= min.y) || (tile.Y == min.y && tile.X <= max.x && tile.X >= min.x) || (tile.Y == max.y && tile.X <= max.x && tile.X >= min.x);
        }
        private static bool IsOnCircleArcFilled<T>(T[,] map, T tile, T center, int radius, float openingAngle, Vector2 direction) where T : ITile
        {
            int x = 0;
            int y = -radius;
            int F_M = 1 - radius;
            int d_e = 3;
            int d_ne = -(radius << 1) + 5;
            if (IsOnLineMirrors(map, tile, center, x, y, openingAngle, direction))
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
                if (IsOnLineMirrors(map, tile, center, x, y, openingAngle, direction))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsOnCircleArcOutline<T>(T[,] map, T tile, T center, int radius, float openingAngle, Vector2 direction) where T : ITile
        {
            int x = 0;
            int y = -radius;
            int F_M = 1 - radius;
            int d_e = 3;
            int d_ne = -(radius << 1) + 5;
            if (IsOneOfTileMirrors(map, tile, center, x, y, openingAngle, direction))
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
                if (IsOneOfTileMirrors(map, tile, center, x, y, openingAngle, direction))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsOneOfTileMirrors<T>(T[,] map, T tile, T centerTile, int x, int y, float openingAngle, Vector2 direction) where T : ITile
        {
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X + x, centerTile.Y + y))
            {
                T nei = GridUtils.GetTile(map, centerTile.X + x, centerTile.Y + y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X - x, centerTile.Y + y))
            {
                T nei = GridUtils.GetTile(map, centerTile.X - x, centerTile.Y + y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X + x, centerTile.Y - y))
            {
                T nei = GridUtils.GetTile(map, centerTile.X + x, centerTile.Y - y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X - x, centerTile.Y - y))
            {
                T nei = GridUtils.GetTile(map, centerTile.X - x, centerTile.Y - y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X + y, centerTile.Y + x))
            {
                T nei = GridUtils.GetTile(map, centerTile.X + y, centerTile.Y + x);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X - y, centerTile.Y + x))
            {
                T nei = GridUtils.GetTile(map, centerTile.X - y, centerTile.Y + x);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X + y, centerTile.Y - x))
            {
                T nei = GridUtils.GetTile(map, centerTile.X + y, centerTile.Y - x);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, centerTile.X - y, centerTile.Y - x))
            {
                T nei = GridUtils.GetTile(map, centerTile.X - y, centerTile.Y - x);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsOnLineMirrors<T>(T[,] map, T tile, T centerTile, int x, int y, float openingAngle, Vector2 direction) where T : ITile
        {
            Vector2Int posLeft = GridUtils.ClampCoordsIntoGrid(map, x >= 0 ? centerTile.X - x : centerTile.X + x, centerTile.Y + y);
            Vector2Int posRight = GridUtils.ClampCoordsIntoGrid(map, x >= 0 ? centerTile.X + x : centerTile.X - x, centerTile.Y + y);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(map, i, posLeft.y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            posLeft = GridUtils.ClampCoordsIntoGrid(map, x >= 0 ? centerTile.X - x : centerTile.X + x, centerTile.Y - y);
            posRight = GridUtils.ClampCoordsIntoGrid(map, x >= 0 ? centerTile.X + x : centerTile.X - x, centerTile.Y - y);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(map, i, posLeft.y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            posLeft = GridUtils.ClampCoordsIntoGrid(map, y >= 0 ? centerTile.X - y : centerTile.X + y, centerTile.Y + x);
            posRight = GridUtils.ClampCoordsIntoGrid(map, y >= 0 ? centerTile.X + y : centerTile.X - y, centerTile.Y + x);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(map, i, posLeft.y);
                if (IsIntoAngle(centerTile.X, centerTile.Y, nei.X, nei.Y, openingAngle, direction) && GridUtils.TileEquals(nei, tile))
                {
                    return true;
                }
            }
            posLeft = GridUtils.ClampCoordsIntoGrid(map, y >= 0 ? centerTile.X - y : centerTile.X + y, centerTile.Y - x);
            posRight = GridUtils.ClampCoordsIntoGrid(map, y >= 0 ? centerTile.X + y : centerTile.X - y, centerTile.Y - x);
            for (int i = posLeft.x; i <= posRight.x; i++)
            {
                T nei = GridUtils.GetTile(map, i, posLeft.y);
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
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="center">The center tile</param>
        /// <param name="rectangleExtends">The Vector2Int representing the extends of the rectangle from the center</param>
        /// <param name="includeCenter">Include the center tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesInARectangle<T>(T[,] map, T center, Vector2Int rectangleExtends, bool includeCenter = true, bool includeWalls = true) where T : ITile
        {
            rectangleExtends.x = rectangleExtends.x < 1 ? 1 : rectangleExtends.x;
            rectangleExtends.y = rectangleExtends.y < 1 ? 1 : rectangleExtends.y;
            return ExtractRectangle(map, center, rectangleExtends, includeCenter, includeWalls);
        }
        /// <summary>
        /// Get tiles on a rectangle outline around a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="center">The center tile</param>
        /// <param name="rectangleExtends">The Vector2Int representing the extends of the rectangle from the center</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesOnARectangleOutline<T>(T[,] map, T center, Vector2Int rectangleExtends, bool includeWalls = true) where T : ITile
        {
            rectangleExtends.x = rectangleExtends.x < 1 ? 1 : rectangleExtends.x;
            rectangleExtends.y = rectangleExtends.y < 1 ? 1 : rectangleExtends.y;
            return ExtractRectangleOutline(map, center, rectangleExtends, includeWalls);
        }

        /// <summary>
        /// Get tiles in a circle around a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="center">The center tile</param>
        /// <param name="radius">The circle radius</param>
        /// <param name="includeCenter">Include the center tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesInACircle<T>(T[,] map, T center, int radius, bool includeCenter = true, bool includeWalls = true) where T : ITile
        {
            radius = radius < 1 ? 1 : radius;
            return ExtractCircleArcFilled(map, center, radius, includeCenter, includeWalls, 360f, Vector2.right);
        }
        /// <summary>
        /// Get tiles on a circle outline around a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="center">The center tile</param>
        /// <param name="radius">The circle radius</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesOnACircleOutline<T>(T[,] map, T center, int radius, bool includeWalls = true) where T : ITile
        {
            radius = radius < 1 ? 1 : radius;
            return ExtractCircleArcOutline(map, center, radius, includeWalls, 360f, Vector2.right);
        }

        /// <summary>
        /// Get tiles in a cone starting from a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="start">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesInACone<T>(T[,] map, T start, T destinationTile, float openingAngle, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            return GetTilesInACone(map, start, new Vector2Int(destinationTile.X, destinationTile.Y), openingAngle, includeStart, includeWalls);
        }
        /// <summary>
        /// Get tiles in a cone starting from a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="start">The start tile</param>
        /// <param name="length">The cone length</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="directionAngle">The cone direction angle in degrees. 0 represents a direction pointing to the right in 2D coordinates</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesInACone<T>(T[,] map, T start, int length, float openingAngle, float directionAngle, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            float radians = directionAngle * Mathf.Deg2Rad;
            float dx = Mathf.Cos(radians);
            float dy = Mathf.Sin(radians);
            return GetTilesInACone(map, start, length, openingAngle, new Vector2(dx, dy), includeStart, includeWalls);
        }
        /// <summary>
        /// Get tiles in a cone starting from a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="start">The start tile</param>
        /// <param name="length">The cone length</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="direction">The Vector2 representing the cone direction. Note that an 'empty' Vector2 (Vector2.zero) will be treated as Vector2.right</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesInACone<T>(T[,] map, T start, int length, float openingAngle, Vector2 direction, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            direction.Normalize();
            direction = direction == Vector2.zero ? Vector2.right : direction;
            return ExtractCircleArcFilled(map, start, length, includeStart, includeWalls, openingAngle, direction);
        }
        /// <summary>
        /// Get tiles in a cone starting from a tile.<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="start">The start tile</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesInACone<T>(T[,] map, T start, Vector2Int endPosition, float openingAngle, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            Vector2 direction = endPosition - new Vector2(start.X, start.Y);
            return ExtractCircleArcFilled(map, start, Mathf.CeilToInt(direction.magnitude), includeStart, includeWalls, openingAngle, direction);
        }

        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesOnALine<T>(T[,] map, T startTile, T destinationTile, bool allowDiagonals = true, bool favorVertical = false, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            Vector2Int endPos = new Vector2Int(destinationTile.X, destinationTile.Y);
            return GetTilesOnALine(map, startTile, endPos, allowDiagonals, favorVertical, includeStart, includeWalls);
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeDestination">Include the destination tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesOnALine<T>(T[,] map, T startTile, int length, float directionAngle, bool allowDiagonals = true, bool favorVertical = false, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(startTile.X + Mathf.Cos(directionAngle * Mathf.Deg2Rad) * length), Mathf.RoundToInt(startTile.Y + Mathf.Sin(directionAngle * Mathf.Deg2Rad) * length));
            return GetTilesOnALine(map, startTile, endPos, allowDiagonals, favorVertical, includeStart, includeWalls);
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeDestination">Include the destination tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesOnALine<T>(T[,] map, T startTile, int length, Vector2 direction, bool allowDiagonals = true, bool favorVertical = false, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPos = Vector2Int.RoundToInt(new Vector2(startTile.X, startTile.Y) + (direction.normalized * length));
            return GetTilesOnALine(map, startTile, endPos, allowDiagonals, favorVertical, includeStart, includeWalls);
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeDestination">Include the destination tile into the resulting array or not. Default is true</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTilesOnALine<T>(T[,] map, T startTile, Vector2Int endPosition, bool allowDiagonals = true, bool favorVertical = false, bool includeStart = true, bool includeWalls = true) where T : ITile
        {
            HashSet<T> hashSet = new HashSet<T>();
            Raycasting.Raycast(map, startTile, endPosition, allowDiagonals, favorVertical, includeStart, false, includeWalls, out bool isClear, ref hashSet);
            return hashSet.ToArray();
        }

        /// <summary>
        /// Get neighbour of a tile if it exists
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="neighbourDirectionAngle">The neighbour direction angle in degrees [0-360]. 0 represents a direction pointing to the right in 2D coordinates</param>
        /// <param name="neighbour">The neighbour of a tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>Returns true if the neighbour exists, false otherwise</returns>
        public static bool GetTileNeighbour<T>(T[,] map, T tile, float neighbourDirectionAngle, out T neighbour, bool includeWalls = true) where T : ITile
        {
            float radians = neighbourDirectionAngle * Mathf.Deg2Rad;

            float dx = Mathf.Cos(radians);
            float dy = Mathf.Sin(radians);

            Vector2Int direction = new Vector2Int(
                Mathf.RoundToInt(dx),
                Mathf.RoundToInt(dy)
            );
            return GetTileNeighbour(map, tile, direction, out neighbour, includeWalls);
        }
        /// <summary>
        /// Get neighbour of a tile if it exists
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="neighbourDirection">The direction from the tile to the desired neighbour</param>
        /// <param name="neighbour">The neighbour of a tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default is true</param>
        /// <returns>Returns true if the neighbour exists, false otherwise</returns>
        public static bool GetTileNeighbour<T>(T[,] map, T tile, Vector2Int neighbourDirection, out T neighbour, bool includeWalls = true) where T : ITile
        {
            int x = neighbourDirection.x > 0 ? tile.X + 1 : (neighbourDirection.x < 0 ? tile.X - 1 : tile.X);
            int y = neighbourDirection.y > 0 ? tile.Y + 1 : (neighbourDirection.y < 0 ? tile.Y - 1 : tile.Y);

            if (GridUtils.AreCoordsIntoGrid(map, x, y))
            {
                T nei = GridUtils.GetTile(map, x, y);
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
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTileNeighbours<T>(T[,] map, T tile, bool includeWalls) where T : ITile
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
                    if (GridUtils.AreCoordsIntoGrid(map, tile.X + i, tile.Y + j))
                    {
                        T nei = GridUtils.GetTile(map, tile.X + i, tile.Y + j);
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
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTileOrthogonalsNeighbours<T>(T[,] map, T tile, bool includeWalls) where T : ITile
        {
            List<T> neis = new List<T>();
            if (GridUtils.AreCoordsIntoGrid(map, tile.X - 1, tile.Y + 0))
            {
                T nei = GridUtils.GetTile(map, tile.X - 1, tile.Y + 0);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, tile.X + 1, tile.Y + 0))
            {
                T nei = GridUtils.GetTile(map, tile.X + 1, tile.Y + 0);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, tile.X + 0, tile.Y - 1))
            {
                T nei = GridUtils.GetTile(map, tile.X + 0, tile.Y - 1);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, tile.X + 0, tile.Y + 1))
            {
                T nei = GridUtils.GetTile(map, tile.X + 0, tile.Y + 1);
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
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="includeWalls">Include the non-walkable tiles into the resulting array or not. Default true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetTileDiagonalsNeighbours<T>(T[,] map, T tile, bool includeWalls) where T : ITile
        {
            List<T> neis = new List<T>();

            if (GridUtils.AreCoordsIntoGrid(map, tile.X - 1, tile.Y - 1))
            {
                T nei = GridUtils.GetTile(map, tile.X - 1, tile.Y - 1);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, tile.X - 1, tile.Y + 1))
            {
                T nei = GridUtils.GetTile(map, tile.X - 1, tile.Y + 1);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, tile.X + 1, tile.Y - 1))
            {
                T nei = GridUtils.GetTile(map, tile.X + 1, tile.Y - 1);
                if (includeWalls || nei.IsWalkable)
                {
                    neis.Add(nei);
                }
            }
            if (GridUtils.AreCoordsIntoGrid(map, tile.X + 1, tile.Y + 1))
            {
                T nei = GridUtils.GetTile(map, tile.X + 1, tile.Y + 1);
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
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="rectangleExtends">The Vector2Int representing the extends of the rectangle from the center</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileInARectangle<T>(T[,] map, T tile, T center, Vector2Int rectangleExtends) where T : ITile
        {
            return IsInARectangle(map, tile, center, rectangleExtends);
        }
        /// <summary>
        /// Is this tile on a rectangle outline or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="rectangleExtends">The Vector2Int representing the extends of the rectangle from the center</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileOnARectangleOutline<T>(T[,] map, T tile, T center, Vector2Int rectangleExtends) where T : ITile
        {
            return IsOnRectangleOutline(map, tile, center, rectangleExtends);
        }

        /// <summary>
        /// Is this tile in a circle or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="radius">The circle radius</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileInACircle<T>(T[,] map, T tile, T center, int radius) where T : ITile
        {
            return IsOnCircleArcFilled(map, tile, center, radius, 360f, Vector2.right);
        }
        /// <summary>
        /// Is this tile on a circle outline or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="radius">The circle radius</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileOnACircleOutline<T>(T[,] map, T tile, T center, int radius) where T : ITile
        {
            return IsOnCircleArcOutline(map, tile, center, radius, 360f, Vector2.right);
        }

        /// <summary>
        /// Is this tile on a cone or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileInACone<T>(T[,] map, T tile, T center, T destinationTile, float openingAngle) where T : ITile
        {
            return IsTileInACone(map, tile, center, new Vector2Int(destinationTile.X, destinationTile.Y), openingAngle);
        }
        /// <summary>
        /// Is this tile on a cone or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileInACone<T>(T[,] map, T tile, T center, Vector2Int endPosition, float openingAngle) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            Vector2 direction = endPosition - new Vector2(center.X, center.Y);
            return IsOnCircleArcFilled(map, tile, center, Mathf.CeilToInt(direction.magnitude), openingAngle, direction);
        }
        /// <summary>
        /// Is this tile on a cone or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="length">The length of the cone</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="directionAngle">The cone direction angle in degrees. 0 represents a direction pointing to the right in 2D coordinates</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileInACone<T>(T[,] map, T tile, T center, int length, float openingAngle, float directionAngle) where T : ITile
        {
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            float radians = directionAngle * Mathf.Deg2Rad;
            float dx = Mathf.Cos(radians);
            float dy = Mathf.Sin(radians);
            return IsOnCircleArcFilled(map, tile, center, length, openingAngle, new Vector2(dx, dy));
        }
        /// <summary>
        /// Is this tile on a cone or not.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="center">The center tile of the rectangle</param>
        /// <param name="length">The length of the cone</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="direction">The Vector2 representing the cone direction. Note that an 'empty' Vector2 (Vector2.zero) will be treated as Vector2.right</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileInACone<T>(T[,] map, T tile, T center, int length, float openingAngle, Vector2 direction) where T : ITile
        {
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            direction.Normalize();
            direction = direction == Vector2.zero ? Vector2.right : direction;
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            return IsOnCircleArcFilled(map, tile, center, length, openingAngle, direction);
        }

        /// <summary>
        /// Is a tile on a line
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <param name="start">The start tile of the line</param>
        /// <param name="destinationTile">The destination tile of the line</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileOnALine<T>(T[,] map, T tile, T start, T destinationTile, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            Vector2Int endPosition = new Vector2Int(destinationTile.X, destinationTile.Y);
            return IsTileOnALine(map, start, tile, endPosition, allowDiagonals, favorVertical);
        }
        /// <summary>
        /// Is a tile on a line
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="tile">A tile</param>
        /// <param name="start">The start tile of the line</param>
        /// <param name="length">The length of the line</param>
        /// <param name="directionAngle">The cone direction angle in degrees. 0 represents a direction pointing to the right in 2D coordinates</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileOnALine<T>(T[,] map, T tile, T start, int length, float directionAngle, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPosition = new Vector2Int(Mathf.RoundToInt(start.X + Mathf.Cos(directionAngle * Mathf.Deg2Rad) * length), Mathf.RoundToInt(start.Y + Mathf.Sin(directionAngle * Mathf.Deg2Rad) * length));
            return IsTileOnALine(map, start, tile, endPosition, allowDiagonals, favorVertical);
        }
        /// <summary>
        /// Is a tile on a line
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="tile">A tile</param>
        /// <param name="start">The center tile of the rectangle</param>
        /// <param name="length">The length of the line</param>
        /// <param name="direction">The Vector2 representing the cone direction. Note that an 'empty' Vector2 (Vector2.zero) will be treated as Vector2.right</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileOnALine<T>(T[,] map, T tile, T start, int length, Vector2 direction, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPosition = Vector2Int.RoundToInt(new Vector2(start.X, start.Y) + (direction.normalized * length));
            return IsTileOnALine(map, start, tile, endPosition, allowDiagonals, favorVertical);
        }
        /// <summary>
        /// Is a tile on a line
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="tile">A tile</param>
        /// <param name="start">The start tile of the line</param>
        /// <param name="endPosition">The line destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsTileOnALine<T>(T[,] map, T start, T tile, Vector2Int endPosition, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            return Raycasting.IsTileOnALine(map, start, tile, endPosition, allowDiagonals, favorVertical, false);
        }

        /// <summary>
        /// Is a tile the neighbour of another tile with the given direction.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="neighbour">The tile to check as a neighbour</param>
        /// <param name="center">A tile</param>
        /// <param name="neighbourDirectionAngle">The cone direction angle in degrees  [0-360]. 0 represents a direction pointing to the right in 2D coordinates</param>
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
    /// <summary>
    /// Allows you to cast lines of sight and cones of vision on a grid
    /// </summary>
    public class Raycasting
    {
        internal static bool IsTileOnALine<T>(T[,] map, T startTile, T tile, Vector2Int endPosition, bool allowDiagonals, bool favorVertical, bool breakOnWalls) where T : ITile
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
                bool isNextTileIntoGrid = GridUtils.AreCoordsIntoGrid(map, p.x, p.y);
                if (!isNextTileIntoGrid)
                {
                    break;
                }
                T nextTile = GridUtils.GetTile(map, p.x, p.y);
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
        internal static void Raycast<T>(T[,] map, T startTile, Vector2Int endPosition, bool allowDiagonals, bool favorVertical, bool includeStart, bool breakOnWalls, bool includeWalls, out bool isClear, ref HashSet<T> results) where T : ITile
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
                results.Add(GridUtils.GetTile(map, p.x, p.y));
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
                bool isNextTileIntoGrid = GridUtils.AreCoordsIntoGrid(map, p.x, p.y);
                if (!isNextTileIntoGrid)
                {
                    break;
                }
                T tile = GridUtils.GetTile(map, p.x, p.y);
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
                results.Add(GridUtils.GetTile(map, p.x, p.y));
            }
        }
        private static void ConeCast<T>(T[,] map, T center, int radius, float openingAngle, Vector2 direction, ref bool isClear, bool includeStart, ref HashSet<T> resultList) where T : ITile
        {
            bool lineClear = true;
            direction.Normalize();
            int x = 0;
            int y = -radius;
            int F_M = 1 - radius;
            int d_e = 3;
            int d_ne = -(radius << 1) + 5;
            RaycastToMirrorPositions(map, center, x, y, openingAngle, direction, ref lineClear, includeStart, ref resultList);
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
                RaycastToMirrorPositions(map, center, x, y, openingAngle, direction, ref isClear, includeStart, ref resultList);
                if (!lineClear)
                {
                    isClear = false;
                }
            }
        }
        private static void RaycastToMirrorPositions<T>(T[,] map, T centerTile, int x, int y, float openingAngle, Vector2 direction, ref bool isClear, bool includeStart, ref HashSet<T> resultList) where T : ITile
        {
            bool lineClear = true;
            Vector2Int nei = new Vector2Int(centerTile.X + x, centerTile.Y + y);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(map, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
            }
            if (!lineClear)
            {
                isClear = false;
            }
            nei = new Vector2Int(centerTile.X - x, centerTile.Y + y);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(map, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
            }
            if (!lineClear)
            {
                isClear = false;
            }
            nei = new Vector2Int(centerTile.X + x, centerTile.Y - y);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(map, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
                if (!lineClear)
                {
                    isClear = false;
                }
            }
            nei = new Vector2Int(centerTile.X - x, centerTile.Y - y);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(map, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
                if (!lineClear)
                {
                    isClear = false;
                }
            }
            nei = new Vector2Int(centerTile.X + y, centerTile.Y + x);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(map, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
                if (!lineClear)
                {
                    isClear = false;
                }
            }
            nei = new Vector2Int(centerTile.X - y, centerTile.Y + x);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(map, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
                if (!lineClear)
                {
                    isClear = false;
                }
            }
            nei = new Vector2Int(centerTile.X + y, centerTile.Y - x);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(map, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
                if (!lineClear)
                {
                    isClear = false;
                }
            }
            nei = new Vector2Int(centerTile.X - y, centerTile.Y - x);
            if (Extraction.IsIntoAngle(centerTile.X, centerTile.Y, nei.x, nei.y, openingAngle, direction))
            {
                Raycast(map, centerTile, new Vector2Int(nei.x, nei.y), true, false, includeStart, true, true, out lineClear, ref resultList);
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
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsLineOfSightClear<T>(T[,] map, T startTile, T destinationTile, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            GetLineOfSight(map, out bool isClear, startTile, destinationTile, allowDiagonals, favorVertical, false);
            return isClear;
        }
        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsLineOfSightClear<T>(T[,] map, T startTile, int length, float directionAngle, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            GetLineOfSight(map, out bool isClear, startTile, length, directionAngle, allowDiagonals, favorVertical, false);
            return isClear;
        }
        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsLineOfSightClear<T>(T[,] map, T startTile, int length, Vector2 direction, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            GetLineOfSight(map, out bool isClear, startTile, length, direction, allowDiagonals, favorVertical, false);
            return isClear;
        }
        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <returns>A boolean value</returns>
        public static bool IsLineOfSightClear<T>(T[,] map, T startTile, Vector2Int endPosition, bool allowDiagonals = true, bool favorVertical = false) where T : ITile
        {
            GetLineOfSight(map, out bool isClear, startTile, endPosition, allowDiagonals, favorVertical, false);
            return isClear;
        }

        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <returns>A boolean value</returns>
        public static bool IsConeOfVisionClear<T>(T[,] map, T startTile, float openingAngle, T destinationTile) where T : ITile
        {
            GetConeOfVision(map, out bool clear, startTile, openingAngle, destinationTile, true);
            return clear;
        }
        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the cone</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <returns>A boolean value</returns>
        public static bool IsConeOfVisionClear<T>(T[,] map, T startTile, int length, float openingAngle, float directionAngle) where T : ITile
        {
            GetConeOfVision(map, out bool clear, startTile, length, openingAngle, directionAngle, true);
            return clear;
        }
        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the cone</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <returns>A boolean value</returns>
        public static bool IsConeOfVisionClear<T>(T[,] map, T startTile, int length, float openingAngle, Vector2 direction) where T : ITile
        {
            GetConeOfVision(map, out bool clear, startTile, length, openingAngle, direction, true);
            return clear;
        }
        /// <summary>
        /// Is the line of sight clear between two tiles
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <returns>A boolean value</returns>
        public static bool IsConeOfVisionClear<T>(T[,] map, T startTile, float openingAngle, Vector2Int endPosition) where T : ITile
        {
            GetConeOfVision(map, out bool clear, startTile, openingAngle, endPosition, true);
            return clear;
        }

        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeDestination">Include the destination tile into the resulting array or not. Default is true</param>
        /// <returns>A boolean value</returns>
        public static T[] GetLineOfSight<T>(T[,] map, T startTile, T destinationTile, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            Vector2Int endPos = new Vector2Int(destinationTile.X, destinationTile.Y);
            return GetLineOfSight(map, startTile, endPos, allowDiagonals, favorVertical, includeStart);
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetLineOfSight<T>(T[,] map, T startTile, int length, float directionAngle, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(startTile.X + Mathf.Cos(directionAngle * Mathf.Deg2Rad) * length), Mathf.RoundToInt(startTile.Y + Mathf.Sin(directionAngle * Mathf.Deg2Rad) * length));
            return GetLineOfSight(map, startTile, endPos, allowDiagonals, favorVertical, includeStart);
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetLineOfSight<T>(T[,] map, T startTile, int length, Vector2 direction, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPos = Vector2Int.RoundToInt(new Vector2(startTile.X, startTile.Y) + (direction.normalized * length));
            return GetLineOfSight(map, startTile, endPos, allowDiagonals, favorVertical, includeStart);
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetLineOfSight<T>(T[,] map, T startTile, Vector2Int endPosition, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            HashSet<T> hashSet = new HashSet<T>();
            Raycast(map, startTile, endPosition, allowDiagonals, favorVertical, includeStart, true, false, out bool isClear, ref hashSet);
            return hashSet.ToArray();
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>A boolean value</returns>
        public static T[] GetLineOfSight<T>(T[,] map, out bool isClear, T startTile, T destinationTile, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            Vector2Int endPos = new Vector2Int(destinationTile.X, destinationTile.Y);
            return GetLineOfSight(map, out isClear, startTile, endPos, allowDiagonals, favorVertical, includeStart);
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetLineOfSight<T>(T[,] map, out bool isClear, T startTile, int length, float directionAngle, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(startTile.X + Mathf.Cos(directionAngle * Mathf.Deg2Rad) * length), Mathf.RoundToInt(startTile.Y + Mathf.Sin(directionAngle * Mathf.Deg2Rad) * length));
            return GetLineOfSight(map, out isClear, startTile, endPos, allowDiagonals, favorVertical, includeStart);
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeDestination">Include the destination tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetLineOfSight<T>(T[,] map, out bool isClear, T startTile, int length, Vector2 direction, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int endPos = Vector2Int.RoundToInt(new Vector2(startTile.X, startTile.Y) + (direction.normalized * length));
            return GetLineOfSight(map, out isClear, startTile, endPos, allowDiagonals, favorVertical, includeStart);
        }
        /// <summary>
        /// Get all tiles on a line of sight from a start tile.<br/>
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="allowDiagonals">Allows the diagonals or not. Default is true</param>
        /// <param name="favorVertical">If diagonals are disabled then favor vertical when a diagonal should have been used. False will favor horizontal and is the default value.</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeDestination">Include the destination tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetLineOfSight<T>(T[,] map, out bool isClear, T startTile, Vector2Int endPosition, bool allowDiagonals = false, bool favorVertical = false, bool includeStart = true) where T : ITile
        {
            HashSet<T> hashSet = new HashSet<T>();
            Raycast(map, startTile, endPosition, allowDiagonals, favorVertical, includeStart, true, false, out isClear, ref hashSet);
            return hashSet.ToArray();
        }

        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="destinationTile">The destination tile at the end of the cone</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] map, T startTile, float openingAngle, T destinationTile, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            Vector2Int startPos = new Vector2Int(startTile.X, startTile.Y);
            Vector2Int endPos = new Vector2Int(destinationTile.X, destinationTile.Y);
            Vector2 direction = endPos - startPos;
            HashSet<T> lines = new HashSet<T>();
            bool isClear = true;
            ConeCast(map, startTile, Mathf.CeilToInt(direction.magnitude), openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] map, T startTile, int length, float openingAngle, float directionAngle, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int startPos = new Vector2Int(startTile.X, startTile.Y);
            Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(startTile.X + Mathf.Cos(directionAngle * Mathf.Deg2Rad) * length), Mathf.RoundToInt(startTile.Y + Mathf.Sin(directionAngle * Mathf.Deg2Rad) * length));
            Vector2 direction = endPos - startPos;
            HashSet<T> lines = new HashSet<T>();
            bool isClear = true;
            ConeCast(map, startTile, length, openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] map, T startTile, int length, float openingAngle, Vector2 direction, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            HashSet<T> lines = new HashSet<T>();
            bool isClear = true;
            ConeCast(map, startTile, length, openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] map, T startTile, float openingAngle, Vector2Int endPosition, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            Vector2Int startPos = new Vector2Int(startTile.X, startTile.Y);
            Vector2 direction = endPosition - startPos;
            HashSet<T> lines = new HashSet<T>();
            bool isClear = true;
            ConeCast(map, startTile, Mathf.FloorToInt(direction.magnitude), openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="destinationTile">The destination tile at the end of the cone</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] map, out bool isClear, T startTile, float openingAngle, T destinationTile, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            Vector2Int startPos = new Vector2Int(startTile.X, startTile.Y);
            Vector2Int endPos = new Vector2Int(destinationTile.X, destinationTile.Y);
            int radius = Mathf.CeilToInt(Vector2Int.Distance(startPos, endPos));
            Vector2 direction = endPos - startPos;
            HashSet<T> lines = new HashSet<T>();
            isClear = true;
            ConeCast(map, startTile, Mathf.FloorToInt(direction.magnitude), openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="directionAngle">The angle of the line from the start tile</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] map, out bool isClear, T startTile, int length, float openingAngle, float directionAngle, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            Vector2Int startPos = new Vector2Int(startTile.X, startTile.Y);
            Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(startTile.X + Mathf.Cos(directionAngle * Mathf.Deg2Rad) * length), Mathf.RoundToInt(startTile.Y + Mathf.Sin(directionAngle * Mathf.Deg2Rad) * length));
            Vector2 direction = endPos - startPos;
            HashSet<T> lines = new HashSet<T>();
            isClear = true;
            ConeCast(map, startTile, length, openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="length">The length of the line</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="direction">The direction of the line from the start tile</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] map, out bool isClear, T startTile, int length, float openingAngle, Vector2 direction, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            float magnitude = new Vector2Int(map.GetLength(0), map.GetLength(1)).magnitude;
            if (length > magnitude || Mathf.Approximately(length, 0f))
            {
                length = Mathf.CeilToInt(magnitude);
            }
            HashSet<T> lines = new HashSet<T>();
            isClear = true;
            ConeCast(map, startTile, length, openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
        /// <summary>
        /// Get all visible tiles from a start tile's cone of vision<br/>
        /// Note that the order of the tiles into the returned array is not guaranteed.
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="map">A two-dimensional array of tiles</param>
        /// <param name="isClear">Is the line of sight clear (no non-walkable tile encountered)</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="openingAngle">The cone opening angle in degrees [1-360]</param>
        /// <param name="endPosition">The destination virtual coordinates (do not need to be into grid range)</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <returns>An array of tiles</returns>
        public static T[] GetConeOfVision<T>(T[,] map, out bool isClear, T startTile, float openingAngle, Vector2Int endPosition, bool includeStart = true) where T : ITile
        {
            openingAngle = Mathf.Clamp(openingAngle, 1f, 360f);
            Vector2Int startPos = new Vector2Int(startTile.X, startTile.Y);
            Vector2 direction = endPosition - startPos;
            HashSet<T> lines = new HashSet<T>();
            isClear = true;
            ConeCast(map, startTile, Mathf.CeilToInt(direction.magnitude), openingAngle, direction, ref isClear, includeStart, ref lines);
            return lines.ToArray();
        }
    }

    /// <summary>
    /// Allows you to calculate paths between tiles.  
    /// This API offers a method which generates and returns a direction map.A direction map can be seen as a "layer" on top of the user grid that indicates, for each accessible tile, the direction to the next tile, ultimately leading to the target tile.  
    /// A direction map holds all the paths to a target tile from all the accessible tiles on the grid.
    /// Storing this DirectionMap object allows you to reconstruct paths between tiles without having to recalculate them every time, which can be costly in terms of performance.
    /// </summary>
    public class Pathfinding
    {
        private static bool GetTile<T>(T[,] map, int x, int y, out T tile) where T : ITile
        {
            if (x > -1 && y > -1 && x < GridUtils.GetHorizontalLength(map) && y < GridUtils.GetVerticalLength(map))
            {
                tile = GridUtils.GetTile(map, x, y);
                return true;
            }
            tile = default;
            return false;
        }
        private static bool GetLeftNeighbour<T>(T[,] map, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(map, x - 1, y, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetRightNeighbour<T>(T[,] map, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(map, x + 1, y, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetBottomNeighbour<T>(T[,] map, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(map, x, y - 1, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetTopNeighbour<T>(T[,] map, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(map, x, y + 1, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetLeftBottomNeighbour<T>(T[,] map, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(map, x - 1, y - 1, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetLeftTopNeighbour<T>(T[,] map, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(map, x - 1, y + 1, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetRightBottomNeighbour<T>(T[,] map, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(map, x + 1, y - 1, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetRightTopNeighbour<T>(T[,] map, int x, int y, out T nei) where T : ITile
        {
            if (GetTile(map, x + 1, y + 1, out nei))
            {
                if (nei != null && nei.IsWalkable)
                {
                    return true;
                }
            }
            return false;
        }
        private static void GetTileOrthographicNeighbours<T>(ref List<T> nodes, T[,] map, int x, int y) where T : ITile
        {
            T nei;

            bool leftWalkable = GetLeftNeighbour(map, x, y, out nei);
            if (leftWalkable)
            {
                nodes.Add(nei);
            }
            bool rightWalkable = GetRightNeighbour(map, x, y, out nei);
            if (rightWalkable)
            {
                nodes.Add(nei);
            }
            bool bottomWalkable = GetBottomNeighbour(map, x, y, out nei);
            if (bottomWalkable)
            {
                nodes.Add(nei);
            }
            bool topWalkable = GetTopNeighbour(map, x, y, out nei);
            if (topWalkable)
            {
                nodes.Add(nei);
            }
        }
        private static void GetTileNeighbours<T>(ref List<T> nodes, T[,] map, int x, int y) where T : ITile
        {
            T nei;

            bool leftWalkable = GetLeftNeighbour(map, x, y, out nei);
            if (leftWalkable)
            {
                nodes.Add(nei);
            }
            bool rightWalkable = GetRightNeighbour(map, x, y, out nei);
            if (rightWalkable)
            {
                nodes.Add(nei);
            }
            bool bottomWalkable = GetBottomNeighbour(map, x, y, out nei);
            if (bottomWalkable)
            {
                nodes.Add(nei);
            }
            bool topWalkable = GetTopNeighbour(map, x, y, out nei);
            if (topWalkable)
            {
                nodes.Add(nei);
            }

            bool leftBottomWalkable = GetLeftBottomNeighbour(map, x, y, out nei);
            if (leftBottomWalkable)
            {
                nodes.Add(nei);
            }
            bool rightBottomWalkable = GetRightBottomNeighbour(map, x, y, out nei);
            if (rightBottomWalkable)
            {
                nodes.Add(nei);
            }
            bool leftTopWalkable = GetLeftTopNeighbour(map, x, y, out nei);
            if (leftTopWalkable)
            {
                nodes.Add(nei);
            }
            bool rightTopWalkable = GetRightTopNeighbour(map, x, y, out nei);
            if (rightTopWalkable)
            {
                nodes.Add(nei);
            }
        }
        /// <summary>
        /// Generates asynchronously a DirectionMap object that will contain all the pre-calculated paths data between a target tile and all the accessible tiles from this target
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <param name="progress">An optional IProgress object to get the generation progression</param>
        /// <param name="cancelToken">An optional CancellationToken object to cancel the generation</param>
        /// <returns>A DirectionMap object</returns>
        public static Task<DirectionMap> GenerateDirectionMapAsync<T>(T[,] grid, T targetTile, IProgress<float> progress = null, CancellationToken cancelToken = default) where T : ITile
        {
            Task<DirectionMap> task = Task.Run(() =>
            {
                if (targetTile == null || !targetTile.IsWalkable)
                {
                    throw new Exception("Do not try to generate a DirectionMap with an unwalkable (or null) tile as the target");
                }
                int width = grid.GetLength(0);
                int height = grid.GetLength(1);
                Vector2Int gridDimensions = new Vector2Int(width, height);
                int totalSize = width * height;
                NextTileDirection[] directionMap = new NextTileDirection[totalSize];
                bool[] visited = new bool[totalSize];
                int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
                visited[targetIndex] = true;
                directionMap[targetIndex] = NextTileDirection.SELF;
                Queue<T> frontier = new Queue<T>();
                frontier.Enqueue(targetTile);
                List<T> neighbourgs = new();
                T current = default;
                int neighborIndex = -1;
                int visitedCount = 0;
                while (frontier.Count > 0)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    progress?.Report((float)visitedCount / totalSize);
                    current = frontier.Dequeue();
                    GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                    foreach (T neiTile in neighbourgs)
                    {
                        neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                        if (!visited[neighborIndex])
                        {
                            visitedCount++;
                            visited[neighborIndex] = true;
                            directionMap[neighborIndex] = GridUtils.GetDirectionTo(neiTile, current);
                            frontier.Enqueue(neiTile);
                        }
                    }
                    neighbourgs.Clear();
                }
                return new DirectionMap(directionMap, targetIndex);
            });
            return task;
        }
        /// <summary>
        /// Generates a DirectionMap object that will contain all the pre-calculated paths data between a target tile and all the accessible tiles from this target
        /// </summary>
        /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="targetTile">The target tile for the paths calculation</param>
        /// <returns>A DirectionMap object</returns>
        public static DirectionMap GenerateDirectionMap<T>(T[,] grid, T targetTile) where T : ITile
        {
            if (targetTile == null || !targetTile.IsWalkable)
            {
                throw new Exception("Do not try to generate a DirectionMap with an unwalkable (or null) tile as the target");
            }
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int gridDimensions = new Vector2Int(width, height);
            int totalSize = width * height;
            NextTileDirection[] directionMap = new NextTileDirection[totalSize];
            bool[] visited = new bool[totalSize];
            int targetIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, targetTile.X, targetTile.Y);
            visited[targetIndex] = true;
            directionMap[targetIndex] = NextTileDirection.SELF;
            Queue<T> frontier = new Queue<T>();
            frontier.Enqueue(targetTile);
            List<T> neighbourgs = new();
            T current = default;
            int neighborIndex = -1;
            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();
                GetTileOrthographicNeighbours(ref neighbourgs, grid, current.X, current.Y);
                foreach (T neiTile in neighbourgs)
                {
                    neighborIndex = GridUtils.GetFlatIndexFromCoordinates(gridDimensions, neiTile.X, neiTile.Y);
                    if (!visited[neighborIndex])
                    {
                        visited[neighborIndex] = true;
                        directionMap[neighborIndex] = GridUtils.GetDirectionTo(neiTile, current);
                        frontier.Enqueue(neiTile);
                    }
                }
                neighbourgs.Clear();
            }
            return new DirectionMap(directionMap, targetIndex);
        }
    }

    /// <summary>
    /// You can generate a DirectionMap object that holds pre-calculated paths data.
    /// This way of doing pathfinding is useful for some usages(like Tower Defenses and more) because it calculates once all the paths between one tile, called the "target", and all the accessible tiles from it.
    /// To generate the DirectionMap object, use the GenerateDirectionMap method that needs the* grid* and the target tile from which to calculate the paths, as parameters.
    /// <i>Note that, obviously, any path calculation is valid as long as the user grid, and walkable states of the tiles, remains unchanged</i>
    /// </summary>
    /// <typeparam name="T">The user-defined type representing a tile (needs to implement the ITile interface)</typeparam>
    public class DirectionMap
    {
        internal readonly NextTileDirection[] _directionMap;
        private readonly int _target;

        internal DirectionMap(NextTileDirection[] directionMap, int target)
        {
            _directionMap = directionMap;
            _target = target;
        }
        /// <summary>
        /// Is the tile is accessible from the target into this this DirectionMap. Usefull to check if the tile is usable as a parameter for this DirectionMap's methods.
        /// </summary>
        /// <param name="tile">The tile to check</param>
        /// <returns>A boolean value</returns>
        public bool IsTileAccessible<T>(T[,] grid, T tile) where T : ITile
        {
            if (tile == null)
            {
                return false;
            }
            return _directionMap[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y)] != NextTileDirection.NONE;
        }
        /// <summary>
        /// Returns the tile that has been used as the target to generate this DirectionMap
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <returns></returns>
        public T GetTargetTile<T>(T[,] grid) where T : ITile
        {
            Vector2Int targetCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _target);
            return GridUtils.GetTile(grid, targetCoords.x, targetCoords.y);
        }
        /// <summary>
        /// Get the next tile on the path between a tile and the target.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">A tile</param>
        /// <returns>A tile object</returns>
        public T GetNextTileFromTile<T>(T[,] grid, T tile) where T : ITile
        {
            if (!IsTileAccessible(grid, tile))
            {
                throw new Exception("Do not call DirectionMap method with an inaccessible tile");
            }
            return GetNextTile(grid, tile);
        }
        /// <summary>
        /// Get the next tile on the path between the target and a tile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="tile">The tile</param>
        /// <returns>A Vector2Int direction</returns>
        public NextTileDirection GetNextTileDirectionFromTile<T>(T[,] grid, T tile) where T : ITile
        {
            if (!IsTileAccessible(grid, tile))
            {
                throw new Exception("Do not call DirectionMap method with an inaccessible tile");
            }
            return _directionMap[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y)];
        }
        /// <summary>
        /// Get all the tiles on the path from a tile to the target.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="startTile">The start tile</param>
        /// <param name="includeStart">Include the start tile into the resulting array or not. Default is true</param>
        /// <param name="includeTarget">Include the target tile into the resulting array or not</param>
        /// <returns>An array of tiles</returns>
        public T[] GetPathToTarget<T>(T[,] grid, T startTile, bool includeStart = true, bool includeTarget = true) where T : ITile
        {
            if (!IsTileAccessible(grid, startTile))
            {
                throw new Exception("Do not call DirectionMap method with an inaccessible tile");
            }

            Vector2Int targetCoords = GridUtils.GetCoordinatesFromFlatIndex(new(grid.GetLength(0), grid.GetLength(1)), _target);
            T target = GridUtils.GetTile(grid, targetCoords.x, targetCoords.y);

            T tile = includeStart ? startTile : GetNextTile(grid, startTile);
            bool targetReached = GridUtils.TileEquals(tile, target);
            if (!includeTarget && targetReached)
            {
                return new T[0];
            }
            List<T> tiles = new List<T>() { tile };
            while (!targetReached)
            {
                tile = GetNextTile(grid, tile);
                targetReached = GridUtils.TileEquals(tile, target);
                if (includeTarget || !targetReached)
                {
                    tiles.Add(tile);
                }
            }
            return tiles.ToArray();
        }
        /// <summary>
        /// Get all the tiles on the path from the target to a tile.
        /// </summary>
        /// <param name="grid">A two-dimensional array of tiles</param>
        /// <param name="destinationTile">The destination tile</param>
        /// <param name="includeDestination">Include the destination tile into the resulting array or not. Default is true</param>
        /// <param name="includeTarget">Include the target tile into the resulting array or not</param>
        /// <returns>An array of tiles</returns>
        public T[] GetPathFromTarget<T>(T[,] grid, T destinationTile, bool includeDestination = true, bool includeTarget = true) where T : ITile
        {
            T[] path = GetPathToTarget(grid, destinationTile, includeDestination, includeTarget);
            T[] reversedPath = new T[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                reversedPath[i] = path[path.Length - 1 - i];
            }
            return reversedPath;
        }
        /// <summary>
        /// Returns the Directionmap serialized as a byte array.
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
        /// Returns the Directionmap serialized as a byte array.
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
        public static DirectionMap FromByteArray<T>(T[,] grid, byte[] bytes)
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
        public static Task<DirectionMap> FromByteArrayAsync<T>(T[,] grid, byte[] bytes, IProgress<float> progress = null, CancellationToken cancelToken = default)
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
        private T GetNextTile<T>(T[,] grid, T tile) where T : ITile
        {
            Vector2Int nextTileDirection = GridUtils.NextNodeDirectionToVector2Int(_directionMap[GridUtils.GetFlatIndexFromCoordinates(new(grid.GetLength(0), grid.GetLength(1)), tile.X, tile.Y)]);
            Vector2Int nextTileCoords = new(tile.X + nextTileDirection.x, tile.Y + nextTileDirection.y);
            return GridUtils.GetTile(grid, nextTileCoords.x, nextTileCoords.y);
        }
    }

    /// <summary>
    /// Some utilitary methods
    /// </summary>
    public static class GridUtils
    {
        internal static NextTileDirection GetDirectionTo(ITile tileA, ITile tileB)
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
        internal static NextTileDirection Vector2IntToNextNodeDirection(Vector2Int dir)
        {
            switch (dir)
            {
                case Vector2Int v when v == Vector2Int.zero:
                    return NextTileDirection.SELF;
                case Vector2Int v when v == Vector2Int.up:
                    return NextTileDirection.UP;
                case Vector2Int v when v == Vector2Int.down:
                    return NextTileDirection.DOWN;
                case Vector2Int v when v == Vector2Int.left:
                    return NextTileDirection.LEFT;
                case Vector2Int v when v == Vector2Int.right:
                    return NextTileDirection.RIGHT;
                case Vector2Int v when v == new Vector2Int(-1, 1):
                    return NextTileDirection.UP_LEFT;
                case Vector2Int v when v == new Vector2Int(1, 1):
                    return NextTileDirection.UP_RIGHT;
                case Vector2Int v when v == new Vector2Int(-1, -1):
                    return NextTileDirection.DOWN_LEFT;
                case Vector2Int v when v == new Vector2Int(1, -1):
                    return NextTileDirection.DOWN_RIGHT;
                default:
                    return NextTileDirection.NONE;
            }
        }
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
