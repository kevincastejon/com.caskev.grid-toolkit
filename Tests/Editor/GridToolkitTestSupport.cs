using System;
using UnityEngine;
using Caskev.GridToolkit;

namespace GridToolkitTests
{
    public class TestTile : IWeightedTile
    {
        public bool IsWalkable { get; set; }
        public int X { get; }
        public int Y { get; }
        public float Weight { get; }
        public TestTile(int x, int y, bool walkable = true, float weight = 1f) { X = x; Y = y; IsWalkable = walkable; Weight = weight; }
        public override string ToString() => $"({X},{Y}) Walkable:{IsWalkable} Weight:{Weight}";
    }

    public static class GridFactory
    {
        public static TestTile[,] Build(int w, int h, Func<int, int, bool> walkable = null)
        {
            walkable ??= ((x, y) => true);
            var g = new TestTile[h, w];
            for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) g[y, x] = new TestTile(x, y, walkable(x, y));
            return g;
        }
        public static TestTile[,] Build(bool[,] grid)
        {
            var g = new TestTile[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < g.GetLength(0); i++)
            {
                for (int j = 0; j < g.GetLength(1); j++)
                {
                    g[i, j] = new TestTile(j, i, grid[i, j]);
                }
            }
            return g;
        }
    }
}
